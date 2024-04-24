using Microsoft.AspNetCore.Identity;

namespace Identity.API
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; } = string.Empty;
    }
}
