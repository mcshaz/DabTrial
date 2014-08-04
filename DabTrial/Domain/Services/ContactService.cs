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
            var model = new ForwardMailInvestigator
            {
                To = _db.Users.Find(investigatorId).Email,
                EnquirerEmail = enquirerEmail,
                Subject = subject,
                Message = message
            };

            CreateEmailService.GetEmailService().Send(model);
        }
        public IEnumerable<SiteContact> GetAdministrators()
        {
            string[] adminTypes = new string[] {RoleExtensions.SiteInvestigator, RoleExtensions.PrincipleInvestigator};
            return (from s in _db.StudyCentres
                    select new SiteContact
                    {
                        Name = s.Name,
                        PublicPhoneNumber = s.PublicPhoneNumber,
                        Investigators = s.Investigators.Where(i=>i.Roles.Any(r=>adminTypes.Contains(r.RoleName)))
                            .Select(i => new InvestigatorContact
                        {
                            FullName = i.FirstName + " " + i.LastName,
                            IsPI = i.Roles.Any(r => r.RoleName == RoleExtensions.PrincipleInvestigator),
                            IsPublicContact = i.IsPublicContact,
                            Role = i.ProfessionalRole,
                            UserId = i.UserId
                        }).OrderByDescending(i=>i.IsPublicContact)
                    }).ToList();

        }
    }
}