using System;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
//my additions
using System.Reflection;

namespace MvcHtmlHelpers
{
    public partial class HtmlExtensions
    {
        //http://stackoverflow.com/questions/10381939/custom-html-helper-that-can-browse-dataannotations#10382089
        private static string GetPropertyNameFromExpression<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Not a memberExpression");

            if (!(memberExpression.Member is PropertyInfo))
                throw new InvalidOperationException("Not a property");

            return memberExpression.Member.Name;
        }
    }
    //http://stackoverflow.com/questions/3800473/how-to-concisely-create-optional-html-attributes-with-razor-view-engine#4232630
    public class HtmlAttribute : IHtmlString
    {
        private string _InternalValue = String.Empty;
        private string _Seperator;

        public string Name { get; set; }
        public string Value { get; set; }
        public bool Condition { get; set; }

        public HtmlAttribute(string name)
            : this(name, null)
        {
        }

        public HtmlAttribute(string name, string seperator)
        {
            Name = name;
            _Seperator = seperator ?? " ";
        }

        public HtmlAttribute Add(string value)
        {
            return Add(value, true);
        }

        public HtmlAttribute Add(string value, bool condition)
        {
            if (!String.IsNullOrWhiteSpace(value) && condition)
                _InternalValue += value + _Seperator;

            return this;
        }

        public string ToHtmlString()
        {
            if (!String.IsNullOrWhiteSpace(_InternalValue))
                _InternalValue = String.Format("{0}=\"{1}\"", Name, _InternalValue.Substring(0, _InternalValue.Length - _Seperator.Length));
            return _InternalValue;
        }
    }
    public static class Extensions
    {
        public static HtmlAttribute Css(this HtmlHelper html, string value)
        {
            return Css(html, value, true);
        }

        public static HtmlAttribute Css(this HtmlHelper html, string value, bool condition)
        {
            return Css(html, null, value, condition);
        }

        public static HtmlAttribute Css(this HtmlHelper html, string seperator, string value, bool condition)
        {
            return new HtmlAttribute("class", seperator).Add(value, condition);
        }

        public static HtmlAttribute Attr(this HtmlHelper html, string name, string value)
        {
            return Attr(html, name, value, true);
        }

        public static HtmlAttribute Attr(this HtmlHelper html, string name, string value, bool condition)
        {
            return Attr(html, name, null, value, condition);
        }

        public static HtmlAttribute Attr(this HtmlHelper html, string name, string seperator, string value, bool condition)
        {
            return new HtmlAttribute(name, seperator).Add(value, condition);
        }

    }
}