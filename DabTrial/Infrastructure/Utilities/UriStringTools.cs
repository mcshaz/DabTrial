using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Utilities
{
    public class UriStringTools
    {
        public readonly string AppPath;
        public UriStringTools()
        {
            AppPath = HttpContext.Current.Server.MapPath("~");
        }
        public static string GetAbsoluteUri()
        {
            HttpRequest request = HttpContext.Current.Request;
            //from http://stackoverflow.com/questions/126242/how-do-i-turn-a-relative-url-into-a-full-url#126258
            return string.Format("http{0}://{1}{2}",
                request.IsSecureConnection ? "s" : "",
                request.Url.Host,
                request.Url.Port == 80 ? "" : ":" + request.Url.Port.ToString());
        }
        public string ReverseMapPath(string path, bool fullAddress = false)
        {
            string relPath = path.Replace(AppPath, "").Replace("\\", "/");
            if (fullAddress)
            {
                return GetAbsoluteUri() + '/' + relPath;
            }
            return "~/" + relPath;
        }
    }
}