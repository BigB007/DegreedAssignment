using Microsoft.Extensions.Options;
using System.Text.Json;
using DegreedAssignment.Config;

namespace DegreedAssignment.Services;

public class JokeService
{
    private readonly HttpClient HttpClient;
    private readonly ILogger<JokeService> Logger;
    private readonly string JokeApiUrl;
    private const string JokeJsonElement = "joke";

    public JokeService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, ILogger<JokeService> logger)
    {
        HttpClient = httpClient;
        JokeApiUrl = apiSettings.Value.DadJokeApiUrl;
        Logger = logger;
    }

    public async Task<string> GetRandomJokeAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, JokeApiUrl);
            request.Headers.Add("Accept", "application/json");
            var response = await HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty(JokeJsonElement, out var jokeResult))
                {
                    return jokeResult.ToString();
                }

                return string.Format("Error parsing json, joke not found");
            }
            return string.Format("Request failed with { StatusCode}", response.StatusCode);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while fetching a random joke.");
            return "An error occurred while fetching a random joke.";
        }
    }

    public async Task<List<string>> SearchJokesAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            Logger.LogWarning("Search term is null or whitespace.");
            return new List<string> { "Invalid search term." };
        }

        try
        {
            var jokes = new List<string>();
            var jokeSearchUrl = string.Format($"{JokeApiUrl}search?term={term}");
            var request = new HttpRequestMessage(HttpMethod.Get, jokeSearchUrl);
            request.Headers.Add("Accept", "application/json");
            var response = await HttpClient.SendAsync(request);
            if (response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();
                JsonDocument doc = JsonDocument.Parse(json);
                foreach (var element in doc.RootElement.GetProperty("results").EnumerateArray())
                {
                    if (element.TryGetProperty(JokeJsonElement, out var jokeResult))
                    {
                        jokes.Add(jokeResult.ToString());
                    }

                }

                if (!jokes.Any())
                {
                    jokes.Add(string.Format("Error parsing json, joke not found"));
                }
                return jokes;
            }
            jokes.Add(string.Format("Request failed with { StatusCode}", response.StatusCode));
            return jokes;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while searching for jokes.");
            return new List<string> { "An error occurred while searching for jokes." };
        }
    }
}

