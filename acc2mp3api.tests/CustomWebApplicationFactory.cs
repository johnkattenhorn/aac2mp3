using aac2mp3api;
using aac2mp3api.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace acc2mp3api.tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace services with mocks or stubs as needed
            services.AddSingleton<IAudioConversionService, AudioConversionService>();
            // Mock IHubContext<NotificationHub> can be added similarly
        });
    }
}

internal class MockAudioConversionService : IAudioConversionService
{
    public void ConvertToMp3(string inputFileName, string outputFileName)
    {
        throw new NotImplementedException();
    }
}