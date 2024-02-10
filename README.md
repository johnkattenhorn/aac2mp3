# AAC to MP3 Conversion Sample Code Documentation

This documentation covers the AAC to MP3 conversion sample code, consisting of several projects that demonstrate different aspects of the conversion process, including an API, a console application, and integration tests.

## Projects Overview

### 1. aac2mpapi

The `aac2mpapi` project is a .NET Core Web API that includes an `AudioController` with various endpoints to handle audio file conversions:

#### AudioController
- `ConvertAudio`: Synchronously converts an AAC file to MP3 and returns the converted file.
- `ConvertAudioAsync`: Asynchronously enqueues a conversion job and returns immediately. Use this for larger files or when non-blocking operations are needed.
- `IsFileReady`: Checks if the converted file is ready for download. Typically used after `ConvertAudioAsync` to poll for conversion completion.
- `Download`: Downloads the converted MP3 file.

#### Flow for `ConvertAudioAsync`:
1. Call `ConvertAudioAsync` with an AAC file.
2. Poll `IsFileReady` periodically to check if the conversion is complete.
3. Once `IsFileReady` returns true, call `Download` to retrieve the converted MP3 file.

#### AudioProcessingHub
- The SignalR hub `AudioProcessingHub` has a method `ConvertAudioAsync` that handles real-time notifications about the status of the audio conversion process.

#### AudioConversionService
- This service uses FFmpeg to perform the actual audio conversion. FFmpeg must be installed and accessible for the service to function properly.

### Docker Image for aac2mpapi
- The Docker image for `aac2mpapi` includes FFmpeg, ensuring all necessary dependencies are available in the containerized environment.
- For local development on Windows, a Windows version of FFmpeg can be used.

### 2. aac2mp3consoleapp

The `aac2mp3consoleapp` project is a console application demonstrating the use of the `AudioProcessingHub` SignalR hub. It shows how to send a file for processing and listen for real-time notifications from the hub.

### 3. aac2mp3api.test

The `aac2mp3api.test` project contains integration tests for the `aac2mpapi`. 

#### Known Issues:
- Integration tests currently require a local setup with FFmpeg or a Docker image that includes FFmpeg.
- The tests are designed to run against the actual API, and their functionality depends on the availability of FFmpeg.

## Additional Information for Developers

- **Security Considerations**: Ensure proper handling of file names and paths to prevent security vulnerabilities, such as path traversal attacks.
- **Error Handling**: Robust error handling in the API and the conversion service is crucial for a smooth user experience and easier debugging.
- **Performance and Scalability**: The asynchronous conversion approach (`ConvertAudioAsync`) is recommended for handling larger files or for scaling the service to handle multiple concurrent conversion requests.
- **SignalR Notifications**: The real-time notification feature via SignalR enhances user experience by providing timely updates on the conversion process.
- **FFmpeg Dependency**: Ensure FFmpeg is installed and properly configured in both development and production environments. The Docker setup facilitates this in production.
- **Testing Strategy**: The integration tests provide a way to ensure the API's functionality but require an environment similar to the production setup for accurate results.

This sample code provides a comprehensive demonstration of handling audio file conversions in a .NET Core environment, leveraging technologies like SignalR for real-time communication and FFmpeg for audio processing.