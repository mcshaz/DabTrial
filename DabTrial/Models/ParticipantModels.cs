using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Foolproof;
using MvcHtmlHelpers;
using System.ComponentModel.DataAnnotations.Schema;
using DabTrial.Infrastructure.Validation;
using DabTrial.Infrastructure.Utilities;


namespace DabTrial.Models
{
    public class ParticipantListItem
    {
        public Int32 ParticipantId { get; set; }
        public string StudyCentreAbbreviation { get; set; }
        public String HospitalId { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth", Description = "day/month/year")]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Dob { get; set; }
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        [Display(Name = "ICU admission")]
        [DataType(DataType.DateTime)]
        public DateTime IcuAdmission { get; set; }
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        public  DateTime? ActualIcuDischarge { get; set; }
        public String TrialArm { get; set; }
        public String EnrollingClinicianFullName { get; set; }

        public DabTrial.Infrastructure.Utilities.DataStageUtilities.TrialStage DataStage { get; set; }
    }
    public class ParticipantDetails
    {
        public int ParticipantId { get; set; }
        [Display(Name = "Hospital Id", Description = "Medical record number or health index used by your institution")]
        public string HospitalId { get; set; }
        [Display(Name = "Date of Birth", Description = "day/month/year")]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Dob { get; set; }
        public double Weight { get; set; }
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        [Display(Name = "Date of ICU admission", Description = "Date and time of admission to ICU involved in study")]
        [DataType(DataType.DateTime)]
        public DateTime IcuAdmission { get; set; }
        public String Gender { get; set; }
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        public DateTime LocalTimeRandomised { get; set; }
        public String TrialArm { get; set; }
        public string EnrollingClinicianFullName { get; set; }
        public String RespiratorySupportAtRandomisationDescription { get; set; }
        public string StudyCentreName { get; set; }
        public int StudyCentreId { get; set; }
        public bool IsInterventionArm { get; set; }

        public DosingModel DrugDoses { get; set; }

        private IEnumerable<RespSupportChangeItem> _respiratorySupportChanges { get; set; }
        public IEnumerable<RespSupportChangeItem> RespiratorySupportChanges
        { 
            get { return _respiratorySupportChanges; }
            set { _respiratorySupportChanges = value.OrderBy(r => r.ChangeTime); }
        }
    }

    public class MissingParticipantDataModel
    {
        [Display(Name = "Study Number")]
        public int ParticipantId { get; set; }
        public DataStageUtilities.TrialStage Stage { get; set; }
        [Display(Name="Centre")]
        public String StudyCentreAbbreviation { get; set; }
        public int StudyCentreId { get; set; }
        public int DaysSinceEnrolled { get; set; }
        public bool ActualDischarge {get;set;}
        public bool ReadyForDischarge {get; set;}
        public bool AdrenalineForStridor {get;set;}
        public bool SteroidsForStridor {get;set;}
        public bool HMPV {get;set;}
        public bool RSV {get;set;}
        public MissingInterventionDetails InterventionDetails {get;set;}
        public bool HospDischarge {get;set;}
        public string LastLoggedSupport { get; set; }
        public int DaysSinceRespSupportLogged { get; set; }

        public class MissingInterventionDetails
        {
            public bool InitialSteroidRoute { get; set; }
            public bool NumberOfSteroidDoses { get; set; }
            public bool FirstAdrenalineNeb { get; set; }
            public bool FifthAdrenalineNebAt { get; set; }
            public bool NumberOfAdrenalineNebulisers { get; set; }
        }
    }

    public class ParticipantUpdate
    {
        [UIHint("HiddenDate")]
        public DateTime MostRecentLoggedEvent { get; set; }
        [UIHint("HiddenDate")]
        public DateTime LocalTimeRandomised { get; set; }

        [HiddenInput(DisplayValue = false)]
        public Int32 ParticipantId { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [ComesBefore("ActualIcuDischarge", AnnotationArgumentType.PropertyName)]
        [Display(Name = "Ready for ICU Discharge", Description = "Date and 24 hour time")]
        [ComesBeforeNowAtClient]
        [RequiredIfNotEmpty("ActualIcuDischarge")]
        public DateTime? ReadyForIcuDischarge { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [ComesAfter("MostRecentLoggedEvent", AnnotationArgumentType.PropertyName)]
        [Display(Name="Actually left ICU", Description="Date and 24 hour time")]
        [ComesBeforeNowAtClient]
        [RequiredIfNotEmpty("HospitalDischarge")]
        public DateTime? ActualIcuDischarge { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [ComesAfter("MostRecentLoggedEvent", AnnotationArgumentType.PropertyName)]
        [Display(Name = "Hospital Discharge", Description = "Date and 24 hour time")]
        [ComesBeforeNowAtClient]
        public DateTime? HospitalDischarge { get; set; }

        [Display(Name = "RSV positive", Description="RSV detected in nasopharyngeal aspirate")]
        public bool? IsRsvPositive { get; set; }

        [Display(Name = "HMPV positive", Description = "Human Meta-Pneumo Virus detected in nasopharyngeal aspirate")]
        public bool? IsHmpvPositive { get; set; }

        //[RequiredIfNotEmpty("ActualIcuDischarge")]
        [Display(Name = "Post-extubation Steroids", Description = "Steroids given for stridor post-extubation")]
        public bool? SteroidsForPostExtubationStridor { get; set; }

        //[RequiredIfNotEmpty("ActualIcuDischarge")]
        [Display(Name = "Post-extubation Adrenaline", Description = "Adrenaline given for stridor post-extubation")]
        public bool? AdrenalineForPostExtubationStridor { get; set; }

        public bool Died { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [ComesAfter("MostRecentLoggedEvent", AnnotationArgumentType.PropertyName)]
        [RequiredIfTrue("Died")]
        [ComesBeforeNowAtClient]
        public DateTime? DeathEventTime { get; set; }

        [RequiredIfTrue("Died")]
        [DataType(DataType.MultilineText)]
        public String DeathDetails { get; set; }

        public bool WithdrawnFromStudy { get; set; }

        [RequiredIfTrue("WithdrawnFromStudy")]
        [Display(Name = "Consent for further data", Description = "Consent given for ongoing data collection")]
        public bool? WithdrawalOngoingDataOk { get; set; }

        [RequiredIfTrue("WithdrawnFromStudy")]
        [ComesBeforeNowAtClient]
        [ComesAfter("MostRecentLoggedEvent", AnnotationArgumentType.PropertyName)]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? WithdrawalEventTime { get; set; }

        [RequiredIfTrue("WithdrawnFromStudy")]
        [DataType(DataType.MultilineText)]
        public String WithdrawalDetails { get; set; }
    }

    public class ParticipantInterventionUpdate : ParticipantUpdate
    {
        [Display(Name="Initial Steroid Route")]
        public int? InitialSteroidRouteId { get; set; }
        public IEnumerable<SelectListItem> InitialSteroidRoutes { get; set; }

        [Range(0, 14)]
        [RequiredIfNotEmpty("HospitalDischarge")]
        [Display(Name = "Total Per-Protocol Steroid Doses", Description = "Number of study protocol doses of dexamethasone, methylpred and pred")]
        public int? NumberOfSteroidDoses { get; set; }

        [Range(0, 100)]
        [Display(Name = "Number of Adrenaline Nebs", Description = "Total number of adrenaline nebulisers administered")]
        [RequiredIfNotEmpty("HospitalDischarge")]
        public int? NumberOfAdrenalineNebulisers { get; set; }

        [ComesAfter("LocalTimeRandomised", AnnotationArgumentType.PropertyName)]
        [ComesBefore("ActualIcuDischarge")]
        [Display(Name = "1st Adrenaline Neb", Description = "Time 1st adrenaline nebuliser administered")]
        [RequiredIfNotEmpty("HospitalDischarge")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FirstAdrenalineNebAt { get; set; }

        [ComesAfter("LocalTimeRandomised", AnnotationArgumentType.PropertyName)]
        [ComesBefore("ActualIcuDischarge")]
        [Display(Name = "5th Adrenaline Neb", Description = "Time 5th adrenaline nebuliser administered")]
        [RequiredIfNotEmpty("HospitalDischarge")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? FifthAdrenalineNebAt { get; set; }
    }

    public class ParticipantRegistration
    {
        [Display(Name = "Hospital Id", Description = "Medical record number or health index used by your institution")]
        [Required]
        public String HospitalId { get; set; }

        [ForeignKey("StudyCentre")]
        [Display(Name = "Study Site")]
        public Int32? StudyCentreId { get; set; }

        [Required]
        [Display(Name = "Date of Birth", Description = "Equates to a corrected age 0-18 mth")]
        [ComesBeforeNowAtClient]
        [CGArange(37, 118, "WeeksGestationAtBirth", ErrorMessage="Gestation corrected age must be between 37 and 118 corrected weeks of age (approximately 1 and 18 months)")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString="{0:d/M/yyyy}", ApplyFormatInEditMode=true)]
        public DateTime? Dob { get; set; }

        [Required]
        [Wt4Age(7, "WeeksGestationAtBirth","Dob","Gender",Genders.Male, Zwarn=2.5)]
        [RegularExpression(@"[0-2]?\d\.\d+", ErrorMessage="Must be a number to at least 1 decimal point (eg 11.0).")]
        [Display(Name="Weight", Description="In Kilograms, to 1 decimal point")]
        public string WeightStr { get; set; }

        [Required]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Time of ICU admission", Description = "Date and time of admission to your ICU")]
        [ComesBeforeNowAtClient('m', 0, 240, ErrorMessage = "Patient must be Enroled within 4 hours of admission")]
        public DateTime? IcuAdmission { get; set; }

        [Required(ErrorMessage="A valid form of respiratory support must be selected.")]
        [Display(Name = "Respiratory support", Description = "Current level of support")]
        public int? RespSupportTypeId { get; set; }

        public IEnumerable<DetailSelectListItem> RespiratorySupportAtRandomisation { get; set; }

        [Required]
        [Display(Description="O2 Therapy > 14 days in 6 months")]
        public bool? ChronicLungDisease { get; set; }

        [Required]
        public bool? CyanoticHeartDisease { get; set; }

        [Required]
        [Range(23, 42)]
        [Display(Name="Gestation at Birth", Description="In completed weeks")]
        public int? WeeksGestationAtBirth { get; set; }

        [Display(Name = "No Immunosuppressives", Description = "Including any corticosteroids in the last 7 days")]
        [MustBeTrue(ErrorMessage = "This MUST be checked prior to randomisation")]
        public bool NoImmunosuppressives { get; set; }

        [Display(Name = "Consented", Description = "A valid, signed consent has been obtained")]
        [MustBeTrue(ErrorMessage="This MUST be checked prior to randomisation")]
        public bool ValidConsent { get; set; }

        [Display(Name = "No Croup", Description = "No evidence of acute laryngotracheitis")]
        [MustBeTrue(ErrorMessage = "This MUST be checked prior to randomisation")]
        public bool NotCroup { get; set; }

        public enum Genders { Male, Female }
        public bool? MaleGender;
        [Required]
        public Genders? Gender
        {
            get
            {
                switch (MaleGender)
                {
                    case true: return Genders.Male;
                    case false: return Genders.Female;
                    default: return null;
                }
            }
            set {
                if (!value.HasValue) { throw new Exception("Gender must have a value of Male or Female"); }
                MaleGender = (value == Genders.Male); 
            }
        }
        public CentreSpecificPatientValidationInfo ViewData { get; set; }
    }
}