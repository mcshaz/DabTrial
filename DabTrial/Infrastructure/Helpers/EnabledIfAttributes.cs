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
    public class PropertyValuePairs<TModel>
    {
        public readonly IDictionary<string, object> Pairs = new Dictionary<string, object>();
        private HtmlHelper<TModel> _html;
        public PropertyValuePairs(HtmlHelper<TModel> html)
        {
            _html=html;
        }
        public PropertyValuePairs<TModel> AddNameFor<TValue>(Expression<Func<TModel, TValue>> expression, object value)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string name = _html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);
            return AddName(name, value);
        }
        public PropertyValuePairs<TModel> AddName(string name, object value)
        {
            Pairs.Add(name, value);
            return this;
        }
        public PropertyValuePairs<TModel> AddIdFor<TValue>(Expression<Func<TModel, TValue>> expression, object value)
        {
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string id = _html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName);
            return AddName(id, value);
        }
        public PropertyValuePairs<TModel> AddId(string id, object value)
        {
            Pairs.Add('#' + id, value); 
            return this;
        }
    }
    public enum LogicalOperator { And, Or }
    public static class EnabledIfAttributes
    {
        public static PropertyValuePairs<TModel> NewPropertyValues<TModel>(this HtmlHelper<TModel> html)
        {
            return new PropertyValuePairs<TModel>(html);
        }
        //for use in javaScript - lower case property names are intentional
        public class PropertyPair
        {
            public string name { get; set; }
            public object val { get; set; }
        }
        public class dataAttribute
        {
            public string logicalOperator { get; set; }
            public List<PropertyPair> propertyPairs { get; set; }
        }
        public const string ClassName = "enabledif";
        public static string AttributeName = "data-" + ClassName;
        public static IDictionary<string, object> EnabledIfAttributesFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object value)
        {
            return EnabledIfAttributesFor(html, expression, value, new RouteValueDictionary());
        }
        public static IDictionary<string, object> EnabledIfAttributesFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object value, object htmlAttributes)
        {
            return EnabledIfAttributesFor(html, expression, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        public static IDictionary<string, object> EnabledIfAttributesFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object value, IDictionary<string, object> htmlAttributes)
        {
            var pairList = new PropertyValuePairs<TModel>(html).AddNameFor(expression, value);
            return EnabledIfAttributesFor(html, pairList, new RouteValueDictionary(htmlAttributes));
        }
        public static IDictionary<string, object> EnabledIfAttributesFor<TModel>(this HtmlHelper<TModel> html, PropertyValuePairs<TModel> values, LogicalOperator logicalOperator = LogicalOperator.Or)
        {
            return EnabledIfAttributesFor(html, values, new RouteValueDictionary(), logicalOperator);
        }
        public static IDictionary<string, object> EnabledIfAttributesFor<TModel>(this HtmlHelper<TModel> html, PropertyValuePairs<TModel> values, object htmlAttributes, LogicalOperator logicalOperator = LogicalOperator.Or)
        {
            return EnabledIfAttributesFor(html, values, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), logicalOperator);
        }
        public static IDictionary<string, object> EnabledIfAttributesFor<TModel>(this HtmlHelper<TModel> html, PropertyValuePairs<TModel> values, IDictionary<string, object> htmlAttributes, LogicalOperator logicalOperator = LogicalOperator.Or)
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

            var attr = new dataAttribute
            {
                logicalOperator = logicalOperator.ToString(),
                propertyPairs = new List<PropertyPair>()
            };

            foreach (var pair in values.Pairs)
            {
                string htmlFieldName = ExpressionHelper.GetExpressionText(pair.Key);
                attr.propertyPairs.Add(new PropertyPair
                {
                    name = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName),
                    val = pair.Value
                });
            }

            htmlAttributes.Add(AttributeName, JsonConvert.SerializeObject(attr));
            return htmlAttributes;
        }

    }
}