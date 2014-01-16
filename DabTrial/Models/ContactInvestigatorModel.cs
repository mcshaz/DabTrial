using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DabTrial.Utilities;
using DabTrial.Infrastructure.Validation;
using DabTrial.Domain.Tables;

namespace DabTrial.Models
{
    public class InvestigatorContact : ContactLink
    {
        public InvestigatorRole Role { get; set; }
        public CentreDetails StudyCentre { get; set; }
        public class CentreDetails
        {
            public String Name { get; set; }
            public String PublicPhoneNumber { get; set; }
        }
    }
    public class ContactLink
    {
        public Int32 UserId { get; set; }
        public String FullName { get; set; }
        public String ProfessionalRole { get; set; }
        public Boolean IsPublicContact { get; set; }
    }
    public class InvestigatorContactList
    {
        public IEnumerable<InvestigatorContact> ContactList { get; set; }
        public IEnumerable<InvestigatorContact> GetPrincipleInvestigators()
        {
            return ContactList.Where(c => c.Role == InvestigatorRole.PrincipleInvestigator);
        }
        public IEnumerable<IGrouping<InvestigatorContact.CentreDetails, ContactLink>> GetInvestigatorsByCentre()
        {
            return ContactList.GroupBy(i => i.StudyCentre, i => new ContactLink{ FullName=i.FullName, UserId=i.UserId, ProfessionalRole=i.ProfessionalRole, IsPublicContact=i.IsPublicContact });
        }
    }
    public class MailInvestigator
    {
        [HiddenInput]
        public int InvestigatorId { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name="Your Email")]
        [Email]
        public string Email { get; set; }
        [Required]
        [StringLength(60)]
        public string Subject { get; set; }
        [DataType(DataType.MultilineText)]
        [Required]
        [StringLength(500, MinimumLength=10)]
        public string Message { get; set; }
        public InvestigatorInfo DisplayInfo { get; set; }
        public class InvestigatorInfo
        {
            public string Name { get; set; }
            public string Role { get; set; }
            public string Hospital { get; set; }
        }
    }
}