using ChuckNorrisApi.Data;
using ChuckNorrisApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with PostgreSQL
builder.Services.AddDbContext<ChuckNorrisContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient
builder.Services.AddHttpClient();

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Database seeding logic
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChuckNorrisContext>();
    var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    using var connection = new NpgsqlConnection(connectionString);
    while (true)
    {
        try
        {
            connection.Open();
            break;
        }
        catch
        {
            Thread.Sleep(1000);
        }
    }

    context.Database.EnsureCreated();

    // Check if database already has quotes
    if (!context.Quotes.Any())
    {
        // Fetch random Chuck Norris quotes
        var quotes = new List<ChuckNorrisQuote>();

        for (int i = 0; i < 25; i++) // Fetch 25 quotes
        {
            var response = await httpClient.GetFromJsonAsync<ChuckNorrisApiResponse>("https://api.chucknorris.io/jokes/random");

            if (response != null && !string.IsNullOrEmpty(response.Value))
            {
                quotes.Add(new ChuckNorrisQuote { Quote = response.Value, Url = response.Url });
            }
        }

        if (quotes.Any())
        {
            context.Quotes.AddRange(quotes);
            await context.SaveChangesAsync();
        }
    }
}

app.Run();

// Define a class to parse API response
record ChuckNorrisApiResponse(string Value, string Url);
