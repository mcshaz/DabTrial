using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace DabTrial.Utilities
{
    public class EnumUtilities
    {
        public static string ToString<T>(object value)
        {
            return Enum.GetName(typeof(T), value);
        }
        public static IEnumerable<string> AndBelowToString<T>(object value)
        {
            string[] items = Enum.GetNames(typeof(T));
            string valName = Enum.GetName(typeof(T), value);
            return items.TakeWhile(t => t != valName);
        }
    }
    //http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx
    public class PascalCaseWordSplittingEnumConverter : EnumConverter
    {
        public PascalCaseWordSplittingEnumConverter(Type type) : base(type) { }
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                string stringValue = (string)base.ConvertTo(context, culture, value, destinationType);
                return stringValue.ToSeparatedWords();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}