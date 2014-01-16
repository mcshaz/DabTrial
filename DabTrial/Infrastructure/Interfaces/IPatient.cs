using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Infrastructure.Interfaces
{
    public interface IPatient
    {
        string HospitalId { get; set; }
        int StudyCentreId { get; set; }
    }
}