namespace CC.Models
{
    public class User
    {
        public string UserId { get; set; }
        public bool IsFirstLogin { get; set; }
        public string FirstName { get; set; }
        public bool IsAdmin { get; set; }
    }
}
