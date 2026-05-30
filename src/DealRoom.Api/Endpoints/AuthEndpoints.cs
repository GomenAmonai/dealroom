using DealRoom.Core.DTOs;
using DealRoom.Core.Interfaces;

namespace DealRoom.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth").WithTags("Auth").RequireRateLimiting("auth");

        group.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
        {
            var result = await authService.RegisterAsync(request);
            return Results.Ok(result);
        })
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status409Conflict);

        group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            return Results.Ok(result);
        })
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh", async (RefreshRequest request, IAuthService authService) =>
        {
            var result = await authService.RefreshAsync(request);
            return Results.Ok(result);
        })
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
