using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PDMS.Domain.Entities;
using PDMS.Services.Interface;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO.Authentication;
using PDMS.Shared.Exceptions;

namespace PDMS.Services;

public class UserService : IUserService {
    private readonly UserManager<User> _userManager;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
    private readonly SecurityKey _authSigningKey;
    private readonly SecurityKey _refreshAuthSigningKey;

    public UserService(
        UserManager<User> userManager,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration
    ) {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        _refreshAuthSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:RefreshKey"]));
    }

    public async Task<User> CreateUser(
        User userInfo,
        string rawPassword,
        string? role,
        User creator
    ) {
        if (role != null && (new string[] {
                RolesConstants.SALEMAN,
                RolesConstants.CUSTOMER,
                RolesConstants.DIRECTOR,
                RolesConstants.SUPERVISOR,
                RolesConstants.ACCOUNTANT
            }).All(x => x != role)) {
            throw new BlameClient("Role không hợp lệ.");
        }

        var duppUser = await _userManager.FindByEmailAsync(userInfo.Email);
        if (duppUser != null) {
            throw new BlameClient($"Tài khoản có email {userInfo.Email} đã tồn tại, vui lòng chọn email khác.");
        }

        var newUser = new User() {
            UserName = userInfo.UserName ?? "",
            FirstName = userInfo.FirstName ?? "",
            LastName = userInfo.LastName ?? "",
            Email = userInfo.Email,
            PhoneNumber = userInfo.PhoneNumber.Trim(),
            PasswordHash = _passwordHasher.HashPassword(null!, rawPassword),
            EmailConfirmed = true,
            ImageUrl = userInfo.ImageUrl,
            LangKey = userInfo.LangKey ?? "vi",
            Activated = true,
            CreatedDate = DateTime.Now,
            CreatedBy = creator?.Id
        };
        await _userManager.CreateAsync(newUser);
        if (role != null) {
            await _userManager.AddToRoleAsync(newUser, role);
        }

        return newUser;
    }

    public async Task DeleteUser(string id) {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) {
            return;
        }

        await _userManager.DeleteAsync(user);
    }

    public async Task<TokenPair> GenerateTokenPair(User user) {
        var issuedTime = DateTime.UtcNow;
        return new TokenPair() {
            AccessToken = await CreateAccessToken(user, issuedTime),
            RefreshToken = CreateRefreshToken(user, issuedTime),
            CreatedTime = issuedTime,
            AccessTokenExpiryTime = issuedTime.AddMinutes(10),
            RefreshTokenExpiryTime = issuedTime.AddDays(2)
        };
    }

    public string CreateRefreshToken(User user, DateTime issuedTime) {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        var identity = new ClaimsIdentity(claims);
        return CreateToken(
            identity, new SigningCredentials(_refreshAuthSigningKey, SecurityAlgorithms.HmacSha256), issuedTime,
            issuedTime.AddDays(2)
        );
    }

    public async Task<string> CreateAccessToken(User user, DateTime issuedTime) {
        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var identity = new ClaimsIdentity(claims);
        return CreateToken(
            identity, new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256), issuedTime,
            issuedTime.AddMinutes(10)
        );
    }

    public async Task<User?> CheckToken(ClaimsPrincipal claimsPrincipal) {
        return await _userManager.GetUserAsync(claimsPrincipal);
    }

    public bool IsAccessTokenExpire(string token) {
        var jwtSecurityToken = ValidateToken(token, _authSigningKey, true);
        if (jwtSecurityToken == null) {
            throw new Exception("Access token is not valid");
        }

        return jwtSecurityToken.ValidTo < DateTime.UtcNow.AddSeconds(2);
    }

    public async Task<User?> GetUserFromRefreshToken(string token) {
        var jwtSecurityToken = ValidateToken(token, _refreshAuthSigningKey);
        if (jwtSecurityToken == null) {
            throw new InvalidTokenException();
        }

        var userId = jwtSecurityToken.Payload["nameid"] as string;
        if (userId == null) {
            throw new InvalidTokenException();
        }

        return await _userManager.FindByIdAsync(userId);
    }

    private string CreateToken(
        ClaimsIdentity claimsIdentity,
        SigningCredentials signingCredentials,
        DateTime issuedTime,
        DateTime expiryTime
    ) {
        var tokenDescriptor = new SecurityTokenDescriptor() {
            IssuedAt = issuedTime,
            Expires = expiryTime,
            Subject = claimsIdentity,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = signingCredentials
        };
        var token = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        return _jwtSecurityTokenHandler.WriteToken(token);
    }

    private JwtSecurityToken? ValidateToken(string token, SecurityKey securityKey, bool ignoreLifetime = false) {
        try {
            var validationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = !ignoreLifetime,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = securityKey
            };
            _jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return (JwtSecurityToken)validatedToken;
        } catch (Exception) {
            return null;
        }
    }
}