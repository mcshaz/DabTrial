using DabTrial;
using Mvc.JQuery.DataTables;
using System.Web;
using System.Web.Mvc;

[assembly: PreApplicationStartMethod(typeof(RegisterDataTablesModelBinder), "Start")]

namespace DabTrial
{

    public static class RegisterDataTablesModelBinder
    {
        public static void Start()
        {
            if (!ModelBinders.Binders.ContainsKey(typeof (DataTablesParam)))
                ModelBinders.Binders.Add(typeof (DataTablesParam), new DataTablesModelBinder());
        }
    }
}