using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DabTrial.Models;
using DabTrial.Domain.Providers;
using System.Security.Cryptography;
using System.Web;
using DabTrial.Domain.Tables;

namespace DabTrial.CustomMembership
{
    public class CodeFirstMembershipProvider : MembershipProvider
    {

        #region Properties

        private const int TokenSizeInBytes = 16;
        internal const int FixedRequiredNonAlphanumChars = 1;
        internal const int FixedRequiredPasswordLen = 6;
        internal const int FixedMaxInvalidPasswordAttempts = 5;
        internal const string PasswordDescription = "6+ characters long and 1 character which is not a number or letter";

        public override string ApplicationName
        {
            get
            {
                return this.GetType().Assembly.GetName().Name.ToString();
            }
            set
            {
                this.ApplicationName = this.GetType().Assembly.GetName().Name.ToString();
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return FixedMaxInvalidPasswordAttempts; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return FixedRequiredNonAlphanumChars; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return FixedRequiredPasswordLen; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 0; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return String.Empty; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        #endregion

        #region Functions
        public static MembershipUser CreateUser(IDataContext context, string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (string.IsNullOrEmpty(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (string.IsNullOrEmpty(password) || !IsValidPassword(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (string.IsNullOrEmpty(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            string hashedPassword = PBKDF2.HashPassword(password);
            if (hashedPassword.Length > 128)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (context.Users.Where(Usr => Usr.UserName == username).Any())
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            if (context.Users.Where(Usr => Usr.Email == email).Any())
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            DateTime now = DateTime.UtcNow;
            User NewUser = new User
            {
                //UserId = Guid.NewGuid(), - Identity (database assigned)
                UserName = username,
                Password = hashedPassword,
                IsApproved = isApproved,
                Email = email,
                CreateDate = now,
                LastPasswordChangedDate = now,
                PasswordFailuresSinceLastSuccess = 0,
                LastLoginDate = now,
                LastActivityDate = now,
                LastLockoutDate = now,
                IsLockedOut = false,
                LastPasswordFailureDate = now
            };

            context.Users.Add(NewUser);
#if debug
            try
            {
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        throw new Exception();
                    }
                }
            }
#else
            context.SaveChanges();
#endif

            status = MembershipCreateStatus.Success;
            return new MembershipUser(Membership.Provider.Name, NewUser.UserName, NewUser.UserId, NewUser.Email, null, null, NewUser.IsApproved, NewUser.IsLockedOut, NewUser.CreateDate.Value, NewUser.LastLoginDate.Value, NewUser.LastActivityDate.Value, NewUser.LastPasswordChangedDate.Value, NewUser.LastLockoutDate.Value);
        }
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            using (var context = new DataContext())
            {
                return CreateUser(context, username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
            }
        }

        public string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            return CreateAccount(userName, password, requireConfirmation);
        }
        public override bool ValidateUser(string username, string password)
        {
            using (var context = new DataContext())
            {
                return (ValidateUser(context, username, password)!=null);
            }
        }
        public static User ValidateUser(IDataContext context, string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            
            User user = null;
            user = context.Users.FirstOrDefault(Usr => Usr.UserName == username);
            if (user == null || !user.IsApproved || user.IsLockedOut)
            {
                return null;
            }

            String HashedPassword = user.Password;
            Boolean VerificationSucceeded = (HashedPassword != null && PBKDF2.VerifyHashedPassword(HashedPassword, password));
            DateTime now = DateTime.UtcNow;
            if (VerificationSucceeded)
            {
                user.PasswordFailuresSinceLastSuccess = 0;
                user.LastLoginDate = now;
                user.LastActivityDate = now;
            }
            else
            {
                int Failures = user.PasswordFailuresSinceLastSuccess;
                if (Failures < FixedMaxInvalidPasswordAttempts)
                {
                    user.PasswordFailuresSinceLastSuccess += 1;
                    user.LastPasswordFailureDate = now;
                }
                else
                {
                    user.LastPasswordFailureDate = now;
                    user.LastLockoutDate = now;
                    user.IsLockedOut = true;
                }
            }
            context.SaveChanges();
            if (VerificationSucceeded)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public static MembershipUser GetUser(IDataContext context, string username, bool userIsOnline)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            User user = context.Users.FirstOrDefault(Usr => Usr.UserName == username);
            if (user != null)
            {
                if (userIsOnline)
                {
                    user.LastActivityDate = DateTime.UtcNow;
                    context.SaveChanges();
                }
                return new MembershipUser(Membership.Provider.Name, 
                    user.UserName, 
                    user.UserId, 
                    user.Email, 
                    null, 
                    null, 
                    user.IsApproved, 
                    user.IsLockedOut, 
                    user.CreateDate.Value, 
                    user.LastLoginDate.Value, 
                    user.LastActivityDate.Value, 
                    user.LastPasswordChangedDate.Value, 
                    user.LastLockoutDate.Value);
            }
            else
            {
                return null;
            }
        }
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (var context = new DataContext())
            {
                return GetUser(context, username, userIsOnline);
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey is Guid) { return null;  } //note for int user id

            using (var Context = new DataContext())
            {
                User User = null;
                User = Context.Users.Find(providerUserKey);
                if (User != null)
                {
                    if (userIsOnline)
                    {
                        User.LastActivityDate = DateTime.UtcNow;
                        Context.SaveChanges();
                    }
                    return new MembershipUser(Membership.Provider.Name, User.UserName, User.UserId, User.Email, null, null, User.IsApproved, User.IsLockedOut, User.CreateDate.Value, User.LastLoginDate.Value, User.LastActivityDate.Value, User.LastPasswordChangedDate.Value, User.LastLockoutDate.Value);
                }
                else
                {
                    return null;
                }
            }
        }
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (var context = new DataContext())
            {
                return ChangePassword(context, username, oldPassword, newPassword);
            }
        }
        public static bool ChangePassword(IDataContext context, string username, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(oldPassword))
            {
                return false;
            }
            if (string.IsNullOrEmpty(newPassword) || !IsValidPassword(newPassword))
            {
                return false;
            }

            User user = null;
            user = context.Users.FirstOrDefault(Usr => Usr.UserName == username);
            if (user == null)
            {
                return false;
            }
            String HashedPassword = user.Password;
            Boolean VerificationSucceeded = (HashedPassword != null && PBKDF2.VerifyHashedPassword(HashedPassword, oldPassword));
            if (VerificationSucceeded)
            {
                user.PasswordFailuresSinceLastSuccess = 0;
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                int Failures = user.PasswordFailuresSinceLastSuccess;
                if (Failures < FixedMaxInvalidPasswordAttempts)
                {
                    user.PasswordFailuresSinceLastSuccess += 1;
                    user.LastPasswordFailureDate = now;
                }
                else
                {
                    user.LastPasswordFailureDate = now;
                    user.LastLockoutDate = now;
                    user.IsLockedOut = true;
                }
                context.SaveChanges();
                return false;
            }
//
            if (CreateNewPassword(user, newPassword))
            {
                context.SaveChanges();
                return true;
            }
            return false;
        }
        private static bool CreateNewPassword(User user, string newPassword)
        {
            String NewHashedPassword = PBKDF2.HashPassword(newPassword);
            if (NewHashedPassword.Length > 128)
            {
                return false;
            }
            user.Password = NewHashedPassword;
            user.LastPasswordChangedDate = DateTime.UtcNow;
            return true;
        }

        public override bool UnlockUser(string userName)
        {
            using (var context = new DataContext())
            {
                User user = context.Users.FirstOrDefault(Usr => Usr.UserName == userName);
                if (user != null)
                {
                    user.IsLockedOut = false;
                    user.PasswordFailuresSinceLastSuccess = 0;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            DateTime DateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(Convert.ToDouble(Membership.UserIsOnlineTimeWindow)));
            using (var context = new DataContext())
            {
                return context.Users.Where(Usr => Usr.LastActivityDate > DateActive).Count();
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            using (var context = new DataContext())
            {
                User user = context.Users.FirstOrDefault(Usr => Usr.UserName == username);
                if (user != null)
                {
                    context.Users.Remove(user);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            using (var context = new DataContext())
            {
                User user = context.Users.FirstOrDefault(Usr => Usr.Email == email);
                if (user != null)
                {
                    return user.UserName;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection MembershipUsers = new MembershipUserCollection();
            using (var context = new DataContext())
            {
                totalRecords = context.Users.Where(Usr => Usr.Email == emailToMatch).Count();
                IQueryable<User> users = context.Users.Where(Usr => Usr.Email == emailToMatch).OrderBy(Usrn => Usrn.UserName).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (User user in users)
                {
                    MembershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value, user.LastLoginDate.Value, user.LastActivityDate.Value, user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value));
                }
            }
            return MembershipUsers;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection MembershipUsers = new MembershipUserCollection();
            using (var context = new DataContext())
            {
                totalRecords = context.Users.Where(Usr => Usr.UserName == usernameToMatch).Count();
                IQueryable<User> users = context.Users.Where(Usr => Usr.UserName == usernameToMatch).OrderBy(Usrn => Usrn.UserName).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (User user in users)
                {
                    MembershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value, user.LastLoginDate.Value, user.LastActivityDate.Value, user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value));
                }
            }
            return MembershipUsers;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection MembershipUsers = new MembershipUserCollection();
            using (var context = new DataContext())
            {
                totalRecords = context.Users.Count();
                IQueryable<User> users = context.Users.OrderBy(Usrn => Usrn.UserName).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (User user in users)
                {
                    MembershipUsers.Add(new MembershipUser(Membership.Provider.Name, user.UserName, user.UserId, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreateDate.Value, user.LastLoginDate.Value, user.LastActivityDate.Value, user.LastPasswordChangedDate.Value, user.LastLockoutDate.Value));
                }
            }
            return MembershipUsers;
        }

        public string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {

            if (string.IsNullOrEmpty(userName))
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.InvalidUserName);
            }

            if (string.IsNullOrEmpty(password) || !IsValidPassword(password))
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.InvalidPassword);
            }

            string hashedPassword = PBKDF2.HashPassword(password);
            if (hashedPassword.Length > 128)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.InvalidPassword);
            }

            using (DataContext context = new DataContext())
            {
                if (context.Users.Any(u => u.UserName == userName))
                {
                    throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);
                }

                string token = string.Empty;
                if (requireConfirmationToken)
                {
                    token = GenerateToken();
                }

                User NewUser = new User
                {
                    //UserId = Guid.NewGuid(),
                    UserName = userName,
                    Password = hashedPassword,
                    IsApproved = !requireConfirmationToken,
                    Email = string.Empty,
                    CreateDate = DateTime.UtcNow,
                    LastPasswordChangedDate = DateTime.UtcNow,
                    PasswordFailuresSinceLastSuccess = 0,
                    LastLoginDate = DateTime.UtcNow,
                    LastActivityDate = DateTime.UtcNow,
                    LastLockoutDate = DateTime.UtcNow,
                    IsLockedOut = false,
                    LastPasswordFailureDate = DateTime.UtcNow,
                    ConfirmationToken = token
                };

                context.Users.Add(NewUser);
                context.SaveChanges();
                return token;
            }
        }

        private static string GenerateToken()
        {
            using (var prng = new RNGCryptoServiceProvider())
            {
                return GenerateToken(prng);
            }
        }

        internal static string GenerateToken(RandomNumberGenerator generator)
        {
            byte[] tokenBytes = new byte[TokenSizeInBytes];
            generator.GetBytes(tokenBytes);
            return HttpServerUtility.UrlTokenEncode(tokenBytes);
        }

        internal static bool IsValidPassword(string password)
        {
            if (password.Length < FixedRequiredPasswordLen) { return false;}
            if (FixedRequiredNonAlphanumChars > 0 && password.Count(c => !char.IsLetterOrDigit(c))<FixedRequiredNonAlphanumChars) { return false; }
            return true;
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }
        public override string ResetPassword(string username, string answer)
        {
            using (DataContext context = new DataContext())
            {
                User user = context.Users.FirstOrDefault(u=>u.UserName==username);
                if (user == null) { return null; }
                return ResetPassword(user/*, answer */);
            }
        }
        public static string ResetPassword(User user /*, string answer*/)
        {
            string password = Membership.GeneratePassword(FixedRequiredPasswordLen + 2, FixedRequiredNonAlphanumChars + 1);
            if (!CreateNewPassword(user, password)) { password = null; }
            return password;
        }

        #endregion

        #region Not Supported

        //CodeFirstMembershipProvider does not support password retrieval scenarios.
        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }


        //CodeFirstMembershipProvider does not support question and answer scenarios.
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        //CodeFirstMembershipProvider does not support UpdateUser because this method is useless.
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}