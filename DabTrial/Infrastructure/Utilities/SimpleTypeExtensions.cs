using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Utilities
{
    public static class SimpleTypeExtensions
    {
        public static string GetOrdinalSuffix(int number)
        {
            switch (number % 100)
            {
                case 11:
                case 12:
                case 13:
                    return "th";
            }

            switch (number % 10)
            {
                case 1:
                    return "st";
                case 2:
                    return "nd";
                case 3:
                    return "rd";
                default:
                    return "th";
            }
        }
        public static string ToOrdinal(this int number)
        {
            return number.ToString() + GetOrdinalSuffix(number);
        }


    }
}