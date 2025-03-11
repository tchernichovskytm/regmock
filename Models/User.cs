
namespace regmock.Models
{
    public class User
    {
        public string? Fullname{ get; set; }
        public string? Password{ get; set; }
        public string? PhoneNumber{ get; set; }
        public string? Email{ get; set; }
        public Role? UserType{ get; set; }
        public Grade? Grade{ get; set; }
        public DateTime? RegistrationDate{ get; set; }
        public Dictionary<Subject, List<Grade>>? HelperFavorites{ get; set; } // Helper Preferences
    }
}
