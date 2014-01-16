using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DabTrial.Infrastructure.Validation
{
    public class PostedFileSizeAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _maxSize;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxSize">File Size in Bytes</param>
        public PostedFileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            return _maxSize > ((HttpPostedFileWrapper)value).ContentLength;
        }

        public override string FormatErrorMessage(string name=null)
        {
            return string.Format("The file size should not exceed {0:N0} KB", _maxSize/1000);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var returnVar = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(),
                ValidationType = "maxfilesize",
            };
            returnVar.ValidationParameters.Add("size", _maxSize);
            yield return returnVar;
        }
    }
    public class PostedFileTypesAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string[] _types;

        public PostedFileTypesAttribute(string types) : this(types.Split(','))
        {
        }

        public PostedFileTypesAttribute(params string[] types)
        {
            _types = types;
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var fileExt = System.IO.Path.GetExtension(((HttpPostedFileWrapper)value).FileName).Substring(1);
            return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Invalid file type. Only {0} are supported.", String.Join(", ", _types));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                       ControllerContext context)
        {
            var returnVar = new ModelClientValidationRule
            {
                ErrorMessage = "Invalid file type. Only {0} are supported.",
                ValidationType = "filetypes",
            };
            returnVar.ValidationParameters.Add("types", string.Join(",",_types));
            yield return returnVar;
        }
    }
}