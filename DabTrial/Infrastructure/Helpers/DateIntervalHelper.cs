using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Infrastructure.Helpers
{
    public static class DateIntervalHelper
    {
        public static string GetIntervalPrior(DateTime d, DateTime? today=null)
        {
            DateTime todayVal = today ?? DateTime.Today;
            var ival = new HumanDateInterval(d, todayVal);
            if (ival.WholeYears>2)
            {
                return ival.WholeYears + " years";
            }
            if (ival.WholeMonths<2)
            {
                var days = (todayVal - d).Days;
                if (days<14) {return days + " days";}
                return days/7 + " weeks";
            }
            return ival.WholeMonths + " months";
        }
    }
    public class HumanDateInterval
    {
        public HumanDateInterval(DateTime start, DateTime end)
        {
            WholeYears = end.Year - start.Year;
            if (end.Month >= start.Month)
            {
                WholeMonths = end.Month - start.Month;
            }
            else
            {
                WholeMonths = end.Month + 12 - start.Month;
                --WholeYears;
            }
            if (end.Day >= start.Day) {
                WholeDays = end.Day-start.Day;
            }
            else
            {
                WholeDays = end.Day + DateTime.DaysInMonth(end.Year, end.Month == 1 ? 12 : end.Month - 1) - start.Day;
                --WholeMonths;
                if (WholeMonths < 0)
                {
                    --WholeYears;
                    WholeMonths = 12;
                }
            }
            
        }

        public int WholeYears { get; private set; }
        public int WholeMonths { get; private set; }
        public int WholeDays { get; private set; }
    }
}