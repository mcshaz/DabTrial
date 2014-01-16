using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BronchiolitisTrial.Infrastructure
{
    //http://www.campusmvp.net/blog/asp-net-mvc-return-of-zip-files-created-on-the-fly
	public class ZipResult : ActionResult
	{
	    private IEnumerable<string> _files;
	    private string _fileName;
	  
	    public string FileName
	    {
	        get
	        {
	            return _fileName ?? "file.zip";
	        }
	        set { _fileName = value; }
	    }
	  
	    public ZipResult(params string[] files)
	    {
	        this._files = files;
	    }
	  
	    public ZipResult(IEnumerable<string> files)
	    {
	        this._files = files;
	    }

	  
	    public override void ExecuteResult(ControllerContext context)
	    {
            using (ZipFile zf = new ZipFile())
            {
                zf.CompressionMethod = CompressionMethod.None;
                zf.AddFiles(_files, false, "");
                MemoryStream stream = new MemoryStream();
                zf.Save(stream);
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.BufferOutput = false;
                context.HttpContext
                    .Response.ContentType = "application/zip";
                context.HttpContext
                    .Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName);
                context.HttpContext
                    .Response.AppendHeader("Content-Length", stream.Length.ToString());
                context.HttpContext.Response.BinaryWrite(stream.ToArray());
                //zf.CompressionMethod = CompressionMethod.None;
                //context.HttpContext.Response.Clear();
                //context.HttpContext.Response.BufferOutput = false;
                //zf.AddFiles(_files, false, "");
                //context.HttpContext
                // .Response.ContentType = "application/zip";
                //context.HttpContext
                // .Response.AppendHeader("Content-Disposition", "attachment; filename=" + FileName);
                //context.HttpContext
                // .Response.AppendHeader("Content-Length", length.ToString());
                //zf.Save(context.HttpContext.Response.OutputStream);

            }
	    }
	  
	} 
}