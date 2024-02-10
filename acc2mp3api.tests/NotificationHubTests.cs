using aac2mp3api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;

namespace acc2mp3api.tests;

public class NotificationHubTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public NotificationHubTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Optionally override services here if needed
            });
        });
    }

    [Fact]
    public async Task ConvertAudioAsync_SendsNotificationUponCompletion()
    {
        // Arrange
        var client = new HubConnectionBuilder()
            .WithUrl("http://localhost/notificationhub", options =>
            {
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .Build();

        var completionSource = new TaskCompletionSource<string>();
        client.On<string>("ReceiveNotification", message => completionSource.SetResult(message));

        await client.StartAsync();

        // Act
        var dummyData = File.ReadAllBytes("sample4.aac"); // Ensure this test file exists
        await client.SendAsync("ConvertAudioAsync", dummyData, "sample4.aac");

        // Assert
        var notification = await completionSource.Task;
        Assert.Contains("Conversion complete", notification);

        // Cleanup
        await client.DisposeAsync();
    }
}