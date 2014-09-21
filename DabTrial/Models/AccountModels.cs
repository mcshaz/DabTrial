using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DabTrial.Infrastructure.Validation;
using DabTrial.Domain.Tables;
using DabTrial.CustomMembership;


namespace DabTrial.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = CodeFirstMembershipProvider.FixedRequiredPasswordLen)]
        [MinNonAlphanum(CodeFirstMembershipProvider.FixedRequiredNonAlphanumChars)]
        [DataType(DataType.Password)]
        [Display(Name = "New password", Description=CodeFirstMembershipProvider.PasswordDescription)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Include password in confirmation email", Description="Leave unchecked if you also use this password for other logins")]
        public bool IncludePasswordInConfirmation { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class EmailPasswordModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Work Email", Description="Your hospital email address")]
        public string Email { get; set; }

        public ProfessionalRoles ProfessionalRole { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password for your ICU", Description="On the inside cover of the DAB trial folder (at the front desk of your unit).")]
        public string SiteSpecificPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = CodeFirstMembershipProvider.FixedRequiredPasswordLen)]
        [MinNonAlphanum(CodeFirstMembershipProvider.FixedRequiredNonAlphanumChars)]
        [DataType(DataType.Password)]
        [Display(Name = "Your Password", Description = CodeFirstMembershipProvider.PasswordDescription)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Include password in confirmation email", Description = "Leave unchecked if you also use this password for other logins")]
        public bool IncludePasswordInConfirmation { get; set; }
    }
}
