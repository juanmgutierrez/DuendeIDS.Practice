using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using WeatherMVC.Models;
using WeatherMVC.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace WeatherMVC.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITokenService _tokenService;

    public HomeController(ILogger<HomeController> logger, ITokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Weather()
    {
        //var accessToken = await _tokenService.GetToken("weatherapi.read").AccessToken;
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        
        using var client = new HttpClient();
        client.SetBearerToken(accessToken);

        var result = await client.GetAsync("https://localhost:5445/weatherforecast");
        if(!result.IsSuccessStatusCode)
            throw new Exception("Weather not available");

        var weatherJson = await result.Content.ReadAsStringAsync();
        var weather = JsonSerializer.Deserialize<List<Weather>>(weatherJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(weather);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
