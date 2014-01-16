using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Utilities
{
    public static class BlockRandomisation
    {
        const double third = 1/3;
        const double twothird = 2/3;
        public static int BlockSize()
        {
            double rdm = new Random().NextDouble();
            if (rdm < third) return 4;
            if (rdm < twothird) return 6;
            return 8;
        }
        public static bool nextAllocation<T>(int blockSize, IEnumerable<T> patientDataCollection, Func<T,bool> predicate)
        {
            double remainingAllocations = blockSize - patientDataCollection.Count();
            if (remainingAllocations <= 0) throw new ArgumentException("patientDataCollection must be smaller than blockSize.");
            double remainingInterventions = blockSize / 2 - patientDataCollection.Count(predicate);
            double Pintervention = remainingInterventions / remainingAllocations;
            double rdm = new Random().NextDouble();
            return (rdm <= Pintervention);
        }
        /*
         * old (working) version.
        public static bool nextAllocation<T>(int blockSize, IEnumerable<T> patientDataCollection, string allocationPropertyName)
        {
            double remainingAllocations = blockSize - patientDataCollection.Count();
            if (remainingAllocations <= 0) throw new ArgumentException("patientDataCollection must be smaller than blockSize.");
            var p = typeof(T).GetProperty(allocationPropertyName);
            if (p.PropertyType != typeof(System.Boolean)) throw new ArgumentException("allocationPropertyName must refer to a boolean property.");
            double remainingInterventions = blockSize / 2 - patientDataCollection.Count(c => (bool)p.GetValue(c, null));
            double Pintervention = remainingInterventions / remainingAllocations;
            double rdm = new Random().NextDouble();
            return (rdm <= Pintervention);
        }
        */
    }
}