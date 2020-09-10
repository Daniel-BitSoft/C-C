using System.Collections.Generic;

namespace CC.Models
{
    public class UsersResponse : BaseModel
    {
        public List<User> Users { get; set; }
    }
}
