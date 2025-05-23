using System.Text.Json;
using System.Text.Json.Nodes;
using Containers.Application;
using Containers.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");

builder.Services.AddTransient<IContainerService, ContainerService>(
    _ => new ContainerService(connectionString));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/api/containers", (IContainerService containerService) =>
{
    try
    {
        return Results.Ok(containerService.GetAllContainers());
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return Results.Problem();
    }
});

app.MapPost("/api/containers", (IContainerService containerService, Container container) =>
{   
    var result = containerService.CreateContainer(container);
    try
    {
        if (result)
        {
            return Results.Created();
        }
        else
        {
            return Results.BadRequest();
        }
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/custom-containers", async (IContainerService ContainerService, HttpRequest request) =>
{
    using (var reader = new StreamReader(request.Body))
    {
        string rawJson = await reader.ReadToEndAsync();

        var json = JsonNode.Parse(rawJson);
        var specifiedType = json["type"];
        if (specifiedType != null && specifiedType.ToString() == "Container")
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true;
            };
            
            var containerInfo = JsonSerializer.Deserialize<ShortContainerInformation>(json["typeValue"], options);
            ContainerService
        }
    }
});

class ShortContainerInformation
{
    
}

/*
app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();
    */

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}