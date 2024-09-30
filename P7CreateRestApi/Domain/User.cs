using Microsoft.AspNetCore.Identity;

namespace Dot.Net.WebApi.Domain
{
    public class User : IdentityUser<int>
    {
        public string Fullname { get; set; }
        public string Role { get; set; }
    }
}