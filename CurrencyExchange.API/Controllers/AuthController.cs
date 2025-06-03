using CurrencyExchange.API.Data.Entities;
using CurrencyExchange.API.Models;
using CurrencyExchange.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    TokenService tokenService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthRequest request)
    {
        if (await userManager.FindByEmailAsync(request.Email) != null)
            return BadRequest("Email already in use");

        var user = new User
        {
            UserName = request.Email, // Identity wymaga UserName
            Email = request.Email,
            FirstName = "Nowy",
            LastName = "Użytkownik"
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var token = tokenService.GenerateToken(user);
        return Created("", new AuthResponse { Email = user.Email, Token = token });
    }    

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest("Invalid credentials");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return BadRequest("Invalid credentials");

        var token = tokenService.GenerateToken(user);
        return Ok(new AuthResponse { Email = user.Email, Token = token });
    }
}