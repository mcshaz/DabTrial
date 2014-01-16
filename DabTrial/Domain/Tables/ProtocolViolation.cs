using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DabTrial.Domain.Tables
{
    public class ProtocolViolation : IDescribableEntity
    {
        [Key]
        public int ViolationId { get; set; }
        [ForeignKey("TrialParticipant")]
        public int ParticipantId { get; set; }
        public DateTime TimeOfViolation { get; set; }
        public string Details { get; set; }
        public bool MajorViolation { get; set; }
        [ForeignKey("ReportingUser")]
        public int ReportingUserId { get; set; }
        public DateTime ReportingTimeLocal { get; set; }

        public virtual TrialParticipant TrialParticipant { get; set; }
        public virtual User ReportingUser { get; set; }
        public string Describe()
        {
            return String.Format("ParticipantId:\"{0}\" TimeOfViolation:\"{1:d/M/yyyy HH:mm}\"", ParticipantId, TimeOfViolation);
        }
    }
}