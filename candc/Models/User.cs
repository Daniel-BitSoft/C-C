using System;

namespace CC.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsLocked { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? IsFirstLogin { get; set; }
    }
}
