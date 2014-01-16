using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BronchiolitisTrial
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class enabledIfAttribute : Attribute, IMetadataAware
    {
        public const string enabledIfProperty = "enabledIfProperty";
        public const string enabledIfValue = "enabledIfValue";
        private readonly string _propertyName;
        private readonly object _propertyValue;

        public enabledIfAttribute(string propertyName, object propertyValue)
        {
            _propertyName = propertyName;
            _propertyValue = propertyValue;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add(enabledIfProperty, _propertyName);
            metadata.AdditionalValues.Add(enabledIfValue, _propertyValue);
        }
    }
}