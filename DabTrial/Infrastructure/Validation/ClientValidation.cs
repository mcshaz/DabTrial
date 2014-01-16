using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DabTrial.Infrastructure.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class DateIntervalClientOnlyAttribute : Attribute, IMetadataAware
    {
        private const string DefaultErrorMessageFormatString = "{0} must be {1}-{2} {3} {4} now.";
        protected string When = "before";

        protected readonly Char TimeUnit;
        protected readonly double Minimum;
        protected readonly double Maximum;

        public virtual string ErrorMessage { get; set; }

        public DateIntervalClientOnlyAttribute(Char timeUnit, double minimum, double maximum)
        {
            this.TimeUnit = timeUnit;
            this.Maximum = maximum;
            this.Minimum = minimum;
        }
        public DateIntervalClientOnlyAttribute(bool beforeNow)
        {
            this.TimeUnit = beforeNow ? 'b' : 'a';
        }
        private const string ToNowName = "data-val-datetonow";
        private const string IntervalName = "data-val-dateinterval";
        public const string DateIntervalValProperty = "DateIntervalValProperty";
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            if (TimeUnit == 'b' || TimeUnit == 'a')
            {

                metadata.AdditionalValues.Add(DateIntervalValProperty,
                    new[] {
                        new KeyValuePair<string,object>("data-val","true"),
                        new KeyValuePair<string,object>(ToNowName, ErrorMessage ?? String.Format("{0} must come {1} the current date and time",
                                                                                metadata.GetDisplayName(),
                                                                                TimeUnit == 'b' ? "before" : "after")),
                        new KeyValuePair<string,object>(ToNowName + "-opt", TimeUnit == 'b' ? "before" : "after")
                    });
            }
            else
            {
                metadata.AdditionalValues.Add(DateIntervalValProperty,
                    new[] {
                        new KeyValuePair<string,object>("data-val","true"),
                        new KeyValuePair<string,object>(IntervalName,GetErrorMessage(metadata.ContainerType, metadata.GetDisplayName())),
                        new KeyValuePair<string,object>(IntervalName + "-unit", TimeUnit),
                        new KeyValuePair<string,object>(IntervalName + "-min", Minimum),
                        new KeyValuePair<string,object>(IntervalName + "-max", Maximum)
                    });
            }
        }
        protected string GetErrorMessage(Type containerType, string displayName)
        {
            return ErrorMessage ?? (Minimum < 0 ? string.Format(DefaultErrorMessageFormatString,
                                                 displayName,
                                                 Math.Abs(Maximum),
                                                 Math.Abs(Minimum),
                                                 DateUtilities.FullTimeUnit(TimeUnit),
                                                 this.When)
                                               : string.Format(DefaultErrorMessageFormatString,
                                                 displayName,
                                                 Minimum,
                                                 Maximum,
                                                 DateUtilities.FullTimeUnit(TimeUnit),
                                                 this.When));
        }

    }
    public class ComesAfterNowAtClientAttribute : DateIntervalClientOnlyAttribute
    {
        public ComesAfterNowAtClientAttribute(Char timeUnit,
                                    double minimum,
                                    double maximum)
            : base(timeUnit, -maximum, -minimum) { this.When = "after"; }
        public ComesAfterNowAtClientAttribute()
            : base(beforeNow: false) { }
    }
    public class ComesBeforeNowAtClientAttribute : DateIntervalClientOnlyAttribute
    {
        public ComesBeforeNowAtClientAttribute(Char timeUnit,
                                    double minimum,
                                    double maximum)
            : base(timeUnit, minimum, maximum) { }
        public ComesBeforeNowAtClientAttribute()
            : base(beforeNow: true) { }
    }
}