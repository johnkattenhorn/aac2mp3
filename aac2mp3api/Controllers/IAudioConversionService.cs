namespace aac2mp3api.Controllers;

public interface IAudioConversionService
{
    void ConvertToMp3(string inputFileName, string outputFileName);
}