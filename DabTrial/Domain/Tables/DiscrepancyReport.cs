using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DabTrial.Domain.Tables
{
    public abstract class DiscrepancyReportBase : IDescribableEntity
    {
        [Key]
        public abstract int Id { get; set; }
        [ForeignKey("ReportingUser"),Required]
        public int ReportingUserId { get; set; }
        public DateTime ReportingTimeLocal { get; set; }
        public DateTime EventTime { get; set; }
        public string Details { get; set; }
        [Required]
        public virtual TrialParticipant TrialParticipant { get; set; }
        public virtual User ReportingUser { get; set; }
        public abstract string Describe();
    }
    public abstract class OneTo1DiscrepancyReport : DiscrepancyReportBase
    {
        [Key, ForeignKey("TrialParticipant"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get; set; }
        public override string Describe()
        {
            return String.Format("ParticipantId:\"{0}\" EventTime:\"{1:d/M/yyyy HH:mm}\"", Id, EventTime);
        }
    }

    public abstract class DiscrepancyReport : DiscrepancyReportBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }
        
        [ForeignKey("TrialParticipant")]
        public int ParticipantId { get; set; }

        public override string Describe()
        {
            return String.Format("ParticipantId:\"{0}\" EventTime:\"{1:d/M/yyyy HH:mm}\"", ParticipantId, EventTime);
        }
    }
}