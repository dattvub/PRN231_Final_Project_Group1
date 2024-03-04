using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PDMS.Domain.Entities;
using PDMS.Services;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO.Authentication;

namespace PDMS.Controllers;

[Route("auth")]
public class AuthenticationController : ControllerBase {
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IUserService userService, IConfiguration configuration) {
        _userService = userService;
        _configuration = configuration;
    }

    [EnableCors("allowAll")]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto) {
        return await _userService.CreateAccessToken(loginDto.Email, loginDto.Password, loginDto.RememberMe);
    }

    [EnableCors("allowAll")]
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(string username, string email, string phoneNumber, string password) {
        return await _userService.CreateUser(
            new User() {
                UserName = username,
                Email = email,
                PhoneNumber = phoneNumber
            }, password, RolesConstants.SALEMAN
        );
    }

    [EnableCors("allowAll")]
    [HttpGet("delete")]
    public async Task<IActionResult> Delete(string id) {
        await _userService.DeleteUser(id);
        return Ok();
    }
}