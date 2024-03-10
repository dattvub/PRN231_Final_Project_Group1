using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PDMS.Domain.Entities;
using PDMS.Models;
using PDMS.Services;
using PDMS.Services.Interface;
using PDMS.Shared.Constants;
using PDMS.Shared.DTO.Authentication;
using PDMS.Shared.DTO.User;
using PDMS.Shared.Exceptions;

namespace PDMS.Controllers;

[Route("auth")]
[ApiController]
public class AuthenticationController : ControllerBase {
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public AuthenticationController(
        IUserService userService,
        IConfiguration configuration,
        IMapper mapper,
        UserManager<User> userManager
    ) {
        _userService = userService;
        _configuration = configuration;
        _mapper = mapper;
        _userManager = userManager;
    }

    [EnableCors("allowAll")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto) {
        try {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, loginDto.Password))) {
                throw new InvalidLoginException();
            }

            var tokenPair = await _userService.GenerateTokenPair(user);
            SetTokenPairToCookies(tokenPair, loginDto.RememberMe);

            return Ok();
        } catch (InvalidLoginException) {
            return ValidationError.BadRequest400("Email hoặc mật khẩu không chính xác");
        } catch (Exception e) {
            return ValidationError.InternalServerError500(e.Message);
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
    [HttpGet("logout")]
    public IActionResult Logout() {
        SetTokenPairToCookies(
            new TokenPair() {
                AccessToken = "",
                RefreshToken = "",
                AccessTokenExpiryTime = DateTime.UnixEpoch,
                RefreshTokenExpiryTime = DateTime.UnixEpoch
            }, true
        );
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

        var userDto = _mapper.Map<UserDto>(user);
        userDto.Role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        return userDto;
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
                Path = "/auth/refresh",
                Secure = true,
                Expires = rememberMe ? tokenPair.RefreshTokenExpiryTime : null
            }
        );
    }
}