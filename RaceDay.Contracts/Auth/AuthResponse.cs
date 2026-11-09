using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Contracts.Auth
{
    public class AuthResponse
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Role { get; set; }
    }
}
