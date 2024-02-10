using aac2mp3api.Controllers;
using Hangfire;
using Hangfire.MemoryStorage;

namespace aac2mp3api;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddSignalR();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddTransient<IAudioConversionService, AudioConversionService>();

        // Add Hangfire services
        builder.Services.AddHangfire(configuration => configuration
            .UseMemoryStorage());  // Or UseSqlServerStorage for production
        builder.Services.AddHangfireServer();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<AudioProcessingHub>("/audioConversionHub");

        app.UseHangfireDashboard();

        app.Run();
    }
}