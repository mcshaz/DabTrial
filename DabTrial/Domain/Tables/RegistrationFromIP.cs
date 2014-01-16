using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DabTrial.Domain.Tables
{
    public class RegistrationFromIP
    {
        [Key]
        public int Id { get; set; }
        public Byte[] IpAddress { get; set; }
        public DateTime? LastAttempt { get; set; }
        public int Failures { get; set; }
        [ForeignKey("StudyCentre")]
        public int? StudyCentreId { get; set; }

        public virtual StudyCentre StudyCentre { get; set; }
    }
}