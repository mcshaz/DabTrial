using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DabTrial.Utilities;
using System.Text.RegularExpressions;
using DabTrial.Domain.Tables;
using DabTrial.CustomMembership;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Domain.Providers;


namespace DabTrial.Domain.Services
{
    public class UserService : ServiceLayer
    {
        /// <summary>
        /// Will register and assign User to membership role specified.
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="Password">The password for the new User. If ommitted or null, a new password will be emailed</param>
        public UserService(IValidationDictionary valDictionary=null, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public void CreateUser(String userNameMakingChanges,
                               String userName,
                               String email,
                               String firstName,
                               String lastName,
                               Int32 studyCentreId,
                               String comment,
                               ProfessionalRoles professionalRole,
                               String roleName,
                               Boolean dbAdmin = false,
                               String password = null,
                               Boolean isPublicContact = false,
                               Boolean isApproved = false,
                               Boolean emailOnSuccess = false)
        {
            var roleType = GetRole(roleName);
            if (!roleType.HasValue) {return;}
            User userMakingChanges = _db.Users.Include("StudyCentre").Include("Roles").First(u => u.UserName == userNameMakingChanges);
            StudyCentre userCentre = _db.StudyCentres.Find(studyCentreId);
            CreateUser(userMakingChanges,
                       userName,
                       email,
                       firstName,
                       lastName,
                       userCentre,
                       comment,
                       professionalRole,
                       roleType.Value,
                       dbAdmin,
                       password,
                       isPublicContact,
                       isApproved,
                       emailOnSuccess);
        }
        public void CreateUser(User userMakingChanges,
                               String userName,
                               String email,
                               String firstName,
                               String lastName,
                               StudyCentre userCentre,
                               String comment,
                               ProfessionalRoles professionalRole,
                               InvestigatorRole roleType,
                               Boolean dbAdmin = false,
                               String password = null,
                               Boolean isPublicContact = false,
                               Boolean isApproved = false,
                               Boolean emailOnSuccess = false)
        {
            Validate(email, roleType, dbAdmin, isPublicContact, userCentre, userMakingChanges);
            if (!_validatonDictionary.IsValid) { return; }
            ExecuteCreateUser(userName,
                       email,
                       firstName,
                       lastName,
                       userCentre.StudyCentreId,
                       comment,
                       professionalRole,
                       roleType,
                       dbAdmin,
                       password,
                       isPublicContact,
                       isApproved,
                       emailOnSuccess,
                       userMakingChanges);
        }
        private void ExecuteCreateUser(String userName,
                               String email,
                               String firstName,
                               String lastName,
                               Int32 studyCentreId,
                               String comment,
                               ProfessionalRoles professionalRole,
                               InvestigatorRole roleType,
                               Boolean dbAdmin,
                               String password,
                               Boolean isPublicContact,
                               Boolean isApproved,
                               Boolean emailOnSuccess,
                               User userMakingChanges = null)
        {
            bool noPassword = String.IsNullOrEmpty(password);
            if (noPassword)
            {
                password = Membership.GeneratePassword(Membership.MinRequiredPasswordLength + 2, Membership.MinRequiredNonAlphanumericCharacters + 1);
            }
            MembershipCreateStatus createStatus;
            CodeFirstMembershipProvider.CreateUser(_db, userName, password, email, null, null, isApproved, null, out createStatus);
            if (createStatus == MembershipCreateStatus.Success)
            {
                bool isChangingSelf = userMakingChanges==null || userName == userMakingChanges.UserName;
                if (noPassword || emailOnSuccess)
                {
                    var emailInfo = new Dictionary<string, string>(3);
                    emailInfo.Add("changePasswordUrl", "~/Account/ChangePassword/");
                    emailInfo.Add("UserName", userName);
                    emailInfo.Add("Password", password);
                    Email.Send( toAddress: email,
                                subject: "DAB trial access",
                                fileName: "newUserConfirmation.txt",
                                replacements: emailInfo,
                                replyAddress: isChangingSelf ? null : userMakingChanges.Email);
                }
                User newUser = _db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                newUser.FirstName = firstName;
                newUser.LastName = lastName;
                newUser.StudyCentreId = studyCentreId;
                newUser.ProfessionalRole = professionalRole;
                newUser.IsPublicContact = isPublicContact;
                UpdateRoles(newUser, roleType, dbAdmin);
                _db.SaveChanges(isChangingSelf?userName:userMakingChanges.UserName);
            }
            else
            {
                SetCreateStatusError(createStatus);
            }
        }
        public void UpdatePassword(string userName, string OldPassword, string NewPassword, bool EmailSuccess = true)
        {
            MembershipUser member = CodeFirstMembershipProvider.GetUser(_db, userName, true);
            if (!String.IsNullOrEmpty(NewPassword))
            {
                try
                {
                    if (CodeFirstMembershipProvider.ChangePassword(_db, userName, OldPassword, NewPassword))
                    {
                        _validatonDictionary.AddError("Password", "Password change failed. Please re-enter your values and try again.");
                    }
                }
                catch (Exception e)
                {
                    _validatonDictionary.AddError("", "An exception occurred: " + HttpUtility.HtmlEncode(e.Message) + ". Please re-enter your values and try again.");
                }
            }
            if (EmailSuccess)
            {
                var emailInfo = new Dictionary<string, string>(3);
                emailInfo.Add("UserName", userName);
                emailInfo.Add("NewPassword", NewPassword);
                Email.Send( toAddress: member.Email,
                            subject: "DAB trial account password changed",
                            fileName: "PasswordChanged.txt",
                            replacements: emailInfo);
            }
        }
        public void UpdateSelf(String userName,
                               String email,
                               String firstName,
                               String lastName,
                               ProfessionalRoles professionalRole)
        {
            User user = _db.Users.Include("Roles").Include("StudyCentre").Where(u => u.UserName == userName).FirstOrDefault();
            UpdateUser(user,
                user,
                email,
                firstName,
                lastName,
                user.Comment,
                professionalRole,
                user.InvestigatorRole().Value,
                user.IsDbAdmin(),
                user.IsLockedOut,
                user.IsPublicContact,
                user.IsApproved,
                false);
        }
        public void UpdateUser(String userNameMakingChanges,
                               String userName,
                               String email,
                               String firstName,
                               String lastName,
                               String comment,
                               ProfessionalRoles professionalRole,
                               String roleName,
                               Boolean dbAdmin,
                               Boolean isLockedOut,
                               Boolean isPublicContact = false,
                               Boolean isApproved = false,
                               Boolean emailOnSuccess = false)
        {
            var roleType = GetRole(roleName);
            if (!roleType.HasValue) {return;}
            UpdateUser(userNameMakingChanges,
                       userName,
                       email,
                       firstName,
                       lastName,
                       comment,
                       professionalRole,
                       roleType.Value,
                       dbAdmin,
                       isLockedOut,
                       isPublicContact,
                       isApproved,
                       emailOnSuccess);
        }
        public void UpdateUser(String userNameMakingChanges,
                               String userName,
                               String email,
                               String firstName,
                               String lastName,
                               String comment,
                               ProfessionalRoles professionalRole,
                               InvestigatorRole roleType,
                               Boolean dbAdmin,
                               Boolean isLockedOut,
                               Boolean isPublicContact = false,
                               Boolean isApproved = false,
                               Boolean emailOnSuccess = false)
        {
            User userForUpdate = _db.Users.Include("Roles").Include("StudyCentre").Where(u => u.UserName == userName).FirstOrDefault();
            User userMakingChanges = _db.Users.Include("Roles").Include("StudyCentre").Where(u => u.UserName == userNameMakingChanges).FirstOrDefault();
            if (userForUpdate == null)
            {
                _validatonDictionary.AddError("UserName", "The user " + userName + " could not be found");
                return;
            }
            UpdateUser(userMakingChanges,
                userForUpdate,
                email,
                firstName,
                lastName,
                comment,
                professionalRole,
                roleType,
                dbAdmin,
                isLockedOut,
                isPublicContact,
                isApproved,
                emailOnSuccess);
        }
        private void UpdateUser(User userMakingChanges,
                               User userForUpdate,
                               String email,
                               String firstName,
                               String lastName,
                               String comment,
                               ProfessionalRoles professionalRole,
                               InvestigatorRole roleType,
                               Boolean dbAdmin,
                               Boolean isLockedOut,
                               Boolean isPublicContact = false,
                               Boolean isApproved = false,
                               Boolean emailOnSuccess = false)
        {
            Validate(email, roleType, dbAdmin, isPublicContact, userForUpdate.StudyCentre, userMakingChanges);
            if (!_validatonDictionary.IsValid) { return; }
            //properties in interface but not in membership
            userForUpdate.Email = email;
            userForUpdate.FirstName = firstName;
            userForUpdate.LastName = lastName;
            userForUpdate.Comment = comment;
            userForUpdate.ProfessionalRole = professionalRole;
            if (userForUpdate.IsLockedOut && !isLockedOut)
            {
                userForUpdate.PasswordFailuresSinceLastSuccess = 0;
            }
            userForUpdate.IsLockedOut = isLockedOut;
            userForUpdate.IsPublicContact = isPublicContact;
            var roleNames = UpdateRoles(userForUpdate, roleType, dbAdmin);
            _db.SaveChanges(userMakingChanges.UserName);
            if (emailOnSuccess)
            {
                var emailInfo = new Dictionary<string, string>(3);
                emailInfo.Add("UserName", userForUpdate.UserName);
                emailInfo.Add("Email", email);
                emailInfo.Add("FirsName", firstName);
                emailInfo.Add("LastName", lastName);
                emailInfo.Add("ProfessionalRole", Enum.GetName(typeof(ProfessionalRoles), professionalRole).ToSeparatedWords());
                emailInfo.Add("Roles", roleNames.Aggregate((current, next) => current + ", " + next));
                Email.Send( toAddress: email,
                            subject: "DAB trial account changed",
                            fileName: "UserDetailsChanged.txt",
                            replacements: emailInfo);
            }
        }
        private string[] UpdateRoles(User usr, InvestigatorRole roleType, Boolean dbAdmin)
        {

            string[] roleNames = dbAdmin ? new string[] { roleType.ToString(), RoleExtensions.DbAdminName } : new string[] { roleType.ToString() };
            List<Role> dbNewRoles = (from d in _db.Roles
                                     where roleNames.Contains(d.RoleName)
                                     select d).ToList();
            if (usr.Roles == null)
            {
                usr.Roles = new List<Role>(2);
            }
            else
            {
                usr.Roles.Clear();
            }
            foreach (Role r in dbNewRoles) { usr.Roles.Add(r); }
            //_db.Entry(DBrecord).State = EntityState.Modified;
            return roleNames;
        }
        public void SelfRegisterClinician(
                               String userName,
                               String email,
                               String personalPassword,
                               String SiteSpecificPassword,
                               String firstName,
                               String lastName,
                               ProfessionalRoles professionalRole,
                               Boolean emailOnSuccess = false)
        {
            StudyCentre clinicianCentre = new StudyCentreService(_validatonDictionary, _db).GetCentreByPassword(SiteSpecificPassword);
            if (clinicianCentre == null)
            {
                _validatonDictionary.AddError("SiteSpecificPassword", "Site specific password is incorrect");
                var attempt = GetOrAddRequestIp();
                attempt.Failures += 1;
                attempt.LastAttempt = DateTime.UtcNow;
                _db.SaveChanges();
                return;
            }
            var roleType = InvestigatorRole.EnrollingClinician;
            Validate(email, roleType, false, clinicianCentre);
            if (!_validatonDictionary.IsValid) { return; }
            ExecuteCreateUser(userName,
                email,
                firstName,
                lastName,
                clinicianCentre.StudyCentreId,
                null,
                professionalRole,
                roleType,
                false,
                personalPassword,
                false,
                true, //approve user at this stage - might be worth email token to finalise approval in future
                emailOnSuccess);

        }

        internal void SetIpStudyCentre(int? studyCentreId)
        {
            var attempt = GetOrAddRequestIp();
            if (attempt.StudyCentreId == null)
            {
                attempt.StudyCentreId = studyCentreId;
                _db.SaveChanges();
            }
        }
        private RegistrationFromIP GetOrAddRequestIp()
        {
            var attempt = _db.RegistrationsFromIP.FirstOrDefault(a => a.IpAddress == RequestIP);
            if (attempt == null)
            {
                attempt = new RegistrationFromIP { IpAddress = RequestIP };
                _db.RegistrationsFromIP.Add(attempt);
            }
            return attempt;
        }

        public IEnumerable<Role> GetAllRoles()
        {
            return _db.Roles.ToList();
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _db.Users.Include("StudyCentre").Include("Roles").ToList();
        }
        public IEnumerable<User> GetUsersFromSameCentre(User user)
        {
            return (from u in _db.Users
                    where u.StudyCentreId == user.StudyCentreId
                    select u).ToList();
        }
        public IEnumerable<User> GetUsersFromSameCentre(String userName)
        {
            return (from u in _db.Users
                    where u.UserName == userName
                    select u.StudyCentre.Investigators).FirstOrDefault().ToList();
        }
        public User GetUser(String userName)
        {
            return _db.Users.Include("Roles").Include("StudyCentre").Where(u => u.UserName == userName).FirstOrDefault();
        }
        public void EmailNewPassword(string email)
        {
            User usr = _db.Users.Where(u => u.Email == email).FirstOrDefault();
            if (usr == null) 
            { 
                _validatonDictionary.AddError("Email","Not found");
                return; 
            }
            string password = CodeFirstMembershipProvider.ResetPassword(usr);
            _db.SaveChanges();
            var emailInfo = new Dictionary<string, string>(3);
            emailInfo.Add("changePasswordUrl", "~/Account/ChangePassword/");
            emailInfo.Add("UserName", usr.UserName);
            emailInfo.Add("Password", password);
            Email.Send( toAddress: email,
                        subject: "DAB trial password",
                        fileName: "ChangePassword.txt",
                        replacements: emailInfo);
        }
        public void DeleteUser(string userMakingChanges, string userName)
        {
            if (userMakingChanges == userName) { _validatonDictionary.AddError("UserName", "You cannot delete yourself."); }
            try
            {
                var user = _db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                if (user == null)
                {
                    _validatonDictionary.AddError("UserName", "Does Not Exist!");
                    return;
                }
                _db.Users.Remove(user);
                _db.SaveChanges(userMakingChanges);
            }
            catch (Exception e)
            {
                Exception innerE = e;
                while (innerE.InnerException != null) { innerE = innerE.InnerException; }
                _validatonDictionary.AddError("", "An exception occurred: " + HttpUtility.HtmlEncode(innerE.Message) + ". Please re-enter your values and try again.");
            }
        }
        public IEnumerable<StudyCentre> GetAllCentres()
        {
            return _db.StudyCentres.ToList();
        }
        public bool CanRegister()
        {
            var attemptNo = _db.RegistrationsFromIP.FirstOrDefault(a => a.IpAddress == RequestIP && a.StudyCentreId == null);
            return (attemptNo == null) ? true : (attemptNo.Failures < CodeFirstMembershipProvider.FixedMaxInvalidPasswordAttempts);
        }
        private byte[] _requestIP;
        protected byte[] RequestIP
        {
            get
            {
                return _requestIP 
                    ?? (_requestIP = System.Net.IPAddress.Parse(System.Web.HttpContext.Current.Request.UserHostAddress).GetAddressBytes());
            }
        }
        private void SetCreateStatusError(MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    _validatonDictionary.AddError("UserName", "Username already exists. Please enter a different User name.");
                    break;

                case MembershipCreateStatus.DuplicateEmail:
                    _validatonDictionary.AddError("Email", "A username for that e-mail address already exists. Please enter a different e-mail address.");
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    _validatonDictionary.AddError("Password", "The password provided is invalid. Please enter a valid password value.");
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    _validatonDictionary.AddError("Email", "The e-mail address provided is invalid. Please check the value and try again.");
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    _validatonDictionary.AddError("Answer", "The password retrieval answer provided is invalid. Please check the value and try again.");
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    _validatonDictionary.AddError("Question", "The password retrieval question provided is invalid. Please check the value and try again.");
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    _validatonDictionary.AddError("UserName", "The User name provided is invalid. Please check the value and try again.");
                    break;
                case MembershipCreateStatus.ProviderError:
                    _validatonDictionary.AddError("", "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.");
                    break;
                case MembershipCreateStatus.UserRejected:
                    _validatonDictionary.AddError("", "The User creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.");
                    break;
                default:
                    _validatonDictionary.AddError("", "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.");
                    break;
            }
        }
        private void Validate(string email, InvestigatorRole roleType, bool dbAdmin, bool isPublicContact, StudyCentre studyCentre, User userMakingChanges)
        {
            if (userMakingChanges == null) { throw new ArgumentNullException("userMakingChanges cannot be null"); }
            Validate(email, roleType, isPublicContact, studyCentre);
            var roleTypeName = roleType.ToString();
            var topRole = (from r in _db.Roles
                           where r.RoleName == roleTypeName
                           orderby r.Rank
                           select r).FirstOrDefault();
            var highestCreatorRole = userMakingChanges.Roles.OrderBy(r => r.Rank).FirstOrDefault();
            int creatorRank = highestCreatorRole == null ? int.MaxValue : (highestCreatorRole.Rank ?? int.MaxValue);
            if (topRole.Rank > creatorRank) { _validatonDictionary.AddError("roleType", "Cannot assign permissions higher than your own"); }
            if (dbAdmin && !userMakingChanges.Roles.Any(r => r.RoleName == RoleExtensions.DbAdminName))
            {
                _validatonDictionary.AddError("dbAdmin", "Can only assign database administrator priveleges if you are a database administrator");
            }
            if (studyCentre != userMakingChanges.StudyCentre && !userMakingChanges.IsPrincipleInvestigator())
            {
                _validatonDictionary.AddError("StudyCentre", "Only principle investigators can create users from centres other than their own");
            }
        }
        private void Validate(string email, InvestigatorRole roleType, bool isPublicContact, StudyCentre centre)
        {
            if (String.IsNullOrWhiteSpace(email)) { throw new ArgumentNullException("email"); }
            if (centre == null) { throw new ArgumentNullException("centre"); }
            if (!IsValidEmail(centre, email))
            {
                _validatonDictionary.AddError("Email", "Must be a valid email address ending in " + centre.ValidEmailDomains.Replace(",", " or ") + ".");
            }
            if (roleType != InvestigatorRole.PrincipleInvestigator && roleType != InvestigatorRole.SiteInvestigator && isPublicContact) 
            { 
                _validatonDictionary.AddError("IsPublicContact", "May only be a public contact if role is Principle or Site Investigator"); 
            }
        }
        private InvestigatorRole? GetRole(string RoleName, bool ignoreCase = true)
        {
            InvestigatorRole returnVal;
            if (Enum.TryParse<InvestigatorRole>(RoleName, true, out returnVal))
            {
                return returnVal;
            }
            _validatonDictionary.AddError("RoleName", string.Format("Is not recognised (must be one of {0})", string.Join(",",typeof(InvestigatorRole).GetEnumNames())));
            return null;
        }
        private bool IsValidEmail(StudyCentre centre, string email)
        {
            Regex validEmail = new Regex(centre.domainRegEx());
            return validEmail.IsMatch(email);
        }

        internal class RoleNameComparer : IEqualityComparer<Role>
        {
            public bool Equals(Role x, Role y)
            {
                return x.RoleName == y.RoleName;
            }
            public int GetHashCode(Role obj)
            {
                return obj.RoleName.GetHashCode();
            }
        }
    }
}