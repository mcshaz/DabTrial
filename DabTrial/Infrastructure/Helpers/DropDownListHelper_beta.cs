using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Text;
//my additions
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BronchiolitisTrial.Utilities;
using System.Web.Routing;

namespace MvcHtmlHelpers
{
    public static partial class HtmlExtensions
    {
        public enum Orientation { Horizontal, Vertical }
        public enum Order { Before, After }
        public enum Elements { li, p, span, div }
        public enum selectTypes { radioButton, checkBox }

        //http://stackoverflow.com/questions/5621013/pass-enum-to-html-radiobuttonfor-mvc3#5621200
        public static MvcHtmlString RadioButtonForEnum<TModel, TProperty>(
                        this HtmlHelper<TModel> htmlHelper,
                        Expression<Func<TModel, TProperty>> expression,
                        Order LabelPosition = Order.After
                    )
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metaData);
            String[] names = Enum.GetNames(enumType);
            StringBuilder sb = new StringBuilder();
            string formatStr = LabelPosition == Order.Before ? "<label for=\"{0}\">{1}</label>{2} ": 
                                                               "{2}<label for=\"{0}\">{1}</label> ";

            foreach (var name in names)
            {
                var id = string.Format(
                    "{0}_{1}_{2}",
                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                    metaData.PropertyName,
                    name
                );
                var radio = htmlHelper.RadioButtonFor(expression, name, new { id = id }).ToHtmlString();
                sb.AppendFormat(
                    formatStr,
                    id,
                    HttpUtility.HtmlEncode(name),
                    radio
                );
            }
            return MvcHtmlString.Create(sb.ToString());
        }
        //enum dropdownlist
        //http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx
        /*
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor<TModel, TEnum>(htmlHelper, expression, "");
        }
         * */

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string nullText=null)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metaData);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            TypeConverter converter = TypeDescriptor.GetConverter(enumType);

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = converter.ConvertToString(value),
                    Value = value.ToString(),
                    Selected = value.Equals(metaData.Model)
                };

            if (metaData.IsNullableValueType)
            {
                SelectListItem[] SingleEmptyItem = new[] { new SelectListItem {
                    Text= nullText ?? "", 
                    Value = ""
                }};
                items = SingleEmptyItem.Concat(items);
            }

            return htmlHelper.DropDownListFor(
                expression,
                items
                );
        }
        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

        public static string ToDescription(this Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType().GetField(
              value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
        //much of the following code adapted from http://www.codeproject.com/Articles/404022/MVC-Enum-RadioButtonList-Helper
        /// <summary>
        /// Insert a list of radio buttons by enumerating through SelectListItems
        /// </summary>
        /// <param name="ItemTemplate">{0} = element, {1} = label</param>
        public static MvcHtmlString ButtonListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> listOfValues,
            selectTypes element = selectTypes.radioButton,
            String ItemTemplate = "<span>{0} {1}</span>")
        {
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
                string name = ExpressionHelper.GetExpressionText(expression);

                var baseAttr = htmlHelper.GetUnobtrusiveValidationAttributes(name, metaData);
                baseAttr.Add("id", "");
                //baseAttr.Add("name", name);
                if (element == selectTypes.checkBox && metaData.IsRequired)
                {
                    baseAttr.Add("data-val-requiredgroup", baseAttr["data-val-required"]);
                    if (baseAttr.ContainsKey("data-val-required")) { baseAttr.Remove("data-val-required"); }
                }

                ModelState modelState;
                if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
                {
                    if (modelState.Errors.Count > 0)
                    {
                        if (baseAttr.ContainsKey("class"))
                        {
                            baseAttr["class"] += ',' + HtmlHelper.ValidationInputCssClassName;
                        }
                        else
                        {
                            baseAttr.Add("class", HtmlHelper.ValidationInputCssClassName);
                        }
                    }
                }

                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field 
                    var id = string.Format("rb_{0}_{1}", 
                      name.Replace("[", "").Replace(
                      "]", "").Replace(".", "_"), item.Value);
                    baseAttr["id"] = id;
                    MvcHtmlString input;
                    if (element == selectTypes.checkBox)
                    {
                        input = htmlHelper.CheckBox(name, item.Selected, baseAttr);
                    }
                    else
                    {
                        input = htmlHelper.RadioButton(name, item.Value, item.Selected, baseAttr);
                    }
                    // Create and populate a radio button using the existing html helpers 
                    var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));

                    sb.AppendFormat(ItemTemplate, 
                                    input.ToHtmlString(), 
                                    label);
                }
            }
            return MvcHtmlString.Create(sb.ToString());
        }
        

        /// <summary>
        /// if not required, returns an empty string, otherwise determines an appropriate error message
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="modelType"></param>
        /// <param name="defaultMsg">{0} - position of element name</param>
        /// <returns></returns>
        private static string getRequiredMessage(ModelMetadata metaData,
                                                 Type modelType,
                                                 string defaultMsg = "A {0} must be selected")
        {
            if (metaData.IsRequired)
            {
                var requiredAttr = getCustomAttributesForProperty(metaData, modelType)
                                    .OfType<RequiredAttribute>()
                                    .First(); 
                string errMsg = requiredAttr.ErrorMessage;
                if (String.IsNullOrEmpty(errMsg))
                {
                    String displayText = metaData.DisplayName ?? metaData.PropertyName; //ExpressionHelper.GetExpressionText(expression)
                    displayText = displayText.Singularise();
                    errMsg = String.Format(defaultMsg, displayText);
                }
                return errMsg;
            }
            return null;
        }
        public static object[] getCustomAttributesForProperty(ModelMetadata metadata, Type modelType)
        {
            var prop = modelType.GetProperty(metadata.PropertyName);
            return prop.GetCustomAttributes(true);
        }
    }
}