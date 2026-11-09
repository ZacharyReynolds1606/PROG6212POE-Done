namespace RaceDay.Api.Models
{
    public class AppUser
    {
        public int AppUserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string ProfileImageUrl { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }

        public ICollection<Event> Events { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; }
    }
}
