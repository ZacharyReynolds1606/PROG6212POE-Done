using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Profiles
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
