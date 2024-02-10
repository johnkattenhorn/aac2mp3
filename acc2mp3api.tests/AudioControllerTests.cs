using System.Net;

namespace acc2mp3api.tests;

public class AudioControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AudioControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task IsFileReady_ReturnsCorrectStatus()
    {
        // Arrange
        var fileName = "test.mp3";
        // Ensure this file exists in the TempStoragePath or mock its existence

        // Act
        var response = await _client.GetAsync($"/api/audio/IsFileReady?fileName={fileName}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        // Further assertions based on the expected response
    }

    [Fact]
    public async Task ConvertAudio_ReturnsFile()
    {
        // Arrange
        var multipartContent = new MultipartFormDataContent();
        var dummyFileContent = new ByteArrayContent(File.ReadAllBytes("sample4.asc"));
        multipartContent.Add(dummyFileContent, "audioFile", "sample4.asc");

        // Act
        var response = await _client.PostAsync("/api/audio/ConvertAudio", multipartContent);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsByteArrayAsync();
        Assert.NotEmpty(responseContent);
        // Further assertions to check if the response is a valid MP3 file
    }

    [Fact]
    public async Task ConvertAudioAsync_AcceptsFile()
    {
        // Arrange
        var multipartContent = new MultipartFormDataContent();
        var dummyFileContent = new ByteArrayContent(File.ReadAllBytes("sample4.asc"));
        multipartContent.Add(dummyFileContent, "audioFile", "sample4.asc");

        // Act
        var response = await _client.PostAsync("/api/audio/ConvertAudioAsync", multipartContent);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("FileName", responseContent);
        // Further assertions based on the expected response
    }

}