using System.Collections.Generic;
using System.Linq;
using DabTrial.Utilities;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Models;


namespace DabTrial.Domain.Services
{
    public class ContactService :ServiceLayer
    {
        public ContactService(IValidationDictionary valDictionary = null, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public void SendMail(int investigatorId, string enquirerEmail, string subject, string message)
        {
            
            /*
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(message);

            doc.DocumentNode.Descendants()
                            .Where(n => n.Name == "script" || n.Name == "#comment")
                            .ToList()
                            .ForEach(n => n.Remove());

            */
            (new CreateEmailService()).ForwardWebMessage(enquirerEmail, _db.Users.Find(investigatorId).Email, subject, message);


            
        }
        public IEnumerable<InvestigatorContact> GetAdministrators()
        {
            string[] adminTypes = new string[] {RoleExtensions.SiteInvestigator, RoleExtensions.PrincipleInvestigator};
            return (from i in _db.Users
                    let site = i.StudyCentre
                    let roles = i.Roles
                    let isPi = roles.Any(r => r.RoleName == RoleExtensions.PrincipleInvestigator)
                    where !i.IsDeactivated && (isPi || roles.Any(r => r.RoleName == RoleExtensions.SiteInvestigator))
                    orderby i.IsPublicContact descending, isPi descending
                    select new InvestigatorContact
                    {
                        FullName = i.FirstName + " " + i.LastName,
                        IsPI = isPi,
                        IsPublicContact = i.IsPublicContact,
                        Role = i.ProfessionalRole,
                        UserId = i.UserId,
                        SiteName = site.Name, 
                        SitePublicPhoneNumber=site.PublicPhoneNumber
                    }).ToList();

        }
    }
}