namespace CC.Models
{
    public class ArrayAntigenRelation
    {
        public string AntigenId { get; set; }
        public string AntigenName { get; set; }
        public string ArrayId { get; set; }
        public string ArrayName { get; set; }
        public string MasterArrayId { get; set; }
        public string MasterArrayName { get; set; }
        public bool IsSubArray { get; set; }
        public string Group { get; set; }
    }
}
