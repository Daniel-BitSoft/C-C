using System.Collections.Generic;

namespace CC.Models
{
    public class AssignedCC
    {
        public string BatchId { get; set; }
        public string RunDate { get; set; }
        public int BlockNumber { get; set; }
        public List<string> CalibratorLotNumbers { get; set; }
        public List<string> NegativeLotNumbers { get; set; }
        public List<string> PositiveLotNumbers { get; set; }
    }
}
