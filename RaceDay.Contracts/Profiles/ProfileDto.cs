using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Profiles
{
    public class ProfileDto
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string ProfileImageUrl { get; set; }
    }
}
