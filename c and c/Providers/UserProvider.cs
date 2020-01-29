using CC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC.Providers
{
    public class UserProvider
    {
        public User ValidateCredentials(string username, string password)
        {
            // validate credentials

            return new User { FirstName = "firstname", IsFirstLogin = false, IsAdmin = true, UserId = Guid.NewGuid().ToString() };

        }

        public User CreateUser(User user)
        {
            // create user

            return user;
        }

        public bool DisableUser(string userId)
        {
            // update user set to disabled
            // Query Get DB

            // Update set disabled, updatedt, updatedBy

            // update audit

            return true;
        }

        public bool EnableUser(string userId)
        {
            // update user set to disabled
            // Query Get DB

            // Update set IsDisable = false, updatedt, updatedBy

            // update audit

            return true;
        }

        public DeleteUserResponse DeleteUser(string username, string password)
        {
            // Delete user if disabled
            // Query Get DB

            // Check disabled
            // if disabled
            //  return new DeleteUserResponse { ErrorMessage = "User is not disabled. Are you sure you want to delete?", IsSuccesful = true }; 


            // delete

            // update audit

            return new DeleteUserResponse { ErrorMessage = "", IsSuccesful = true };
        }
    }
}
