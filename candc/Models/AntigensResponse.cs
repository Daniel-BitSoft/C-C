using System.Collections.Generic;

namespace CC.Models
{
    public class AntigensResponse : BaseModel
    {
        public List<Antigen> Antigens { get; set; }
    }
}
