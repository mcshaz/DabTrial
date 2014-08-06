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
    public class InvestigatorContact 
    {
        public Int32 UserId { get; set; }
        public String FullName { get; set; }
        public ProfessionalRoles Role { set { ProfessionalRole = value.ToString().ToSeparateWords(); } } //bit of a hack
        public String ProfessionalRole { get; private set; }
        public bool IsPublicContact { get; set; }
        public bool IsPI { get; set; }
        public string SiteName { get; set; }
        public string SitePublicPhoneNumber { get; set; }
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

    public class ForwardMailInvestigator:EmailModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}