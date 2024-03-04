using PDMS.Domain.Entities;

namespace PDMS.Services;

public interface IUserService {
    public Task<User> CreateUser(User userInfo, string rawPassword, string role);
    public Task DeleteUser(string id);
    public Task<string> CreateAccessToken(string email, string rawPassword, bool rememberMe);
}