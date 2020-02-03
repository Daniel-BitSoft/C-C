using CC.Constants;
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
        public List<User> ActiveUsers { get; set; }
        public List<User> DisabledUsers { get; set; }

        public UserProvider()
        {
        }

        public async Task UpdateUsersList()
        {
            var users = GetAllUsers();
            DisabledUsers = users.Where(a => a.IsDisabled.HasValue && a.IsDisabled.Value).ToList();
            ActiveUsers = users.Except(DisabledUsers).ToList();
        }

        public User ValidateCredentials(string username, string password)
        {

            try
            {
                var user = App.dbcontext.Users.FirstOrDefault(a => a.Username == username.Trim() && a.Password == password.Trim());
                if (user != null)
                    App.LoggedInUser = user;

                return user;
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetAllUsers), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public void CreateUser(User user)
        {
            try
            {
                user.CreatedBy = App.LoggedInUser.UserId;
                user.CreatedDt = DateTime.Now;
                user.UserId = Guid.NewGuid().ToString();
                user.IsFirstLogin = true;
                user.Password = string.IsNullOrEmpty(user.Password) ? UsersConsts.DefaultTempPassword : user.Password;

                App.dbcontext.Users.Add(user);
                App.dbcontext.SaveChanges();

                ActiveUsers.Add(user);
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateUser), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                user.UpdatedBy = App.LoggedInUser.UserId;
                user.UpdatedDt = DateTime.Now;

                Audit audit = new Audit
                {
                    RecordId = user.UserId,
                    Type = AuditTypes.User.ToString(),
                    Description = "User is modified",
                    UpdatedBy = App.LoggedInUser.UserId,
                    UpdatedDt = DateTime.Now
                };
                App.dbcontext.Audits.Add(audit);

                App.dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(CreateUser), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public void DeleteUser(User user)
        {
            try
            {
                var userToDelete = App.dbcontext.Users.Find(user.UserId);
                App.dbcontext.Users.Remove(userToDelete);

                Audit audit = new Audit
                {
                    RecordId = user.UserId,
                    Type = AuditTypes.User.ToString(),
                    Description = "User is deleted",
                    UpdatedBy = App.LoggedInUser.UserId,
                    UpdatedDt = DateTime.Now
                };
                App.dbcontext.Audits.Add(audit);  

                App.dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetAllUsers), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public List<CC.User> GetAllUsers()
        {
            try
            {
                return App.dbcontext.Users.ToList();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetAllUsers), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }

        public List<CC.User> GetUserByUsername(string username, bool IsActive)
        {
            try
            {
                if (IsActive)
                {
                    return ActiveUsers.Where(a => username.Contains(a.Username)).ToList();
                }
                else
                {
                    return DisabledUsers.Where(a => username.Contains(a.Username)).ToList();
                }
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(GetAllUsers), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }
    }
}
