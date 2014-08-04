using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DabTrial.Domain.Tables
{
    public class ProtocolViolation : DiscrepancyReport
    {
        public bool MajorViolation { get; set; }
    }
}