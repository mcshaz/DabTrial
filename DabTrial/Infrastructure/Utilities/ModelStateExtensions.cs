using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DabTrial.Utilities
{
    public static class ModelStateExtensions
    {
        //jsonNetResult rather than jsonresult because zangit was serving 400 response header OR JSON, but not both???
        public static JsonResult JsonValidation(this ModelStateDictionary state)
        {
            var returnVar = new JsonResult
            {
                Data = new
                {
                    Tag = "ValidationError",
                    State = (from e in state
                            where e.Value.Errors.Any()
                            select new 
                            {
                                Name = e.Key,
                                Errors = e.Value.Errors.Select(x => x.ErrorMessage)
                                                  .Concat(e.Value.Errors.Where(x => x.Exception != null).Select(x => x.Exception.Message))
                            })
                }
            };
            return returnVar;
        }
    }
}