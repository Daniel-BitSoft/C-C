using CC.Constants;
using System.Windows;

namespace CC.Models
{
    public class BatchAntigen
    {
        public string AntigenId { get; set; }
        public string AntigenName { get; set; }
        public string LotNumber { get; set; } 
        public CCType Type { get; set; }

        // view model
        public Visibility ShowExpired { get; set; } = Visibility.Hidden;
    }
}
