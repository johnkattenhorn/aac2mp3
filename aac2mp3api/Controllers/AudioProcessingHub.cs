using Microsoft.AspNetCore.SignalR;

namespace aac2mp3api.Controllers;

public class AudioProcessingHub : Hub
{
    private const string TempStoragePath = "/tmp/";

    private readonly IAudioConversionService _audioConversionService;

    public AudioProcessingHub(IAudioConversionService audioConversionService)
    {
        _audioConversionService = audioConversionService;
    }

    public async Task ConvertAudioAsync(byte[] fileData, string fileName)
    {
        var inputFileName = Path.Combine(TempStoragePath, fileName);
        var outputFileName = Path.ChangeExtension(inputFileName, ".mp3");

        await File.WriteAllBytesAsync(inputFileName, fileData);

        // Process the audio file
        _audioConversionService.ConvertToMp3(inputFileName, outputFileName);

        // Notify the client when the conversion is complete
        await Clients.Caller.SendAsync("ReceiveNotification", $"Conversion complete: {outputFileName}");

        // Clean up the input file
        File.Delete(inputFileName);
    }
}