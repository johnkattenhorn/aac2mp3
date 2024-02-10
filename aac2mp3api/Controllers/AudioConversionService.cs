using System.Diagnostics;

namespace aac2mp3api.Controllers;

public class AudioConversionService : IAudioConversionService
{
    public void ConvertToMp3(string inputFileName, string outputFileName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i \"{inputFileName}\" \"{outputFileName}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        process.WaitForExit();
    }
}