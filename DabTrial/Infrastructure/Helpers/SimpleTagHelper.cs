using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
namespace MvcHtmlHelpers
{
    public static class SimpleTagHelper
    {
        public static MvcHtmlString Element(string tagName, IDictionary<string, object> attributes, TagRenderMode renderMode=TagRenderMode.Normal)
        {
            var tag = new TagBuilder(tagName);
            tag.MergeAttributes(attributes);
            return MvcHtmlString.Create(tag.ToString(renderMode));
        }
        public static MvcHtmlString ToAttributes(this IDictionary<string, object> attributeDictionary)
        {
            return MvcHtmlString.Create(string.Join(" ",attributeDictionary.Select(d=>string.Format("{0}=\"{1}\"",d.Key,HttpUtility.HtmlAttributeEncode(d.Value.ToString())))));
        }
    }
}