using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DabTrial.Utilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DabTrial.Domain.Tables
{
    public class Role :IDescribableEntity
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }
        [StringLength(100)]
        public string Description {get; set; }
        public int? Rank {get; set;}

        public virtual ICollection<User> Users { get; set; }

        public string Describe()
        {
            return "RoleName:\"" + RoleName + "\"";
        }
    }
    public enum InvestigatorRole
    {
        PrincipleInvestigator,
        SiteInvestigator,
        EnrollingClinician
    }
    //doesn't work for selectlist constructor
    public static class RoleExtensions
    {
        public const string PrincipleInvestigator = "PrincipleInvestigator";
        public const string EnrollingClinician = "EnrollingClinician";
        public const string SiteInvestigator = "SiteInvestigator";
        public const string DbAdministrator = "DatabaseAdmin";
        public static string GetRoleDescription(this Role role)
        {
            if (String.IsNullOrEmpty(role.Description))
            {
                return role.RoleName.ToSeparateWords();
            }
            else
            {
                return role.Description;
            }
        }
        public static readonly string[] InvestigatorRoleNames = new string[] { PrincipleInvestigator, SiteInvestigator };
    }
}