using DabTrial;
using DabTrial.Infrastructure.Validation;
using DabTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UkAdcHtmlAttributeProvider.Infrastructure;
using Web;

namespace DabTrial
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AutoMapperConfiguration.Configure();

            ModelMetadataProviders.Current = new ConventionProvider();

            HtmlAttributeProvider.Register((metadata) =>
            {
                switch (metadata.DataTypeName)
                {
                    case "Date":
                        return new[] 
                            { 
                                new KeyValuePair<string, object>("class", "date"),
                                new KeyValuePair<string, object>("placeholder", "day/month/year")
                            };
                    case "DateTime":
                        return new[] 
                            { 
                                new KeyValuePair<string, object>("class", "dateTime"),
                                new KeyValuePair<string, object>("placeholder", "day/month/year 24Hr:min")
                            };
                    case "Time":
                        return new[] 
                            { 
                                new KeyValuePair<string, object>("class", "time"),
                                new KeyValuePair<string, object>("placeholder", "24Hr:min")
                            };
                    default:
                        return null;
                };
            });

            HtmlAttributeProvider.Register((metadata) =>
            {
                if (metadata.AdditionalValues.ContainsKey(DateIntervalClientOnlyAttribute.DateIntervalValProperty))
                {
                    return metadata.AdditionalValues[DateIntervalClientOnlyAttribute.DateIntervalValProperty] as IEnumerable<KeyValuePair<string, object>>;
                }
                return null;
            });
            HtmlAttributeProvider.Register((metadata) =>
            {
                return MultipleValidationAttribute.GetAttributes(metadata);
            });
        }
    }
}
