using System.Threading.Tasks;
using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Services
{
    public interface IJwtService
    {
        Task<string> GenerateToken(User user); 
    }
}
