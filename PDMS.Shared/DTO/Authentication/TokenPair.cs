namespace PDMS.Shared.DTO.Authentication;

public class TokenPair {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime AccessTokenExpiryTime { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}