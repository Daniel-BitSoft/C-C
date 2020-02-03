using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Models
{
    public class UsersResponse : BaseModel
    {
        public List<User> Users { get; set; }
    }
}
