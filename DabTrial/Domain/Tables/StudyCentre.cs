using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace DabTrial.Domain.Tables
{
    public class StudyCentre : IDescribableEntity
    {
        [Key]
        [HiddenInput(DisplayValue=false)]
        public int StudyCentreId { get; set; }
        [Required]
        [StringLength(50, MinimumLength=2, ErrorMessage="Hospital Name must be between 2 and 50 characters")]
        public string Name { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 2)]
        public string Abbreviation { get; set; }
        [Required]
        [Display(Name = "Email Domains", Description = "A comma seperated list of valid email domains")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The entire list of email domains must be between 2 and 100 characters")]
        [RegularExpression(@"^(@(\w+([-.]?\w+)){1,}\.\w{2,4}(?!,$),?)+$", ErrorMessage = "Must be a list of valid domains beginning with @, separated by commas")]
        public string ValidEmailDomains { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 2)]
        [Display(Name = "Registration Password", Description = "The password required to register a patient in the trial")]
        public string SiteRegistrationPwd { get; set; }
        public string PublicPhoneNumber { get; set; }
        [ForeignKey("RecordSystem")]
        public int RecordSystemProviderId { get; set; }
        [Required]
        public string TimeZoneId { get; set; }
        public bool IsUsing1pcAdrenaline { get; set; }
        public DateTime CommencedEnrollingOn { get; set; }

        public virtual LocalRecordProvider RecordSystem {get; set;}
        public virtual ICollection<TrialParticipant> TrialParticipants {get; set;}
        public virtual ICollection<ScreenedPatient> ScreenedPatients { get; set; }
        public virtual ICollection<User> Investigators { get; set; }
        public virtual ICollection<RegistrationFromIP> RegistrationsFromIP { get; set; }
        public string Describe()
        {
            return "Abbreviation:\"" + Abbreviation + "\"";
        }
    }

    public static class StudyCentreExtensions
    {
        public static DateTime LocalTime(this StudyCentre centre)
        {
            TimeZoneInfo centreZone = TimeZoneInfo.FindSystemTimeZoneById(centre.TimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centreZone);
        }
        public const string localEmailRegEx=@"\w+([-+.]*[\w-]+)*";
        public static string domainRegEx(this StudyCentre centre)
        {
            return centre.ValidEmailDomains
                .Replace(".",@"\.")
                .Split(',')
                .Aggregate((current, next) => localEmailRegEx + current + "|" + localEmailRegEx + next);
        }
    }
}
