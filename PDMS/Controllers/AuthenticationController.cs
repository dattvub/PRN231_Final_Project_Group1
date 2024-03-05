using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Services;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO.Authentication;
using PDMS.Shared.DTO.User;
using PDMS.Shared.Exceptions;

namespace PDMS.Controllers;

[Route("auth")]
public class AuthenticationController : ControllerBase {
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthenticationController(IUserService userService, IConfiguration configuration, IMapper mapper) {
        _userService = userService;
        _configuration = configuration;
        _mapper = mapper;
    }

    [EnableCors("allowAll")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto) {
        try {
            var tokenPair = await _userService.AuthorizeUser(loginDto.Email, loginDto.Password);
            SetTokenPairToCookies(tokenPair, loginDto.RememberMe);

            return Ok();
        } catch (Exception e) {
            return ValidationError.BadRequest400(e.Message);
        }
    }

    [EnableCors("allowAll")]
    [HttpGet("refresh")]
    public async Task<IActionResult> RefreshToken() {
        try {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken)) {
                throw new InvalidTokenException();
            }

            var rememberMe = false;
            if (refreshToken.EndsWith(".")) {
                rememberMe = true;
                refreshToken = refreshToken[..^1];
            }

            var user = await _userService.GetUserFromRefreshToken(refreshToken);
            if (user == null) {
                throw new InvalidTokenException();
            }

            var tokenPair = await _userService.GenerateTokenPair(user);
            SetTokenPairToCookies(tokenPair, rememberMe);

            return Ok();
        } catch (InvalidTokenException e) {
            return Unauthorized(e.Message);
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
        }
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

    [EnableCors("allowAll")]
    [HttpGet("checkToken")]
    [Authorize]
    public async Task<ActionResult<UserDto>> CheckToken() {
        var user = await _userService.CheckToken(User);
        if (user == null) {
            return Unauthorized();
        }

        return _mapper.Map<UserDto>(user);
    }

    private void SetTokenPairToCookies(TokenPair tokenPair, bool rememberMe) {
        Response.Cookies.Append(
            "accessToken", tokenPair.AccessToken, new CookieOptions() {
                Domain = Request.Host.Host,
                Secure = true,
                Expires = rememberMe ? tokenPair.AccessTokenExpiryTime : null
            }
        );
        Response.Cookies.Append(
            "refreshToken", tokenPair.RefreshToken + (rememberMe ? "." : ""), new CookieOptions() {
                Domain = Request.Host.Host,
                Path = "/auth/refresh",
                Secure = true,
                Expires = rememberMe ? tokenPair.RefreshTokenExpiryTime : null
            }
        );
    }
}