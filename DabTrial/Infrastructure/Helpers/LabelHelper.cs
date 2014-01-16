using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
//my additions
using System.Web.Routing;

namespace MvcHtmlHelpers
{
    public static partial class LabelExtensions
    {
        public static MvcHtmlString LabelDetailsFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return LabelDetailsFor(html, expression, new RouteValueDictionary());
        }
        public static MvcHtmlString LabelDetailsFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return LabelDetailsFor(html, expression, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString LabelDetailsFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            string labelDescription = metadata.Description;
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder label = new TagBuilder("label");
            if (htmlAttributes.Any()) label.MergeAttributes(htmlAttributes);
            label.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            if (String.IsNullOrEmpty(labelDescription))
            {
                label.InnerHtml = labelText;
                return MvcHtmlString.Create(label.ToString(TagRenderMode.Normal));
            }
            TagBuilder span = new TagBuilder("span");
            span.AddCssClass("label-details");
            span.SetInnerText(labelDescription);

            // assign <span> to <label> inner html
            label.InnerHtml = labelText + span.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(label.ToString(TagRenderMode.Normal));
        }
        public static MvcHtmlString LabelDetailsFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, String labelText, String labelDescription)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            if (String.IsNullOrEmpty(labelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder labelTag = new TagBuilder("label");
            //if (htmlAttributes.Any()) labelTag.MergeAttributes(htmlAttributes);
            labelTag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            if (String.IsNullOrEmpty(labelDescription))
            {
                labelTag.InnerHtml = labelText;
                return MvcHtmlString.Create(labelTag.ToString(TagRenderMode.Normal));
            }
            TagBuilder span = new TagBuilder("span");
            span.AddCssClass("label-details");
            span.SetInnerText(labelDescription);

            // assign <span> to <label> inner html
            labelTag.InnerHtml = labelText + span.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(labelTag.ToString(TagRenderMode.Normal));
        }
    }
}