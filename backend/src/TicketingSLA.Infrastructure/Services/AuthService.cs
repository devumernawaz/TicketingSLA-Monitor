using Microsoft.AspNetCore.Identity;
using TicketingSLA.Application.DTOs.Auth;
using TicketingSLA.Application.Interfaces;
using TicketingSLA.Domain.Enums;
using TicketingSLA.Infrastructure.Identity;
using TicketingSLA.Shared.Common;

namespace TicketingSLA.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITenantRepository _tenantRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITenantRepository tenantRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _tenantRepository = tenantRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId);
        if (tenant is null || !tenant.IsActive)
            return Result<AuthResponse>.Failure("Tenant not found.");

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return Result<AuthResponse>.Failure("An account with this email already exists.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            TenantId = request.TenantId,
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result<AuthResponse>.Failure(string.Join(" ", createResult.Errors.Select(e => e.Description)));

        var roleName = request.Role.ToString();
        await _userManager.AddToRoleAsync(user, roleName);

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.DisplayName, roleName, user.TenantId);

        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Role = request.Role,
            TenantId = user.TenantId,
        });
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Result<AuthResponse>.Failure("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault() ?? UserRole.Client.ToString();

        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.DisplayName, roleName, user.TenantId);

        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Role = Enum.Parse<UserRole>(roleName),
            TenantId = user.TenantId,
        });
    }
}
