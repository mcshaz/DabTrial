using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using DabTrial.Models;



//using Microsoft.Web.Mvc;

namespace DabTrial.Infrastructure.Validation
{
    /*
     * legacy - available in .net 4.5
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class EmailAttribute : RegularExpressionAttribute
    {
        private const string pattern = @"^\w+([-+.]*[\w-]+)*@(\w+([-.]?\w+)){1,}\.\w{2,4}$";

        static EmailAttribute()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EmailAttribute), typeof(RegularExpressionAttributeAdapter));
        }

        public EmailAttribute()
            : base(pattern)
        {
            ErrorMessage = "Must be a valid email address";
        }
    }
    */
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IsValidRegExAttribute : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value,
                                                    ValidationContext validationContext)
        {
            try
            {
                Regex.Match("", (string)value);
            }
            catch (ArgumentException)
            {
                return new ValidationResult(GetErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
        private const string DefaultErrorMessageFormatString = "{0} must be a valid regular expression";
        protected string GetErrorMessage(string displayName)
        {
            return ErrorMessage ?? String.Format(DefaultErrorMessageFormatString, displayName);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = GetErrorMessage(metadata.GetDisplayName()),
                ValidationType = "isvalidregex",
            };
            yield return rule;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MustBeTrueAttribute : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value,
                                                    ValidationContext validationContext)
        {
            if ((bool)value)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(GetErrorMessage(validationContext.DisplayName));
        }
        private const string DefaultErrorMessageFormatString = "{0} must be selected";
        protected string GetErrorMessage(string displayName)
        {
            return ErrorMessage ?? String.Format(DefaultErrorMessageFormatString, displayName);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = GetErrorMessage(metadata.GetDisplayName()),
                ValidationType = "mustbetrue",
            };
            yield return rule;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TrueOnlyIfAttribute : ValidationAttribute, IClientValidatable
    {
        protected readonly String _otherProperty;
        protected readonly object[] _valuesToCompare;
        public TrueOnlyIfAttribute(string property, params object[] values)
        {
            _otherProperty = property;
            _valuesToCompare = values;
        }
        protected override ValidationResult IsValid(object value,
                                                    ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            Type classType = validationContext.ObjectType;
            var otherPropertyInfo = classType.GetProperty(_otherProperty);
            Object otherPropertyVal = otherPropertyInfo.GetValue(instance, null);
            //will need to convert valuesToCompare to date if that is the other Type type = otherPropertyInfo.PropertyType;

            if ((bool)value && !_valuesToCompare.Any(_v=>otherPropertyVal.Equals(_v)))
            {
                return new ValidationResult(GetErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
        private const string DefaultErrorMessageFormatString = "{0} can only be true if {1} has value of {2}";
        protected string GetErrorMessage(string displayName)
        {
            return ErrorMessage ?? String.Format(DefaultErrorMessageFormatString, displayName,
                                                 _otherProperty,
                                                 _valuesToCompare.Aggregate((current, next)=>String.Format("{0} or {1}",current, next)));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = GetErrorMessage(metadata.GetDisplayName()),
                ValidationType = "trueonlyif",
            };
            rule.ValidationParameters.Add("othervals", _valuesToCompare.Aggregate((current, next)=>String.Format("{0}||{1}",current,next)));
            rule.ValidationParameters.Add("otherprop", "*." + _otherProperty); 
            yield return rule;
        }
    }
}