using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DabTrial.Utilities;
using System.Web.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace DabTrial.Domain.Tables
{
    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum ProfessionalRoles
    {
        ResearchNurse,
        ResearchAdministrator,
        ClinicalNurse,
        Registrar,
        Consultant
    }
    public class User : IDescribableEntity
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(150)]
        public string Password { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [ForeignKey("StudyCentre")]
        public int? StudyCentreId { get; set; }
        public virtual StudyCentre StudyCentre { get; set; }
        public int? ProfessionalRoleId { get; set; }
        [NotMapped]
        public virtual ProfessionalRoles? ProfessionalRole {
            get { return(ProfessionalRoles?)ProfessionalRoleId;}
            set { ProfessionalRoleId = (Int32)value; }
        } 

        [DataType(DataType.MultilineText)]
        [StringLength(150)]
        public string Comment { get; set; }
        public bool IsPublicContact { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeactivated { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        [StringLength(100)]
        public string ConfirmationToken { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        [StringLength(100)]
        public string PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<TrialParticipant> EnrolledParticipants { get; set; }

        public virtual ICollection<ProtocolViolation> ReportedViolations { get; set; }
        public virtual ICollection<AdverseEvent> ReportedAdverseEvents { get; set; }

        public string Describe()
        {
            return String.Format("UserName:\"{0}\" Email:\"{1}\"", UserName , Email);
        }

        public static implicit operator MembershipUser(User thisUser)
        {

            return new MembershipUser(providerName: "CodeFirstMembershipProvider",
                name: thisUser.UserName,
                providerUserKey: thisUser.UserId,
                email: thisUser.Email,
                passwordQuestion: null,
                comment: thisUser.Comment,
                isApproved: thisUser.IsApproved,
                isLockedOut: thisUser.IsLockedOut,
                creationDate: thisUser.CreateDate.HasValue ? thisUser.CreateDate.Value : DateTime.MinValue,
                lastLoginDate: thisUser.LastLoginDate.HasValue ? thisUser.LastLoginDate.Value : DateTime.MinValue,
                lastActivityDate: thisUser.LastActivityDate.HasValue ? thisUser.LastActivityDate.Value : DateTime.MinValue,
                lastPasswordChangedDate: thisUser.LastPasswordChangedDate.HasValue ? thisUser.LastPasswordChangedDate.Value : DateTime.MinValue,
                lastLockoutDate: thisUser.LastLockoutDate.HasValue ? thisUser.LastLockoutDate.Value : DateTime.MinValue);
        }
    }
    public static class UserExtensions
    {
        public static int? RestrictedToCentre(this User usr)
        {
            if (usr.Roles.Any(r => r.RoleName == RoleExtensions.EnrollingClinician))
            {
                return usr.StudyCentreId.Value;
            }
            else if(usr.Roles.Any(r => r.RoleName == RoleExtensions.PrincipleInvestigator || r.RoleName == RoleExtensions.SiteInvestigator ))
            {
                return null;
            }
            throw new InvalidOperationException("User does not have permission to be accessing this view");
        }
        public static bool IsDbAdmin(this User usr)
        {
            return usr.Roles.Any(r => r.RoleName == RoleExtensions.DbAdminName);
        }
        public static bool IsPrincipleInvestigator(this User usr)
        {
            return usr.Roles.Any(r => r.RoleName == RoleExtensions.PrincipleInvestigator);
        }
        public static InvestigatorRole? InvestigatorRole(this User usr)
        {
            InvestigatorRole returnVal;
            if (Enum.TryParse<InvestigatorRole>(usr.InvestigatorRoleName(), true, out returnVal))
            {
                return returnVal;
            }
            return null;
        }
        public static String InvestigatorRoleName(this User usr)
        {
            return usr.Roles.FirstOrDefault(r => r.RoleName != RoleExtensions.DbAdminName).RoleName;
        }
    }
}


