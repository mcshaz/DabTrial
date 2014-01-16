using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Domain.Tables
{
    public interface IDescribableEntity
    {
        // Override this method to provide a description of the entity for audit purposes
        string Describe();
    }
}
