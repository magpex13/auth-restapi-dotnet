using System.Security.Claims;
using AuthRestApi.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthRestApi.Controllers;


[ApiController]
[Route("api/auth")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly UserDto[] users = new UserDto[]
    {
        new UserDto("conejo1","conejo1@hotmail.com", 19),
        new UserDto("conejo2","conejo2@hotmail.com", 19),
        new UserDto("conejo3","conejo3@hotmail.com", 19),
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    [Route("user")] 
    public bool GetUserInfo()
    {
        return false;
    }

    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto)
    {
        var userExists = users.Any(x=> x.email == signInDto.email);

        if(!userExists){
            return Unauthorized("User not exists!");
        }

        var user = users.Where(x=> x.email == signInDto.email).First();

        var claims = new Claim[] {
            new Claim(ClaimTypes.Email, signInDto.email),
            new Claim(ClaimTypes.GivenName, user.names),
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(claimPrincipal);
        return Ok();
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
