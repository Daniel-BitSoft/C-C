using CC.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
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
            DisabledUsers = users.Where(a => a.IsDisabled).ToList();
            ActiveUsers = users.Except(DisabledUsers).ToList();
        }

        public User ValidateCredentials(string username, string password)
        {

            try
            {
                var user = App.dbcontext.Users.FirstOrDefault(a => a.Username == username.Trim());
                if (user != null)
                {
                    if (CryptoProvider.ValidatePassword(password, user.Password))
                    {
                        App.LoggedInUser = user;
                        return user;
                    }
                }

                return null;
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

        public string CreateUser(User user)
        {
            try
            {
                var existinguser = App.dbcontext.Users.FirstOrDefault(a => a.Username == user.Username.Trim());
                if (existinguser == null)
                {
                    user.CreatedBy = App.LoggedInUser.UserId;
                    user.CreatedDt = DateTime.Now;
                    user.UserId = Guid.NewGuid().ToString();
                    user.RequirePasswordChange = true;
                    user.Password = string.IsNullOrEmpty(user.Password) ? CryptoProvider.HashPassword(UsersConsts.DefaultTempPassword) : CryptoProvider.HashPassword(user.Password);

                    App.dbcontext.Users.Add(user);
                    App.dbcontext.SaveChanges();

                    ActiveUsers.Add(user);
                }
                else
                {
                    return Messages.UsernameTaken;
                }
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

            return string.Empty;
        }

        public string UpdateUser(User user, string auditEvent = null)
        {
            try
            {
                var existinguser = App.dbcontext.Users.FirstOrDefault(a => a.Username == user.Username.Trim());
                if (existinguser == null || existinguser.UserId == user.UserId)
                {
                    user.UpdatedBy = App.LoggedInUser.UserId;
                    user.UpdatedDt = DateTime.Now;
                    user.Password = CryptoProvider.HashPassword(user.Password);

                    Audit audit = new Audit
                    {
                        RecordId = user.UserId,
                        Type = AuditTypes.User.ToString(),
                        Description = auditEvent,
                        UpdatedBy = App.LoggedInUser.UserId,
                        UpdatedDt = DateTime.Now
                    };

                    App.dbcontext.Audits.Add(audit); 
                    App.dbcontext.SaveChanges();
                }
                else
                {
                    return Messages.UsernameTaken;
                }
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(UpdateUser), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }

            return null;
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
                    Description = AuditEvents.UserDeleted.ToString(),
                    UpdatedBy = App.LoggedInUser.UserId,
                    UpdatedDt = DateTime.Now
                };
                App.dbcontext.Audits.Add(audit); 
                App.dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                var logNumber = Logger.Log(nameof(DeleteUser), new Dictionary<string, object>
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
                var logNumber = Logger.Log(nameof(GetUserByUsername), new Dictionary<string, object>
                {
                    { LogConsts.Exception, ex }
                });

                ex.Data.Add(nameof(logNumber), logNumber);
                throw ex;
            }
        }
    }
}
