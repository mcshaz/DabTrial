using DabTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using DabTrial.Utilities;

namespace DabTrial.Infrastructure.ModelBinders
{
    public class ParticipantModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //note difference between string.Empty and null in the following
            if (controllerContext.HttpContext.Request.Form["FirstAdrenalineNebAt"] != null)
            {
                bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(ParticipantInterventionUpdate));
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }

}