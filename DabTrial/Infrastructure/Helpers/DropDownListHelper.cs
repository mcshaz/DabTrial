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
using DabTrial.Utilities;

namespace MvcHtmlHelpers
{
    public static partial class HtmlExtensions
    {
        public enum Orientation { Horizontal, Vertical }
        public enum Order { Before, After }
        public enum Elements { li, p, span, div }
        public enum selectTypes { radioButton, checkBox }

        //http://stackoverflow.com/questions/5621013/pass-enum-to-html-radiobuttonfor-mvc3#5621200

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="ItemTemplate">{0} = radio, {1} = label</param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonForEnum<TModel, TProperty>(
                        this HtmlHelper<TModel> htmlHelper,
                        Expression<Func<TModel, TProperty>> expression,
                        String ItemTemplate = " {0} {1} ")
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metaData);
            String[] names = Enum.GetNames(enumType);
            string checkedVal = (metaData.Model == null) ? "" : Enum.GetName(enumType, metaData.Model);
            StringBuilder sb = new StringBuilder();
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            var radioAttr = htmlHelper.GetUnobtrusiveValidationAttributes(expressionText, metaData);
            radioAttr.Add("id", null);
            if (metaData.IsRequired)
            {
                radioAttr.Add("data-val-requiredgroup", radioAttr["data-val-required"]);
            }
            var labelAttr = new Dictionary<string, object>();
            labelAttr.Add("for", null);
            foreach (var name in names)
            {
                string prefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
                if (!string.IsNullOrEmpty(prefix)) { prefix += '_';}
                labelAttr["for"] = radioAttr["id"] = prefix + metaData.PropertyName + '_' + name;
                if (name==checkedVal)
                {
                    if (!radioAttr.ContainsKey("checked")) { radioAttr.Add("checked", "checked"); }
                }
                else
                {
                    if (radioAttr.ContainsKey("checked")) { radioAttr.Remove("checked"); }
                }
                var radio = htmlHelper.RadioButton(expressionText, name, radioAttr).ToHtmlString();//need to set @checked = value.Equals(metaData.Model)
                var label = htmlHelper.Label(name.ToSeparatedWords(), labelAttr);
                sb.AppendFormat(ItemTemplate,
                    radio,
                    label
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
                    Text = converter.ConvertToString(value).ToSeparatedWords(),
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

            return Nullable.GetUnderlyingType(realModelType) ?? realModelType;
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

                baseAttr.Add("name", name);
                switch (element)
                {
                    case selectTypes.checkBox:
                        baseAttr.Add("type", "checkbox");
                        if (metaData.IsRequired) 
                        {
                            baseAttr.Add("data-val-requiredgroup", baseAttr["data-val-required"]);
                            baseAttr.Remove("data-val-required");
                        }
                        break;
                    case selectTypes.radioButton:
                        baseAttr.Add("type", "radio");
                        break;
                }

                ModelState modelState;
                string validationCssClass = null;
                if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
                {
                    if (modelState.Errors.Count > 0)
                    {
                        validationCssClass = HtmlHelper.ValidationInputCssClassName;
                    }
                }

                foreach (SelectListItem item in listOfValues)
                {
                    var input = new TagBuilder("input");
                    input.MergeAttributes(baseAttr);
                    if (validationCssClass != null) { input.AddCssClass(validationCssClass); }
                    // Generate an id to be given to the radio button field 
                    var id = string.Format("rb_{0}_{1}", 
                      name.Replace("[", "").Replace(
                      "]", "").Replace(".", "_"), item.Value);

                    input.MergeAttribute("id", id);
                    input.MergeAttribute("value", item.Value);
                    if (item.Selected) { input.MergeAttribute("checked", "checked"); }
                    // Create and populate a radio button using the existing html helpers 
                    var label = htmlHelper.Label(id, HttpUtility.HtmlEncode(item.Text));
                    //var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id }).ToHtmlString();
                    //var radio = htmlHelper.RadioButton(fullName, item.Value, item.Selected, new { id = id }).ToHtmlString();
                    
                    // Create the html string that will be returned to the client 
                    // e.g. <input data-val="true" data-val-required=
                    //   "You must select an option" id="TestRadio_1" 
                    //    name="TestRadio" type="radio"
                    //   value="1" /><label for="TestRadio_1">Line1</label> 
                    sb.AppendFormat(ItemTemplate, 
                                    input, 
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