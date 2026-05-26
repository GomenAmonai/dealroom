using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DealRoom.Core.DTOs;
using DealRoom.Core.Entities;
using DealRoom.Core.Exceptions;
using DealRoom.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DealRoom.Infrastructure.Services;

public class AuthService : IAuthService
{
    private const int AccessTokenLifetimeMinutes = 15;
    private const int RefreshTokenLifetimeDays = 7;

    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _config;

    public AuthService(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration config)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new EmailAlreadyExistsException(request.Email);

        var organization = await _organizationRepository.CreateAsync(new Organization
        {
            Name = request.OrganizationName,
            CreatedAt = DateTime.UtcNow
        });

        var user = await _userRepository.CreateAsync(new User
        {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            OrganizationId = organization.Id
        });

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
            throw new InvalidCredentialsException();

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request)
    {
        var stored = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (stored is null || !stored.IsActive)
            throw new InvalidRefreshTokenException();

        await _refreshTokenRepository.RevokeAsync(stored);

        return await IssueTokensAsync(stored.User);
    }

    private async Task<AuthResponse> IssueTokensAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        await _refreshTokenRepository.CreateAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenLifetimeDays),
            CreatedAt = DateTime.UtcNow
        });

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("orgId", user.OrganizationId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(AccessTokenLifetimeMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
