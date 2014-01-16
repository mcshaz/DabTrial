using System;
using System.Web.Mvc;
using AutoMapper;

namespace DabTrial.Models
{
    public class AutoMapModelAttribute : ActionFilterAttribute
    {
        readonly Type _destType;
        readonly Type _sourceType;

        public AutoMapModelAttribute(Type sourceType, Type destType)
        {
            _sourceType = sourceType;
            _destType = destType;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var model = filterContext.Controller.ViewData.Model;

            var viewModel = Mapper.Map(model, _sourceType, _destType);

            filterContext.Controller.ViewData.Model = viewModel;
        }
    }
}