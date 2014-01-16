using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System.Reflection;
//using Microsoft.Web.Mvc;

namespace DabTrial.Infrastructure.Validation
{
    public enum AnnotationArgumentType { PropertyName, DateTime }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ComesBeforeAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessageFormatString = "{0} must be before {1}.";

        protected readonly String OtherDateProperty;
        protected readonly DateTime ComparisonDate;

        public ComesBeforeAttribute(String comparisonDate, AnnotationArgumentType dataType = AnnotationArgumentType.PropertyName)
        {
            switch (dataType)
            {
                case AnnotationArgumentType.DateTime:
                    this.ComparisonDate = DateTime.Parse(comparisonDate);
                    break;
                case AnnotationArgumentType.PropertyName:
                    this.OtherDateProperty = comparisonDate;
                    break;
                default:
                    throw new ArgumentException("dataType", "Unknown value");
            }
        }

        protected override ValidationResult IsValid(object value,
                                                    ValidationContext validationContext)
        {
            var otherDate = String.IsNullOrEmpty(OtherDateProperty) ? ComparisonDate : DateUtilities.DateFromProperty(validationContext, OtherDateProperty);
            var date = value as DateTime?;
            if (date.HasValue && otherDate.HasValue)
            {
                if (date <= otherDate) return ValidationResult.Success;
                string errorMessage = GetErrorMessage(validationContext.ObjectType, validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }
            else
            {
                //if one or both are null, must rely on required validator
                return ValidationResult.Success;
            }
        }
        protected string GetErrorMessage(Type containerType, string displayName)
        {
            string otherDisplayName;
            if (String.IsNullOrEmpty(OtherDateProperty))
            {
                otherDisplayName = ComparisonDate.ToShortDateString();
            }
            else
            {
                ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, containerType,
                                                                                            OtherDateProperty);
                otherDisplayName = metadata.GetDisplayName();
            }
            return ErrorMessage ??  string.Format(DefaultErrorMessageFormatString,
                                                 displayName,
                                                 otherDisplayName);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = GetErrorMessage(metadata.ContainerType, metadata.GetDisplayName()),
                ValidationType = "comesbefore",
            };
            if (String.IsNullOrEmpty(OtherDateProperty)) 
            {
                rule.ValidationParameters.Add("comparisondate", ComparisonDate.ToString("s"));
            }
            else
            {
                rule.ValidationParameters.Add("otherdateprop", "*." + OtherDateProperty); // for edge cases displaying in heirarchical form
            }

            yield return rule;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ComesAfterAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessageFormatString = "{0} must be After {1}.";

        protected readonly String OtherDateProperty;
        protected readonly DateTime ComparisonDate;

        public ComesAfterAttribute(String comparisonDate, AnnotationArgumentType dataType = AnnotationArgumentType.PropertyName)
        {
            switch (dataType)
            {
                case AnnotationArgumentType.DateTime:
                    this.ComparisonDate = DateTime.Parse(comparisonDate);
                    break;
                case AnnotationArgumentType.PropertyName:
                    this.OtherDateProperty = comparisonDate;
                    break;
                default:
                    throw new ArgumentException("dataType", "Unknown value");
            }
        }

        protected override ValidationResult IsValid(object value,
                                                    ValidationContext validationContext)
        {
            var otherDate = String.IsNullOrEmpty(OtherDateProperty)?ComparisonDate:DateUtilities.DateFromProperty(validationContext, OtherDateProperty);
            var date = value as DateTime?;
            if (date.HasValue && otherDate.HasValue)
            {
                if (date >= otherDate) return ValidationResult.Success;
                string errorMessage = GetErrorMessage(validationContext.ObjectType, validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }
            else
            {
                //if one or both are null, must rely on required validator
                return ValidationResult.Success;
            }
        }
        protected string GetErrorMessage(Type containerType, string displayName)
        {
            string otherDisplayName;
            if (String.IsNullOrEmpty(OtherDateProperty))
            {
                otherDisplayName = ComparisonDate.ToShortDateString();
            }
            else
            {
                ModelMetadata metadata = DateUtilities.GetMetadataForNestedProperty(null, containerType, OtherDateProperty);
                otherDisplayName = metadata.GetDisplayName();
            }
            return ErrorMessage ?? string.Format(DefaultErrorMessageFormatString,
                                                 displayName,
                                                 otherDisplayName);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = GetErrorMessage(metadata.ContainerType, metadata.GetDisplayName()),
                ValidationType = "comesafter",
            };
            if (String.IsNullOrEmpty(OtherDateProperty))
            {
                rule.ValidationParameters.Add("comparisondate", ComparisonDate.ToString("s"));
            }
            else
            {
                rule.ValidationParameters.Add("otherdateprop", "*." + OtherDateProperty); // for edge cases displaying in heirarchical form
            }
            yield return rule;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class DateIntervalAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessageFormatString = "{0} must be {1}-{2} {3} {4} {5}.";
        protected string When = "before";

        protected readonly Char TimeUnit;
        protected readonly double Minimum;
        protected readonly double Maximum;
        protected readonly String OtherDateProperty;

        public DateIntervalAttribute(Char timeUnit, double minimum, double maximum, String otherDateProperty)
        {
            this.TimeUnit = timeUnit;
            this.Maximum = maximum;
            this.Minimum = minimum;
            this.OtherDateProperty = otherDateProperty;
        }

        protected override ValidationResult IsValid(object value,
                                                    ValidationContext validationContext)
        {
            var otherDate = DateUtilities.DateFromProperty(validationContext, OtherDateProperty);
            var date = value as DateTime?;
            if (date.HasValue && otherDate.HasValue)
            {
                double difference = DateUtilities.DateDifference(date.Value, otherDate.Value, TimeUnit);
                if (difference >= Minimum && difference <= Maximum) return ValidationResult.Success;
                string errorMessage = GetErrorMessage(validationContext.ObjectType, validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }
            else
            {
                //if one or both are null, must rely on required validator
                return ValidationResult.Success;
            }
        }
        protected string GetErrorMessage(Type containerType, string displayName)
        {
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, containerType,
                                                                                            OtherDateProperty);
            string otherDisplayName = metadata.GetDisplayName();
            return ErrorMessage ?? (Minimum<0 ? string.Format(DefaultErrorMessageFormatString,
                                                 displayName,
                                                 Math.Abs(Maximum),
                                                 Math.Abs(Minimum),
                                                 DateUtilities.FullTimeUnit(TimeUnit),
                                                 this.When,
                                                 otherDisplayName)
                                               : string.Format(DefaultErrorMessageFormatString,
                                                 displayName,
                                                 Minimum,
                                                 Maximum,
                                                 DateUtilities.FullTimeUnit(TimeUnit),
                                                 this.When,
                                                 otherDisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = GetErrorMessage(metadata.ContainerType, metadata.GetDisplayName()),
                ValidationType = "dateinterval",
            };
            rule.ValidationParameters.Add("unit", TimeUnit);
            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);
            rule.ValidationParameters.Add("otherdateprop", "*." + OtherDateProperty); // for edge cases displaying in heirarchical form

            yield return rule;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ComesLaterAttribute : DateIntervalAttribute
    {
        public ComesLaterAttribute(Char timeUnit,
                                        double minimum,
                                        double maximum,
                                        String otherDateProperty)
            : base(timeUnit, -maximum, -minimum, otherDateProperty) { this.When = "after"; }
    }
    public class ComesEarlierAttribute : DateIntervalAttribute
    {
        public ComesEarlierAttribute(Char timeUnit,
                                        double minimum,
                                        double maximum,
                                        String otherDateProperty)
            : base(timeUnit, minimum, maximum, otherDateProperty) { }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CGArangeAttribute : ValidationAttribute, IClientValidatable
    {
        private const string DefaultErrorMessageFormatString = "Corrected gestational age must be between {0} and {1}.";

        protected readonly double Minimum;
        protected readonly double Maximum;
        protected readonly string WeeksGestationAtBirthProperty;

        public CGArangeAttribute(double minimum, double maximum, string weeksGestationAtBirthProperty)
        {
            this.Maximum = maximum;
            this.Minimum = minimum;
            this.WeeksGestationAtBirthProperty = weeksGestationAtBirthProperty;
        }

        protected override ValidationResult IsValid(object value,
                                                    ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            Type type = validationContext.ObjectType;
            DateTime? dOB = (DateTime?)value;
            Int32? gestationAtBirth = (int?)type.GetProperty(WeeksGestationAtBirthProperty).GetValue(instance, null);
            if (!gestationAtBirth.HasValue)
            {
                if (!dOB.HasValue) {return ValidationResult.Success;} //rely on required attribute
                gestationAtBirth = 40; // default to term - if this default is not wanted, place required attribute on this field
            }
            double cga = DateUtilities.CorrectedAgeInWeeks(dOB.Value, gestationAtBirth.Value);
            if (cga >= Minimum && cga <= Maximum) { return ValidationResult.Success; }
            string errorMessage =GetErrorMessage();
            return new ValidationResult(errorMessage);
        }
        protected string GetErrorMessage()
        {
            return ErrorMessage ?? string.Format(DefaultErrorMessageFormatString,
                                                 Minimum,
                                                 Maximum);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage ?? GetErrorMessage(),
                ValidationType = "cgarange",
            };
            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);
            rule.ValidationParameters.Add("weeksgestationprop", "*." + WeeksGestationAtBirthProperty); // for edge cases displaying in heirarchical form
            yield return rule;
        }
    }

    public static class DateUtilities
    {
        internal static DateTime? DateFromProperty(ValidationContext validationContext, String datePropertyName)
        {
            var nestedProps = GetNestedPropertyDetails(validationContext.ObjectType, datePropertyName);
            object obj = validationContext.ObjectInstance;
            foreach (var np in nestedProps)
            {
                obj = np.Info.GetValue(obj, null);
            }

            if (obj == null) { return null; }

            Type dateType = nestedProps[nestedProps.Length - 1].Info.PropertyType;
            if (dateType == typeof(string))
            {
                return DateTime.Parse((string)obj);
            }
            else if (dateType == typeof(DateTime?) || dateType == typeof(DateTime))
            {
                return (DateTime?)obj;
            }
            else
            {
                throw new ArgumentException(String.Format("ValidationAttribute Argument Exception:{0}Class:\"{1}\"{0}Property:\"{2}\"{0}Only String and DateTime or DateTime? are acceptable for otherDateProperty, which is of type \"{3}\"",
                                                                                                Environment.NewLine,
                                                                                                datePropertyName,
                                                                                                DateUtilities.PropertyName(validationContext),
                                                                                                dateType));
            }
        }

        /* trial only*/
        internal static ModelMetadata GetMetadataForNestedProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            var nestedProps = GetNestedPropertyDetails(containerType, propertyName);
            int lastIndex = nestedProps.Length - 1;
            return ModelMetadataProviders.Current.GetMetadataForProperty(modelAccessor, nestedProps[lastIndex].ParentType, nestedProps[lastIndex].PropertyName);
        }
        internal static NestedPropertyDetails[] GetNestedPropertyDetails(Type containerType, string propertyName)
        {
            var propertyNames = propertyName.Split('.');
            var returnVar = new NestedPropertyDetails[propertyNames.Length];
            Type lastType = containerType;
            for (int i = 0; i < propertyNames.Length; i++)
            {
                var info = lastType.GetProperty(propertyNames[i]);
                returnVar[i] = new NestedPropertyDetails
                {
                    PropertyName = propertyNames[i],
                    Info = info,
                    ParentType = lastType
                };
                lastType = info.PropertyType;
            }
            return returnVar;
        }
        internal class NestedPropertyDetails
        {
            internal Type ParentType { get; set; }
            internal PropertyInfo Info { get; set; }
            internal String PropertyName { get; set; }
        }
        //18 months = 118 weeks
        internal static double CorrectedAgeInWeeks(DateTime Dob, double weeksGestationAtBirth) //, double correctFromLessThan=40, double correctToLessThan=43
        {
            if (weeksGestationAtBirth >= 43 || weeksGestationAtBirth < 23) { throw new ArgumentOutOfRangeException("weeksGestationAtBirth", weeksGestationAtBirth, "Must be between 23 and 42"); }
            return DateDifference(Dob, DateTime.Now, 'w') + weeksGestationAtBirth;
        }
        /// <summary>
        /// if otherDate comes after thisDate, returns +ve, otherwise -ve
        /// </summary>
        /// <param name="thisDate"></param>
        /// <param name="otherDate"></param>
        /// <param name="timeUnit">one of m,h,d,w,M,Y</param>
        /// <returns>if otherDate comes after thisDate, returns +ve, otherwise -ve</returns>
        public static double DateDifference(DateTime thisDate, DateTime otherDate, char timeUnit)
        {
            //if otherDate > thisDate, returns +ve, otherwise -ve
            TimeSpan currentSpan = otherDate - thisDate;
            switch (timeUnit)
            {
                case 'm':
                    return currentSpan.TotalMinutes;
                case 'h':
                    return currentSpan.TotalHours;
                case 'd':
                    return currentSpan.TotalDays;
                case 'w':
                    return currentSpan.TotalDays / 7;
                default:
                    double difference = otherDate.Month - thisDate.Month + 12 * (otherDate.Year - thisDate.Year);
                    int dayDif = otherDate.Day - thisDate.Day;
                    //get fraction part - more important for min=0
                    if (dayDif > 0)
                    {
                        difference += dayDif / DateTime.DaysInMonth(otherDate.Year, otherDate.Month);
                    }
                    else if (dayDif < 0)
                    {
                        difference += dayDif / DateTime.DaysInMonth(otherDate.Year, otherDate.Month - 1);
                    }
                    if (timeUnit == 'Y') { difference /= 12; }
                    return difference;
            }
        }
        public static string PropertyName(ValidationContext validationContext)
        {
            var displayName = validationContext.DisplayName;

            return validationContext.ObjectType.GetProperties()
                .Where(p => p.GetCustomAttributes(false).OfType<DisplayAttribute>().Any(a => a.Name == displayName))
                .Select(p => p.Name)
                .FirstOrDefault();
        }
        public static string FullTimeUnit(char timeUnit)
        {
            switch (timeUnit)
            {
                case 'm':
                    return "minutes";
                case 'h':
                    return "hours";
                case 'd':
                    return "days";
                case 'w':
                    return "weeks";
                case 'M':
                    return "months";
                case 'Y':
                    return "years";
                default:
                    throw new Exception("timeUnit must be 1 of m,h,d,w,M or Y");
            }
        }
    }
}