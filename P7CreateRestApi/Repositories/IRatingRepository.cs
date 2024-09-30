using Dot.Net.WebApi.Controllers.Domain;
using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using System.Security.Cryptography;

namespace P7CreateRestApi.Repositories
{
    public interface IRatingRepository
    {
        Task<Rating> CreateRatingAsync(Rating rating);
        Task<Rating> GetRatingByIdAsync(int id);
        Task<IEnumerable<Rating>> GetAllRatingsAsync();
        Task<Rating> UpdateRatingAsync(Rating rating);
        Task<bool> DeleteRatingAsync(int id);

    }
}
