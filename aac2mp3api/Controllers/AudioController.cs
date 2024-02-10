using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace aac2mp3api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AudioController : ControllerBase
{
    private readonly IHubContext<AudioProcessingHub> _hubContext;
    private readonly IAudioConversionService _audioConversionService;
    private const string TempStoragePath = "/tmp/";

    public AudioController(IHubContext<AudioProcessingHub> hubContext,IAudioConversionService audioConversionService)
    {
        _hubContext = hubContext;
        _audioConversionService = audioConversionService;
    }

    [HttpGet("Download")]
    public IActionResult Download(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest("File name is not provided.");
        }

        var filePath = Path.Combine(TempStoragePath, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File does not exist.");
        }

        // Set the correct content type for MP3 files
        const string contentType = "audio/mpeg";

        // Get the byte array of the file
        var fileBytes = System.IO.File.ReadAllBytes(filePath);

        // Return the file with the specified content type and a suggested file name for the user to save
        return File(fileBytes, contentType, fileName);
    }

    [HttpGet("IsFileReady")]
    public IActionResult IsFileReady(string fileName)
    {
        var filePath = Path.Combine(TempStoragePath, fileName);
        var isFileReady = CheckIfFileIsReady(filePath);
        return Ok(isFileReady);
    }

    [HttpPost("ConvertAudio")]
    public async Task<IActionResult> ConvertAudio(IFormFile audioFile)
    {
        if (audioFile == null || audioFile.Length == 0)
        {
            return BadRequest("File is not provided or is empty.");
        }

        var outputFileName = Path.ChangeExtension(audioFile.FileName, ".mp3");

        // Read the uploaded file into a byte array
        byte[] fileData;
        using (var ms = new MemoryStream())
        {
            await audioFile.CopyToAsync(ms);
            fileData = ms.ToArray();
        }

        // Process the file and convert it
        await ProcessAudioFile(fileData, outputFileName);

        // Read the converted file into memory and return it
        var outputFilePath = Path.Combine(TempStoragePath, outputFileName);
        var memory = new MemoryStream();

        await using (var stream = new FileStream(outputFilePath, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;

        // Clean up the converted file
        System.IO.File.Delete(outputFilePath);

        return File(memory, "audio/mpeg", outputFileName);
    }



    [HttpPost("ConvertAudioAsync")]
    public IActionResult ConvertAudioAsync(IFormFile audioFile)
    {
        if (audioFile == null || audioFile.Length == 0)
        {
            return BadRequest("File is not provided or is empty.");
        }

        var fileName = Path.GetFileName(audioFile.FileName);
        var outputFileName = Path.ChangeExtension(fileName, ".mp3");

        using var ms = new MemoryStream();
        audioFile.CopyTo(ms);
        var fileData = ms.ToArray();

        // Enqueue the job with file data and file name
        BackgroundJob.Enqueue(() => ProcessAudioFile(fileData, outputFileName));

        return Accepted(new { FileName = outputFileName });
    }



    [NonAction]
    public async Task ProcessAudioFile(byte[] fileData, string outputFileName)
    {
        var tempInputFileName = Path.GetTempFileName(); // Create a unique temp file
        var outputFilePath = Path.Combine(TempStoragePath, outputFileName);

        try
        {
            await System.IO.File.WriteAllBytesAsync(tempInputFileName, fileData);
            
            _audioConversionService.ConvertToMp3(tempInputFileName, outputFilePath);
        }
        finally
        {
            // Clean up: delete the temp input file
            if (System.IO.File.Exists(tempInputFileName))
            {
                System.IO.File.Delete(tempInputFileName);
            }
        }
    }

    private static bool CheckIfFileIsReady(string filePath)
    {
        var outputFileName = Path.Combine(TempStoragePath, filePath);
        return System.IO.File.Exists(outputFileName);
    }
}