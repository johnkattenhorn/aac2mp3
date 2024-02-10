using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:32774/audioConversionHub") // Replace with your actual hub URL
            .Build();

        hubConnection.On<string>("ReceiveNotification", message =>
        {
            Console.WriteLine($"Notification: {message}");
        });

        try
        {
            await hubConnection.StartAsync();
            Console.WriteLine("Connected to hub");

            var filePath = "sample4.aac"; // Replace with the actual path to sample4.aac
            var fileBytes = await File.ReadAllBytesAsync(filePath);

            // Assuming the hub has a method called "ConvertAudioAsync" that accepts a byte array and file name
            await hubConnection.InvokeAsync("ConvertAudioAsync", fileBytes, "sample4.aac");

            Console.WriteLine("File sent for processing. Waiting for notification...");

            Console.ReadLine(); // Keep the app running to listen for messages
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}