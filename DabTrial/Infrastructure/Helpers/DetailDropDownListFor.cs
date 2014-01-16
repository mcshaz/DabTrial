using System.Collections;
// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// http://aspnetwebstack.codeplex.com/SourceControl/changeset/3a49f5a23bd0#src%2fSystem.Web.Mvc%2fHtml%2fSelectExtensions.cs
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web;

namespace MvcHtmlHelpers
{
    public static partial class HtmlExtensions
     {
     // DetailDropDownList
 
         public static MvcHtmlString DetailDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList)
         {
            return DetailDropDownList(htmlHelper, name, selectList, null /* optionLabel */, null /* htmlAttributes */);
         }
 
         public static MvcHtmlString DetailDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList, object htmlAttributes)
         {
            return DetailDropDownList(htmlHelper, name, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
         }
 
         public static MvcHtmlString DetailDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
         {
            return DetailDropDownList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes);
         }
 
         public static MvcHtmlString DetailDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList, string optionLabel)
         {
            return DetailDropDownList(htmlHelper, name, selectList, optionLabel, null /* htmlAttributes */);
         }
 
         public static MvcHtmlString DetailDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList, string optionLabel, object htmlAttributes)
         {
            return DetailDropDownList(htmlHelper, name, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
         }
 
         public static MvcHtmlString DetailDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
         {
            return DetailDropDownListHelper(htmlHelper, metadata: null, expression: name, selectList: selectList, optionLabel: optionLabel, htmlAttributes: htmlAttributes);
         }
 
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString DetailDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList)
         {
            return DetailDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, null /* htmlAttributes */);
         }
 
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString DetailDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList, object htmlAttributes)
         {
            return DetailDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
         }
 
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString DetailDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
         {
            return DetailDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
         }
 
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString DetailDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList, string optionLabel)
         {
            return DetailDropDownListFor(htmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */);
         }
 
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString DetailDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList, string optionLabel, object htmlAttributes)
         {
            return DetailDropDownListFor(htmlHelper, expression, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
         }
 
         [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString DetailDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
         {
             if (expression == null)
             {
                throw new ArgumentNullException("expression");
             }
 
             ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
 
             return DetailDropDownListHelper(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes);
         }
 
         private static MvcHtmlString DetailDropDownListHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string expression, IEnumerable<DetailSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
         {
            return SelectInternal(htmlHelper, metadata, optionLabel, expression, selectList, allowMultiple: false, htmlAttributes: htmlAttributes);
         }
 
         // ListBox
 
         public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name)
         {
            return ListBox(htmlHelper, name, null /* selectList */, null /* htmlAttributes */);
         }
 
         public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList)
         {
            return ListBox(htmlHelper, name, selectList, (IDictionary<string, object>)null);
         }
 
         public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList, object htmlAttributes)
         {
            return ListBox(htmlHelper, name, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
         }
 
         public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<DetailSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
         {
            return ListBoxHelper(htmlHelper, metadata: null, name: name, selectList: selectList, htmlAttributes: htmlAttributes);
         }
 
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList)
         {
            return ListBoxFor(htmlHelper, expression, selectList, null /* htmlAttributes */);
         }
 
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList, object htmlAttributes)
         {
            return ListBoxFor(htmlHelper, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
         }
 
         [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
         [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
         public static MvcHtmlString ListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<DetailSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
         {
             if (expression == null)
             {
                throw new ArgumentNullException("expression");
             }
 
             ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
 
             return ListBoxHelper(htmlHelper,
             metadata,
             ExpressionHelper.GetExpressionText(expression),
             selectList,
             htmlAttributes);
         }
 
         private static MvcHtmlString ListBoxHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string name, IEnumerable<DetailSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
         {
            return SelectInternal(htmlHelper, metadata, optionLabel: null, name: name, selectList: selectList, allowMultiple: true, htmlAttributes: htmlAttributes);
         }
 
         // Helper methods
 
         private static IEnumerable<DetailSelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
         {
             object o = null;
             if (htmlHelper.ViewData != null)
             {
                o = htmlHelper.ViewData.Eval(name);
             }
             if (o == null)
             {
                 throw new InvalidOperationException(
                     "Missing Select Data: " +//MvcResources.HtmlHelper_MissingSelectData,
                     name +
                     "IEnumerable<DetailSelectListItem>");
             }
             IEnumerable<DetailSelectListItem> selectList = o as IEnumerable<DetailSelectListItem>;
             if (selectList == null)
             {
                 throw new InvalidOperationException(
                     "Wrong Select Data Type " + //MvcResources.HtmlHelper_WrongSelectDataType,
                     name + 
                     o.GetType().FullName +
                     "IEnumerable<DetailSelectListItem>");
             }
             return selectList;
         }

        internal static string ListItemToOption(DetailSelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Detail != null)
            {
                builder.Attributes["data-detail"] = item.Detail;
            }
            if (item.IsDisabled)
            {
                builder.Attributes["disabled"] = "disabled";
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            return builder.ToString(TagRenderMode.Normal);
        }
 
         private static IEnumerable<DetailSelectListItem> GetSelectListWithDefaultValue(IEnumerable<DetailSelectListItem> selectList, object defaultValue, bool allowMultiple)
         {
            IEnumerable defaultValues;
 
             if (allowMultiple)
             {
                defaultValues = defaultValue as IEnumerable;
                 if (defaultValues == null || defaultValues is string)
                 {
                     throw new InvalidOperationException(
                         "Select Expression Not Enumerable");
                 }
             }
             else
             {
                defaultValues = new[] { defaultValue };
             }
 
             IEnumerable<string> values = from object value in defaultValues
             select Convert.ToString(value, CultureInfo.CurrentCulture);
             HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
             List<DetailSelectListItem> newSelectList = new List<DetailSelectListItem>();
 
             foreach (DetailSelectListItem item in selectList)
             {
                 item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                 newSelectList.Add(item);
             }
             return newSelectList;
        }
 
         private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, ModelMetadata metadata, string optionLabel, string name, IEnumerable<DetailSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
         {
             string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
             if (String.IsNullOrEmpty(fullName))
             {
                throw new ArgumentException("unable to get fullname with GetFullHtmlFieldName", "name");
             }
 
             bool usedViewData = false;
 
             // If we got a null selectList, try to use ViewData to get the list of items.
             if (selectList == null)
             {
                 selectList = htmlHelper.GetSelectData(name);
                 usedViewData = true;
             }
 
             object defaultValue = (allowMultiple) ? htmlHelper.GetModelStateValue(fullName, typeof(string[])) : htmlHelper.GetModelStateValue(fullName, typeof(string));
 
             // If we haven't already used ViewData to get the entire list of items then we need to
             // use the ViewData-supplied value before using the parameter-supplied value.
             if (!usedViewData && defaultValue == null && !String.IsNullOrEmpty(name))
             {
                defaultValue = htmlHelper.ViewData.Eval(name);
             }
 
             if (defaultValue != null)
             {
                selectList = GetSelectListWithDefaultValue(selectList, defaultValue, allowMultiple);
             }
 
             // Convert each ListItem to an <option> tag
             StringBuilder listItemBuilder = new StringBuilder();
 
             // Make optionLabel the first item that gets rendered.
             if (optionLabel != null)
             {
                listItemBuilder.AppendLine(ListItemToOption(new DetailSelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));
             }
 
             foreach (DetailSelectListItem item in selectList)
             {
                listItemBuilder.AppendLine(ListItemToOption(item));
             }

             TagBuilder selectTagBuilder = new TagBuilder("select")
             {
                InnerHtml = listItemBuilder.ToString()
             };
             selectTagBuilder.MergeAttributes(htmlAttributes);
             selectTagBuilder.MergeAttribute("details", "true");
             selectTagBuilder.MergeAttribute("detaildisplayid", HtmlHelperExtensions.GetDetailDisplayId(fullName));
             selectTagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
             selectTagBuilder.GenerateId(fullName);

             if (allowMultiple)
             {
                selectTagBuilder.MergeAttribute("multiple", "multiple");
             }
 
             // If there are any errors for a named field, we add the css attribute.
             ModelState modelState;
             if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
             {
                 if (modelState.Errors.Count > 0)
                 {
                    selectTagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                 }
             }
            
             selectTagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name, metadata));

             return MvcHtmlString.Create(selectTagBuilder.ToString(TagRenderMode.Normal));
        }

         public static MvcHtmlString DetailDisplayFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string tagName="span")
         {
             return DetailDisplayFor(htmlHelper, expression, null /* htmlAttributes */, tagName);
         }

         public static MvcHtmlString DetailDisplayFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes, string tagName = "span")
         {
             return DetailDisplayFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), tagName);
         }

         public static MvcHtmlString DetailDisplayFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes, string tagName = "span")
         {
             if (expression == null)
             {
                 throw new ArgumentNullException("expression");
             }

             ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

             return DetailDisplayInternal(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), tagName, htmlAttributes);
         }

        private static MvcHtmlString DetailDisplayInternal(this HtmlHelper htmlHelper, ModelMetadata metadata, string name, string tagType, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            TagBuilder detail = new TagBuilder(tagType);
            detail.MergeAttributes(htmlAttributes);
            detail.MergeAttribute("id", HtmlHelperExtensions.GetDetailDisplayId(fullName));
            return MvcHtmlString.Create(detail.ToString(TagRenderMode.Normal));
        }
    }
    static class HtmlHelperExtensions
    {
        public static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }
        public static string GetDetailDisplayId(string fullName)
        {
            fullName += "Detail";
            return TagBuilder.CreateSanitizedId(fullName);
        }
    }
    public class DetailSelectListItem : SelectListItem
    {
        public string Detail { get; set; }
        public bool IsDisabled { get; set; }
    }

}