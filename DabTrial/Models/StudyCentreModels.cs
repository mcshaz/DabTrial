using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using DabTrial.Infrastructure.Validation;

namespace DabTrial.Models
{
    public class StudyCentreCreate
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Hospital Name must be between 2 and 50 characters")]
        [Display(Name="Hospital Name")]
        public String Name { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 2)]
        [Display(Name = "Abbreviation for Hospital")]
        public String Abbreviation { get; set; }
        [Display(Name = "Medical Record Provider")]
        public IEnumerable<SelectListItem> RecordSystems { get; set; }
        [Required]
        public int? RecordSystemId { get; set; }
        [Required]
        [Display(Name = "Email domain list", Description = "A comma separated list of acceptable email domains")]
        [StringLength(100, MinimumLength = 4)]
        [RegularExpression(@"^(\s*@(\w+([-.]?\w+)){1,}\.\w{2,4}\s*(?!,$),?)+$", ErrorMessage = "Must be a list of valid domains beginning with @, separated by commas")]
        public String ValidDomainList { get; set; }
        [Required]
        [Display(Name = "Registration Password", Description = "The password required to register a patient in the trial")]
        [RegexCount(@"[\W\d]*dabtrial[\W\d]*", MaximumCount = 0)]
        [MinNonAlphanum(1)]
        public String SiteRegistrationPwd { get; set; }
        public PhoneNumber PublicPhoneNumber { get; set; }
        [Required]
        public String TimeZoneId { get; set; }
        [Required, Display(Name="Uses 1% adrenaline", Description="unchecked = 1 in 1000")]
        public bool IsUsing1pcAdrenaline { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [ComesAfter("2013-7-1T00:00:00", AnnotationArgumentType.DateTime, ErrorMessage = "Must be a date after commencement of the study (1/7/2013)")]
        public DateTime CommencedEnrollingOn { get; set; }
        public IEnumerable<SelectListItem> TimeZones { get; set; }
    }
    public class PhoneNumber
    {
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Int'l", Description = "Int'l Code")]
        [RegularExpression(@"\d{2}", ErrorMessage = "International code must be 2 digits")]
        public string IntlCode { get; set; }
        [Required]
        [Display(Name = "Area", Description = "Area Code")]
        [RegularExpression(@"\d{1,2}", ErrorMessage = "Area code must be 1-2 digits")]
        public string AreaCode { get; set; }
        [Required]
        [Display(Name = "Phone", Description = "Phone Number")]
        [RegularExpression(@"[0-9 ]{7,12}", ErrorMessage = "Phone number may contain only digits and spaces (max 12)")]
        public String LocalNo { get; set; }
        private const string numberFormat = "(+{0}) {1} {2}";
        private const string numberExtract = @"\(\+(?<intl>[\d]+)\) (?<area>[\d]+) (?<local>[0-9 ]+)";
        public string Formatted
        {
            get { return String.Format(numberFormat, IntlCode, AreaCode, LocalNo.Trim()); }
            set 
            {
                if (value != null)
                {
                    Regex deformat = new Regex(numberExtract);
                    var grps = deformat.Match(value).Groups;
                    IntlCode = grps["intl"].Value;
                    AreaCode = grps["area"].Value;
                    LocalNo = grps["local"].Value;
                }
            }
        }
    }
    public class StudyCentreEdit
    {
        public int StudyCentreId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Hospital Name")]
        public String Name { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 2)]
        public string Abbreviation { get; set; }
        [Display(Name="Medical Record Provider")]
        public String RecordSystemName { get; set; }
        public int RecordSystemId { get; set; }
        [Required]
        [Display(Name = "Email domain list", Description = "A comma separated list of acceptable email domains")]
        [StringLength(100, MinimumLength = 4)]
        [RegularExpression(@"^(\s*@(\w+([-.]?\w+)){1,}\.\w{2,4}\s*(?!,$),?)+$", ErrorMessage = "Must be a list of valid domains beginning with @, separated by commas")]
        public String ValidDomainList { get; set; }
        [Required]
        [Display(Name = "Registration Password", Description = "The password required to register a patient in the trial")]
        [RegexCount(@"[\W\d]*dabtrial[\W\d]*", MaximumCount = 0)]
        [MinNonAlphanum(1)]
        public String SiteRegistrationPwd { get; set; }
        public PhoneNumber PublicPhoneNumber { get; set; }
        [Required]
        public String TimeZoneId { get; set; }
        [Required, Display(Name = "Uses 1% adrenaline", Description = "unchecked = 1 in 1000")]
        public bool IsUsing1pcAdrenaline { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [ComesAfter("2013-7-1T00:00:00", AnnotationArgumentType.DateTime, ErrorMessage = "Must be a date after commencement of the study (1/7/2013)")]
        public DateTime CommencedEnrollingOn { get; set; }
        public IEnumerable<SelectListItem> TimeZones { get; set; }
    }
    public class StudyCentreDetails
    {
        public int StudyCentreId { get; set; }
        public String Name { get; set; }
        public string Abbreviation { get; set; }
        [Display(Name = "Medical Record Provider")]
        public String RecordSystemName { get; set; }
        [Display(Name = "Email domain list", Description = "A comma separated list of acceptable email domains")]
        public String ValidDomainList { get; set; }
        [Display(Name = "Registration Password", Description = "The password required to register a patient in the trial")]
        public String SiteRegistrationPwd { get; set; }
        [DataType(DataType.PhoneNumber)]
        public String PublicPhoneNumber { get; set; }
        public String TimeZoneId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime CommencedEnrollingOn { get; set; }
        public bool IsUsing1pcAdrenaline { get; set; }
    }
    public class StudyCentreListItem
    {
        public int StudyCentreId { get; set; }
        [Display(Name = "Hospital Name")]
        public String Abbreviation { get; set; }
        [Display(Name = "Acceptable Emails", Description = "A list of acceptable email domains")]
        public String ValidDomainList { get; set; }
        [Display(Name = "Password", Description = "The password required to register a patient")]
        public String SiteRegistrationPwd { get; set; }
        [Display(Name = "Medical Record Index", Description = "The specific hospital index number provider")]
        public String RecordSystemName { get; set; }
        //public int Mrnid { get; set; }
    }
    public class StudyCentreStatistic
    {
        public int StudyCentreId { get; set; }
        [Display(Name = "Hospital Name")]
        public String Abbreviation { get; set; }
        public int InterventionArmCount { get; set; }
        public int ControlArmCount { get { return TotalParticiapants - InterventionArmCount; } }
        public int TotalParticiapants { get; set; }
        public int CurrentParticipants { get; set; }
        public int AdverseEventCount { get; set; }
        public int DeathCount { get; set; }
        public int ViolationCount { get; set; }
        public int WithdrawnCount { get; set; }
        [Display(Name="Screened")]
        public int Screened { get; set; }
        [Display(Name = "Eligible")]
        public int Eligible { get; set; }
        [DisplayFormat(DataFormatString = "{0:s}")]
        public DateTime? MostRecentScreen { get; set; }
        public int TotalScreened
        {
            get
            {
                return Screened + TotalParticiapants;
            }
        }
        public int TotalEligible
        {
            get
            {
                return Eligible + TotalParticiapants;
            }
        }
        [DisplayFormat(DataFormatString="{0:0%}")]
        public double? RatioEnrollScreen
        {
            get
            {
                return (TotalEligible == 0) ? (double?)null : ((double)TotalParticiapants / TotalScreened);
            }
        }
        [DisplayFormat(DataFormatString = "{0:0%}")]
        public double? RatioEnrollEligible
        {
            get
            {
                return (TotalEligible == 0) ? (double?)null : ((double)TotalParticiapants / TotalEligible);
            }
        }
    }
    public class StudyCentreStats
    {
        public StudyCentreStats(IEnumerable<StudyCentreStatistic> statList, int? restrictTo=null)
        {
            OverallStats = new StudyCentreStatistic()
            {
                Abbreviation = "Total",
                StudyCentreId = -1,
                TotalParticiapants = statList.Sum(c => c.TotalParticiapants),
                CurrentParticipants = statList.Sum(c => c.CurrentParticipants),
                DeathCount = statList.Sum(c => c.DeathCount),
                InterventionArmCount = statList.Sum(c => c.InterventionArmCount),
                ViolationCount = statList.Sum(c => c.ViolationCount),
                AdverseEventCount = statList.Sum(c => c.AdverseEventCount),
                WithdrawnCount = statList.Sum(c => c.WithdrawnCount),
                Screened = statList.Sum(c=>c.Screened),
                Eligible = statList.Sum(c=>c.Eligible)
            };
            CentreStatList = restrictTo.HasValue?statList.Where(s=>s.StudyCentreId==restrictTo.Value):statList;
        }
        public IEnumerable<StudyCentreStatistic> CentreStatList {get; private set;}
        public StudyCentreStatistic OverallStats {get; private set;}
    }
    public class CentreDataStageMatrixModel
    {
        public IList<string> RowCentreNames { get; set; }
        public IList<string> ColDataStages { get; set; }
        public IList<IList<int>> Counts { get; set; }
    }
    public class CentreSpecificPatientValidationInfo
    {
        public string Abbreviation { get; set; }
        public string RecordSystemName { get; set; }
        public string RecordSystemHospitalNoRegEx { get; set; }
        public string RecordSystemNotationDescription { get; set; }
        [UIHint("HiddenDate")]
        public DateTime CommencedEnrollingOn { get; set; }
        public IDictionary<string, object> HospitalNoRegEx()
        {
            var returnDictionary = DynamicClientValidation.RegEx(errorMessage: String.Format("{0} must consist of {1}",
                                                                            RecordSystemName,
                                                                            RecordSystemNotationDescription),
                                                 regExPattern: RecordSystemHospitalNoRegEx);
            returnDictionary.Add("class", "upperCase");
            return returnDictionary;
        }
    }
}