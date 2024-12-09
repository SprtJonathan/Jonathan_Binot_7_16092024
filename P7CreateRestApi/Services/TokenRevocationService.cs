namespace P7CreateRestApi.Services
{
    public class TokenRevocationService : ITokenRevocationService
    {
        private readonly HashSet<string> _revokedTokens = new HashSet<string>();

        public Task RevokeTokenAsync(string token)
        {
            _revokedTokens.Add(token);
            return Task.CompletedTask;
        }

        public Task<bool> IsTokenRevokedAsync(string token)
        {
            return Task.FromResult(_revokedTokens.Contains(token));
        }
    }
}
