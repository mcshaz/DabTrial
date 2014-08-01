using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Utilities
{
    public static class BlockRandomisation
    {
        public static bool IsNextAllocationInterventionWithCentralTendancy(IEnumerable<bool> interventionArmWithinBlock, int blockSize,Func<double> wholeTrialProportionIntervention)
        {
            double remainingAllocations = blockSize;
            double remainingInterventions = blockSize / 2;
            foreach (bool isIntervention in interventionArmWithinBlock)
            {
                remainingAllocations--;
                if (isIntervention)
                {
                    remainingInterventions--;
                }
            }
            if (remainingAllocations <= 0) { throw new ArgumentException("No remaining allocations"); }
            if (remainingAllocations == blockSize && wholeTrialProportionIntervention!=null)
            {
                return wholeTrialProportionIntervention() < 0.5;
            }
            double Pintervention = remainingInterventions / remainingAllocations;
            double rdm = new Random().NextDouble();
            return (rdm <= Pintervention);
        }
        public static bool IsNextAllocationIntervention(IEnumerable<bool> interventionArmWithinBlock, int blockSize)
        {
            return IsNextAllocationInterventionWithCentralTendancy(interventionArmWithinBlock, blockSize, null);
        }
        /*
        public static bool nextAllocation<T>(int blockSize, IEnumerable<T> patientDataCollection, Func<T,bool> predicate)
        {
            double remainingAllocations = blockSize - patientDataCollection.Count();
            if (remainingAllocations <= 0) throw new ArgumentException("patientDataCollection must be smaller than blockSize.");
            double remainingInterventions = blockSize / 2 - patientDataCollection.Count(predicate);
            double Pintervention = remainingInterventions / remainingAllocations;
            double rdm = new Random().NextDouble();
            return (rdm <= Pintervention);
        }
        
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