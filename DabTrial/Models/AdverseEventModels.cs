using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DabTrial.Infrastructure.Validation;
using System.Web.Mvc;
using MvcHtmlHelpers;
using DabTrial.Domain.Tables;

namespace DabTrial.Models
{
    public class AdverseEventCreateModel: IAdverseEventModel
    {
        [HiddenInput(DisplayValue=true)]
        [Display(Name = "DAB Trial Id")]
        public Int32 ParticipantId { get; set; }
        [UIHint("HiddenDate")]
        [DisplayFormat(DataFormatString = "{0:s}", ApplyFormatInEditMode=true)]
        public DateTime TrialParticipantLocalTimeRandomised { get; set; }
        [Display(Name="Hospital Id")]
        public string TrialParticipantHospitalId { get; set; }
        [DataType(DataType.DateTime)]
        [Required]
        [ComesAfter("TrialParticipantLocalTimeRandomised", AnnotationArgumentType.PropertyName)]
        [ComesBeforeNowAtClient]
        [DisplayFormat(DataFormatString="{0:dd/M/yyyy HH:mm}", ApplyFormatInEditMode=true)]
        public DateTime? EventTime { get; set; }
        public Boolean FatalEvent { get; set; }
        [Required]
        public Int32? SeverityLevelId { get; set; }
        [Required]
        public Int32? AdverseEventTypeId { get; set; }
        public Boolean Sequelae { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public String Details { get; set; }

        [Display(Name="Severity Classification", Description="Select to see formal definition")]
        public IEnumerable<DetailSelectListItem> SeverityLevels { get; set; }

        [Display(Name = "Event Classification", Description = "Category of adverse event")]
        public IEnumerable<SelectListItem> EventTypes { get; set; }
    }
    public class AdverseEventEditModel: IAdverseEventModel
    {
        [HiddenInput(DisplayValue = false)]
        public Int32 AdverseEventId { get; set; }
        [Display(Name = "DAB Trial Id")]
        public Int32 ParticipantId { get; set; }
        [UIHint("HiddenDate")]
        [DisplayFormat(DataFormatString = "{0:s}", ApplyFormatInEditMode = true)]
        public DateTime TrialParticipantLocalTimeRandomised { get; set; }
        [Display(Name = "Hospital Id")]
        public string TrialParticipantHospitalId { get; set; }
        [DataType(DataType.DateTime)]
        [Required]
        [ComesAfter("TrialParticipantLocalTimeRandomised", AnnotationArgumentType.PropertyName)]
        [ComesBeforeNowAtClient]
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? EventTime { get; set; }
        public Boolean FatalEvent { get; set; }
        [Required]
        public Int32? SeverityLevelId { get; set; }
        [Required]
        public Int32? AdverseEventTypeId { get; set; }
        public Boolean Sequelae { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public String Details { get; set; }

        [Display(Name = "Severity Classification", Description = "Select to see formal definition")]
        public IEnumerable<DetailSelectListItem> SeverityLevels { get; set; }

        [Display(Name = "Event Classification", Description = "Category of adverse event")]
        public IEnumerable<SelectListItem> EventTypes { get; set; }
    }
    public class AdverseEventDetails
    {
        public Int32 AdverseEventId { get; set; }
        [Display(Name = "DAB trial Id", Description = "Unique identifier for trial")]
        public string TrialParticipantParticipantId { get; set; }
        public string ReportingUserFullName { get; set; }
        public string ReportingUserEmail { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}")]
        public DateTime ReportingTimeLocal { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}")]
        public DateTime EventTime { get; set; }
        public Boolean FatalEvent { get; set; }
        public String SeverityDescription { get; set; }
        public string AdverseEventTypeDescription { get; set; }
        public Boolean Sequelae { get; set; }
        public String Details { get; set; }

        public IEnumerable<DrugListItem> Drugs { get; set; }
    }
    public class AdverseEventListItem
    {
        public string ParticipantId { get; set; }
        public string TrialParticipantStudyCentreAbbreviation { get; set; }
        [HiddenInput(DisplayValue = false)]
        public Int32 AdverseEventId { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}")]
        public DateTime EventTime { get; set; }
        public Boolean FatalEvent { get; set; }
        public Boolean Sequelae { get; set; }
        public String SeverityDescription { get; set; }
        public string AdverseEventTypeDescription { get; set; }
        public String Details { get; set;} 

        public IEnumerable<DrugListItem> Drugs { get; set; }
    }
    public class DrugCreateModify
    {
        private Int32 _drugId = -1;
        public Int32 DrugId { get { return _drugId; } set { _drugId = value; } }
        public Int32 AdverseEventId {get; set;}
        [StringLength(50, MinimumLength = 2)]
        [Required]
        public String DrugName { get; set; }
        [StringLength(50, MinimumLength = 2)]
        [Required]
        public String Dosage { get; set; }
        public string AdverseEventTrialParticipantDob { get; private set; }
        public AdverseEvent AdverseEvent
        {
            set
            {
                AdverseEventId = value.AdverseEventId;
                AdverseEventTrialParticipantDob = value.TrialParticipant.Dob.ToString("s");
            }
        }
        [Required]
        [DataType(DataType.DateTime)]
        [ComesBeforeNowAtClient]
        [ComesAfter("AdverseEventTrialParticipantDob", ErrorMessage = "must come after the patients date of birth")]
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.DateTime)]
        [ComesAfter("StartDate")]
        [ComesBeforeNowAtClient]
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StopDate { get; set; }
        [DataType(DataType.MultilineText)]
        public String ReasonsForUse { get; set; }
    }
    public class DrugListItem
    {
        public Int32 DrugId { get; set; }
        public String DrugName { get; set; }
        public String Dosage { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}")]
        public DateTime? StopDate { get; set; }
        public String ReasonsForUse { get; set; }
    }
    public class ConfirmDeleteDrugModel
    {
        public Int32 AdverseEventId { get; set; }
        public Int32 DrugId { get; set; }
        public String DrugName { get; set; }
        public String Dosage { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/M/yyyy HH:mm}")]
        public DateTime? StopDate { get; set; }
        public String ReasonsForUse { get; set; }
    }
    public interface IAdverseEventModel
    {
        Int32? SeverityLevelId { get; set; }
        IEnumerable<DetailSelectListItem> SeverityLevels { get; set; }

        Int32? AdverseEventTypeId { get; set; }
        IEnumerable<SelectListItem> EventTypes { get; set; }
    }
}