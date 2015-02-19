using DabTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace DabTrial.Utilities
{
    public static class BlockRandomisation
    {
        public static bool IsNextAllocationInterventionWithCentralTendancy(IEnumerable<bool> interventionArmWithinBlock, int blockSize,IRandom randomGenerator,Func<double> newBlockProbIntervention=null)
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
            double pIntervention;
            if (remainingAllocations == blockSize)
            {
                if (newBlockProbIntervention == null)
                {
                    pIntervention = 0.5;
                }
                else
                {
                    pIntervention = newBlockProbIntervention();
                }
            }
            else
            {
                pIntervention = remainingInterventions / remainingAllocations;
            }
            
            return (randomGenerator.NextDouble() <= pIntervention);
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