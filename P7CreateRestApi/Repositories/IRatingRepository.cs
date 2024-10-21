using P7CreateRestApi.Controllers.Domain;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
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
