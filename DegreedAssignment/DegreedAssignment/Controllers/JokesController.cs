using System;
using DegreedAssignment.Services;
using Microsoft.AspNetCore.Mvc;

namespace DegreedAssignment.Controllers;

[ApiController]
[Route("api/jokes")]
public class JokesController : ControllerBase
{
    private readonly JokeService JokeService;

    public JokesController(JokeService jokeService)
    {
        JokeService = jokeService;
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandomJoke()
    {
        var joke = await JokeService.GetRandomJokeAsync();
        return Ok(joke);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchJokes([FromQuery] string term)
    {
        var jokes = await JokeService.SearchJokesAsync(term);
        var groupedJokes = jokes
            .Select(j => new { Joke = j, Length = j.Split(' ').Length })
            .GroupBy(j => j.Length < 10 ? "Short" : j.Length < 20 ? "Medium" : "Long")
            .ToDictionary(g => g.Key, g => g.Select(j => j.Joke));

        var emphasizedJokes = groupedJokes.ToDictionary(
            g => g.Key,
            g => g.Value.Select(j => j.Replace(term, $"<{term}>")));

        return Ok(emphasizedJokes);
    }
}


