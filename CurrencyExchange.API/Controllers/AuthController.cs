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
public async Task<IActionResult> Register([FromBody] AuthRequest model)
{
    if (!model.AcceptTerms)
        return BadRequest(new AuthResponse { Success = false, Message = "Terms must be accepted." });

    if (model.Password != model.ConfirmPassword)
        return BadRequest(new AuthResponse { Success = false, Message = "Passwords do not match." });

    var user = new User
    {
        UserName = model.Username,
        Email = model.Email,
        FirstName = model.FirstName,
        LastName = model.LastName
    };

    var result = await userManager.CreateAsync(user, model.Password);

    if (!result.Succeeded)
    {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return BadRequest(new AuthResponse { Success = false, Message = errors });
    }

    return StatusCode(201, new AuthResponse
    {
        Success = true,
        Message = "User registered successfully."
    });
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
        return Ok(new AuthResponse { Token = token });
    }
}