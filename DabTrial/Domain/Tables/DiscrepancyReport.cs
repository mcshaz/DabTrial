using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DabTrial.Domain.Tables
{
    public abstract class DiscrepancyReportBase : IDescribableEntity
    {
        public abstract int Id { get; set; }
        [ForeignKey("ReportingUser")]
        public int ReportingUserId { get; set; }
        public DateTime ReportingTimeLocal { get; set; }
        public DateTime EventTime { get; set; }
        public string Details { get; set; }

        [Required]
        public virtual TrialParticipant TrialParticipant { get; set; }
        [Required]
        public virtual User ReportingUser { get; set; }
        public abstract string Describe();
    }
    public abstract class OneTo1DiscrepancyReport : DiscrepancyReportBase
    {
        [Key, ForeignKey("TrialParticipant")]
        public override int Id { get; set; }
        
        public override string Describe()
        {
            return String.Format("ParticipantId:\"{0}\" EventTime:\"{1:d/M/yyyy HH:mm}\"", Id, EventTime);
        }
    }

    public abstract class DiscrepancyReport : DiscrepancyReportBase
    {
        [Key]
        public override int Id { get; set; }
        
        [ForeignKey("TrialParticipant")]
        public int ParticipantId { get; set; }

        public override string Describe()
        {
            return String.Format("ParticipantId:\"{0}\" EventTime:\"{1:d/M/yyyy HH:mm}\"", ParticipantId, EventTime);
        }
    }
}