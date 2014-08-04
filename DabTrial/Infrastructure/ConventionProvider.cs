using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DabTrial.Utilities;

namespace Web
{
    public class ConventionProvider :
        DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Func<object> modelAccessor,
            Type modelType,
            string propertyName)
        {
            var meta = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            if (meta.DisplayName == null)
                meta.DisplayName = meta.PropertyName.ToSeparateWords();
            return meta;
        }
    }
}