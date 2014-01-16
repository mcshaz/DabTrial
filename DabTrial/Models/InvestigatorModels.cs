using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DabTrial.Infrastructure.Validation;
using DabTrial.Domain.Tables;

namespace DabTrial.Models
{
    public class InvestigatorEditBrief
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "User Name must be between 3 and 50 characters long")]
        [Required]
        public String UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public String Email { get; set; }
        [Required]
        public String FirstName { get; set; }
        [Required]
        public String LastName { get; set; }
        [Required]
        public ProfessionalRoles? ProfessionalRole { get; set; }
    }
    public class InvestigatorCreateEdit : InvestigatorEditBrief
    {
        [Required]
        [DisplayName("Study Centre")]
        public Int32? StudyCentreId { get; set; }
        public IEnumerable<SelectListItem> StudyCentresSelectList { get; set; }
        [DataType(DataType.MultilineText)]
        public String Comment { get; set; }
        [Required]
        public String Role { get; set; }
        public IEnumerable<SelectListItem> RolesSelectList { get; set; }
        [Display(Name="Database Administrator")]
        public bool IsDbAdmin { get; set; }
        public bool CanAssignDbAdmin { get; set; }
        [TrueOnlyIf("Role", RoleExtensions.PrincipleInvestigator, RoleExtensions.SiteInvestigator)]
        public Boolean IsPublicContact { get; set; }
        [DisplayName("Locked Out")]
        public Boolean IsLockedOut {get; set;}
    }
    public class InvestigatorDetailsBrief
    {
        public String UserName { get; set; }
        public String Email { get; set; }
        public String FullName { get; set; }
        [DisplayName("Hospital")]
        public String StudyCentreName { get; set; }
        public String ProfessionalRole { get; set; }
    }
    public class InvestigatorDetailsFull : InvestigatorDetailsBrief
    {
        public String Comment { get; set; }
        public String Role { get; set; }
        public Boolean IsPublicContact { get; set; }
        [DisplayName("Locked Out")]
        public Boolean IsLockedOut { get; set; }
    }
    public class InvestigatorListItem
    {
        public String UserName { get; set; }
        public String FullName { get; set; }
        public String Email { get; set; }
        [Display(Name="Hospital")]
        public String StudyCentreAbbreviation { get; set; }
        public bool IsLockedOut { get; set; }
        public String ProfessionalRole { get; set; }
        public String Roles { get; set; }
        public Boolean IsPublicContact { get; set; }
    }
}
