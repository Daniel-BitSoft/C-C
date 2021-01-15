namespace CC.Models
{
    public class CCAntigens : Antigen
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public string ArrayName { get; set; }
        public string MasterArrayName { get; set; }
    }
}
