using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PDMS.Domain.Entities;
using PDMS.Shared.Constants;

namespace PDMS.Services;

public class UserService : IUserService {
    private readonly UserManager<User> _userManager;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public UserService(
        UserManager<User> userManager,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration
    ) {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<User> CreateUser(User userInfo, string rawPassword, string role) {
        if ((new string[] {
                RolesConstants.SALEMAN,
                RolesConstants.CUSTOMER,
                RolesConstants.DIRECTOR,
                RolesConstants.SUPERVISOR,
                RolesConstants.ACCOUNTANT
            }).All(x => x != role)) {
            throw new Exception("Invalid role");
        }

        var duppUser = await _userManager.FindByEmailAsync(userInfo.Email);
        if (duppUser != null) {
            throw new Exception($"User with email {userInfo.Email} already exist.");
        }

        var newUser = new User() {
            UserName = userInfo.UserName ?? "",
            FirstName = userInfo.FirstName ?? "",
            LastName = userInfo.LastName ?? "",
            Email = userInfo.Email,
            PhoneNumber = userInfo.PhoneNumber.Trim(),
            PasswordHash = _passwordHasher.HashPassword(null!, rawPassword),
            LangKey = userInfo.LangKey ?? "vi",
            Activated = true
        };
        await _userManager.CreateAsync(newUser);
        await _userManager.AddToRoleAsync(newUser, role);
        return newUser;
    }

    public async Task DeleteUser(string id) {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) {
            return;
        }

        await _userManager.DeleteAsync(user);
    }

    public async Task<string> CreateAccessToken(string email, string rawPassword, bool rememberMe) {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !(await _userManager.CheckPasswordAsync(user, rawPassword))) {
            throw new Exception("Wrong user login infomation!");
        }

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var identity = new ClaimsIdentity(claims);
        var validity =
            DateTime.UtcNow.AddSeconds(
                rememberMe
                    ? 2592000
                    : 86400
            );
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var tokenDescriptor = new SecurityTokenDescriptor() {
            Expires = validity,
            Subject = identity,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        };
        var token = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        return _jwtSecurityTokenHandler.WriteToken(token);
    }
}