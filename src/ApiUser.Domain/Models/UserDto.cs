namespace ApiUser.Domain.Models
{
    public class UserDto 
    {
        public string FullName { get; set; } = string.Empty;

        public string Document { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
