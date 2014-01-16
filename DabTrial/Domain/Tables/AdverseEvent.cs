using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DabTrial.Domain.Tables
{
    public class AdverseEvent : IDescribableEntity
    {
        [Key]
        public int AdverseEventId {get; set; }
        [ForeignKey("TrialParticipant")]
        public int ParticipantId { get; set; }
        public DateTime EventTime { get; set; }
        public int SeverityLevelId { get; set; }
        [ForeignKey("AdverseEventType")]
        public int AdverseEventTypeId { get; set; }
        public bool Sequelae { get; set; }
        [ForeignKey("ReportingUser")]
        public int ReportingUserId { get; set; }
        public DateTime ReportingTimeLocal { get; set; }
        public bool FatalEvent { get; set; }
        public string Details { get; set; } 

        public virtual AdverseEventType AdverseEventType {get;set;}
        public virtual TrialParticipant TrialParticipant { get; set; }
        public virtual User ReportingUser { get; set; }
        public virtual ICollection<Drug> Drugs { get; set; }
        public Severity Severity
        {
            get
            {
                return Severity.Levels.FirstOrDefault(s => s.SeverityId == this.SeverityLevelId);
            }
        }

        public string Describe()
        {
            return String.Format("ParticipantId:\"{0}\" EventTime:\"{1:d/M/yyyy HH:mm}\"", ParticipantId, EventTime); 
        }
    }
    //entity framework does not support nested classes as yet - could be nested in above class
    public class Drug : IDescribableEntity
    {
        [Key]
        public int DrugId { get; set; }
        [ForeignKey("AdverseEvent")]
        public int AdverseEventId { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string DrugName { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string Dosage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? StopDate { get; set; }
        public string ReasonsForUse { get; set; }

        public virtual AdverseEvent AdverseEvent { get; set; }

        public string Describe()
        {
            return String.Format("AdverseEventId:\"{0}\" DrugName:\"{1}\"", AdverseEventId, DrugName); 
        }
    }
    public class Severity
    {
        public Int32 SeverityId { get; set; }
        public String Description { get; set; }
        public String Definition { get; set; }
        public Severity(int severityId, String description, String definition)
        {
            this.SeverityId = severityId;
            this.Description = description;
            this.Definition = definition;
        }
        private const string nl = "<br/>";
        public static Severity[] Levels = {
            new Severity(1, "Reportable Serious Adverse Event (SAE)", String.Format("Results in death{0}Is life threatening (actual risk of death from event: NOT might have caused death if more severe){0}Results in persistent or significant disability/incapacity",nl)),
            new Severity(2, "Severe Adverse Event", "Requires prolongation of existing hospitalisation"),
            new Severity(3, "Moderate Adverse Event", "Event requiring medical management"),
            new Severity(4, "Minor Adverse Event", "Self resolving event lasting less than 8 hours")
        };
    }
    public class AdverseEventType :IDescribableEntity
    {
        [Key]
        public int AdverseEventTypeId { get; set; }
        [StringLength(60)]
        public string Description { get; set; }

        public string Describe()
        {
            return String.Format("Description:\"{0}\"", Description);
        }
    }

}