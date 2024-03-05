using System.Security.Claims;
using PDMS.Domain.Entities;
using PDMS.Shared.DTO.Authentication;

namespace PDMS.Services;

public interface IUserService {
    public Task<User> CreateUser(User userInfo, string rawPassword, string role);
    public Task DeleteUser(string id);

    public Task<TokenPair> AuthorizeUser(string email, string rawPassword);

    public Task<TokenPair> GenerateTokenPair(User user);
    public Task<string> CreateAccessToken(User user, DateTime issuedTime);
    public string CreateRefreshToken(User user, DateTime issuedTime);
    public bool IsAccessTokenExpire(string token);
    public Task<User?> GetUserFromRefreshToken(string token);
    public Task<User?> CheckToken(ClaimsPrincipal claimsPrincipal);
}