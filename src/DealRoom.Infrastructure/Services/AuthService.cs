using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DealRoom.Core.DTOs;
using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DealRoom.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser is not null)
                throw new Exception("Account with simillar email already exists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = passwordHash
        } ;
        await _userRepository.CreateAsync(user);
        var token = GenerateToken(user);
        return new AuthResponse { AccessToken = token, RefreshToken= ""};
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new []
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials : creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser is null)
            throw new Exception("Email is not registered.");
        
        var passwordHash = BCrypt.Net.BCrypt.Verify(request.Password, existingUser.PasswordHash);
        if (passwordHash is false)
            throw new Exception("Wrong password");

        var token = GenerateToken(existingUser);
        return new AuthResponse { AccessToken = token, RefreshToken = ""};
    }
}