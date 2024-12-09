namespace P7CreateRestApi.Services
{
    public interface ITokenRevocationService
    {
        Task RevokeTokenAsync(string token);
        Task<bool> IsTokenRevokedAsync(string token);
    }
}
