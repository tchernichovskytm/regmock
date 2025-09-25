
namespace regmock.Models
{
    public class User
    {
        public string? Fullname { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public Role? UserType { get; set; }
        public Grade? Grade { get; set; }
        public Int64? RegistrationDate { get; set; } // unix milliseconds
        public List<Favorite>? HelperFavorites { get; set; } // Helper Preferences
    }
}
