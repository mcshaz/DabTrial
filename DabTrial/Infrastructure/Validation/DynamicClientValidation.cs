using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Infrastructure.Validation
{
    public static class DynamicClientValidation
    {
        public static IDictionary<string, object> RegEx(string errorMessage, string regExPattern)
        {
            var returnVal = new Dictionary<string, object>(3);
            returnVal.Add("data-val-regex", errorMessage);
            returnVal.Add("data-val-regex-pattern", regExPattern);
            returnVal.Add("data-val", "true");
            return returnVal;
        }
    }
}