using System.Collections.Generic;
using System.Linq;
using DabTrial.Utilities;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;


namespace DabTrial.Domain.Services
{
    public class ContactService :ServiceLayer
    {
        /// <summary>
        /// Will register and assign User to membership role specified.
        /// </summary>
        /// <param name="newUser"></param>
        /// <param name="Password">The password for the new User. If ommitted or null, a new password will be emailed</param>
        public ContactService(IValidationDictionary valDictionary = null, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public IEnumerable<User> GetAdministrators()
        {
            string[] adminRoles = new string[] { InvestigatorRole.PrincipleInvestigator.ToString(), InvestigatorRole.SiteInvestigator.ToString() };
            return _db.Roles.Include("Users").Include("Users.StudyCentre")
                        .Where(r=>adminRoles.Contains(r.RoleName))
                        .SelectMany(r=>r.Users)
                        .OrderBy(u=>u.StudyCentreId)
                        .ThenByDescending(u=>u.ProfessionalRoleId)
                        .ToList();
        }
        public void sendMail(int userId, string fromAddress, string subject, string message)
        {
            User usr = _db.Users.Find(userId);
            if (usr == null) { _validatonDictionary.AddError("UserId", "Investigator Not Found"); }
            Email.Send(usr.Email, "DAB Trial website enquiry:" + subject, message, fromAddress);
        }
    }
}