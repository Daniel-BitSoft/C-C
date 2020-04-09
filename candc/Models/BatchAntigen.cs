using CC.Constants;

namespace CC.Models
{
    public class BatchAntigen
    {
        public string AntigenId { get; set; }
        public string AntigenName { get; set; }
        public string LotNumber { get; set; } 
        public CCType Type { get; set; } 
    }
}
