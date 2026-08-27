using FluentValidation;
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
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITenantRepository tenantRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _userManager = userManager;
        _tenantRepository = tenantRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var validation = await _registerValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<AuthResponse>.Failure(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

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
        var validation = await _loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return Result<AuthResponse>.Failure(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

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
