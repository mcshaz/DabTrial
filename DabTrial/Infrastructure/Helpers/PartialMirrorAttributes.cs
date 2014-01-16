using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq.Expressions;
//my additions
using System.Web.Routing;
using Newtonsoft.Json;
using System.Collections;

namespace MvcHtmlHelpers
{
    public static class PartialMirrorAttributes
    {
        public const string ClassName = "partialmirror";
        public static string AttributeName = "data-" + ClassName;
        public static IDictionary<string, object> PartialMirrorAttributesFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string regex, bool alwaysMirror = false)
        {
            return PartialMirrorAttributesFor(html, expression, regex, alwaysMirror, new RouteValueDictionary());
        }
        public static IDictionary<string, object> PartialMirrorAttributesFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string regex, bool alwaysMirror,  object htmlAttributes)
        {
            return PartialMirrorAttributesFor(html, expression, regex, alwaysMirror, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static IDictionary<string, object> PartialMirrorAttributesFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string regex, bool alwaysMirror, object value, IDictionary<string, object> htmlAttributes)
        {
            return PartialMirrorAttributesFor(html, expression, regex, alwaysMirror, new RouteValueDictionary(htmlAttributes));
        }
        public static IDictionary<string, object> PartialMirrorAttributesFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string regex, bool alwaysMirror,IDictionary<string, object> htmlAttributes, LogicalOperator logicalOperator = LogicalOperator.Or)
        {
            //in this case expression refers to other property 
            if (htmlAttributes.ContainsKey("class"))
            {
                string existingClass = htmlAttributes["class"].ToString().Trim();
                htmlAttributes["class"] = existingClass + ' ' + ClassName; 
            }
            else
            {
                htmlAttributes.Add("class",ClassName);
            }
            var expTxt = ExpressionHelper.GetExpressionText(expression);
            var name = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expTxt).Replace('.','_');
            htmlAttributes.Add(AttributeName + "-watch", name);
            htmlAttributes.Add(AttributeName + "-regex", regex);
            htmlAttributes.Add(AttributeName + "-always", alwaysMirror.ToString().ToLower());
            return htmlAttributes;
        }

    }
}