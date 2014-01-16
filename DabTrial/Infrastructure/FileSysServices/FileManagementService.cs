using DabTrial.Models;
using DabTrial.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;

namespace DabTrial.Infrastructure.FilesysServices
{
    public class FileManagementService
    {
        /*
        private readonly IValidationDictionary _valDictionary;
        public FileManagementService(IValidationDictionary valDictionary = null)
        {
            _valDictionary = valDictionary;
        }
         * */
        public static bool AddFile(HttpPostedFileWrapper postedFile, DirectoryType directory, string fileName)
        {
            string newName;
            fileName = (fileName ?? string.Empty).Trim();
            if (fileName == string.Empty)
            {
                newName = Path.GetFileName(postedFile.FileName);
            }
            else
            {
                string fileType = Path.GetExtension(postedFile.FileName);
                newName = Path.GetFileName(fileName);
                if (!newName.EndsWith(fileType)) { newName += fileType; }
            }
            try
            {
                postedFile.SaveAs(Combine(directory, newName));
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return false;
            }
            return true;
        }
        private static HttpStatusCode ModifyFileSys(Action modifyFS)
        {
            if (modifyFS == null) { throw new ArgumentNullException(); }
            try
            {
                modifyFS();
                return HttpStatusCode.OK;
            }
            catch (UnauthorizedAccessException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return HttpStatusCode.NotImplemented; 
            }
            catch (DirectoryNotFoundException e)
            {
                //wil log an error here as there is no user text input, so something is wrong with the UI if this exception occurs
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return HttpStatusCode.NotFound; 
            }
            catch (NotSupportedException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return HttpStatusCode.NotImplemented; 
            }
            catch (PathTooLongException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return HttpStatusCode.ExpectationFailed; 
            }
            catch (IOException e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return HttpStatusCode.InternalServerError; 
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static HttpStatusCode ModifyFile(ManageFileModel source, ManageFileModel destination)
        {
            var srcFile = FileManagementService.Combine(source.SaveDirectory.Value, source.FileName + '.' + source.FileType);
            var newFile = FileManagementService.Combine(destination.SaveDirectory.Value, destination.FileName + '.' + source.FileType);
            return ModifyFileSys(delegate { System.IO.File.Move(srcFile, newFile); });
        }
        public const string AcceptedFileTypes = "pdf,doc,dot,docx,dotx,docm,dotm,xls,xlt,xla,xlsx,xltx,xlsm,xltm,xlam,xlsb,ppt,pot,pps,ppa,pptx,potx,ppsx,ppam,pptm,potm,ppsm";
        public static string GetPath(DirectoryType directory)
        {
            switch (directory)
            {
                case DirectoryType.ForHealthProfessionals:
                    return @"~\Content\PublicFiles\HealthProfessionals\" ;
                case DirectoryType.ForInvestigators:
                    return @"~\App_Data\PrivateStatic\";
                case DirectoryType.ForParentsAndCaregivers:
                    return @"~\Content\PublicFiles\Parents\";
                default:
                    throw new ArgumentException();
            }
        }
        public static IList<ManageFileDetailsModel> GetAllFiles()
        {
            var returnList = new List<ManageFileDetailsModel>();
            var acceptedTypes = AcceptedFileTypes.Split(',');
            foreach (DirectoryType directory in Enum.GetValues(typeof(DirectoryType)))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(GetFullPath(directory));
                foreach(var file in dirInfo.EnumerateFiles())
                {
                    int dotPos = file.Name.LastIndexOf('.');
                    if (dotPos == 0) { continue; }
                    string fileType = file.Name.Substring(dotPos+1);
                    if (acceptedTypes.Contains(fileType))
                    {
                        var src = new ManageFileModel
                        {
                            FileName = file.Name.Substring(0, dotPos),
                            SaveDirectory = directory,
                            FileType = fileType
                        };
                        //string oldValues = JsonConvert.SerializeObject(newMiniModel);
                        returnList.Add(new ManageFileDetailsModel
                        {
                            FileName = src.FileName,
                            SaveDirectory = src.SaveDirectory,
                            FileType = src.FileType,
                            LastModified = file.LastWriteTime,
                            Src = JsonConvert.SerializeObject(src)
                        });
                    }
                }
            }
            return returnList;
        }
        public static IDictionary<DirectoryType,IEnumerable<FileLink>> GetAllLinks(bool isAuthenticated=false)
        {
            var returnDict = new Dictionary<DirectoryType,IEnumerable<FileLink>>();
            var acceptedTypes = AcceptedFileTypes.Split(',');
            var uriTool = new UriStringTools();
            foreach (DirectoryType directory in Enum.GetValues(typeof(DirectoryType)))
            {
                if (!isAuthenticated && directory == DirectoryType.ForInvestigators)
                {
                    continue;
                }
                string directoryPath = GetFullPath(directory);
                string[] dirFiles = Directory.GetFiles(directoryPath);
                var currentFiles = new List<FileLink>(dirFiles.Length);
                bool directDir = directory != DirectoryType.ForInvestigators;
                returnDict.Add(directory, currentFiles);
                foreach (string file in dirFiles)
                {
                    string ext = Path.GetExtension(file).Substring(1);
                    if (acceptedTypes.Contains(ext))
                    {
                        var newLink = new FileLink { FileName = Path.GetFileName(file) };
                        if (directDir && ext == "pdf") 
                        {
                            newLink.Href = uriTool.ReverseMapPath(directoryPath, false).Substring(1) + Uri.EscapeDataString(newLink.FileName);  
                        }
                        currentFiles.Add(newLink);
                    }
                }
            }
            return returnDict;
        }
        public static HttpStatusCode DeleteFile(DirectoryType directory, string fileName)
        {
            fileName = FileManagementService.Combine(directory, fileName);
            return ModifyFileSys(delegate { System.IO.File.Delete(fileName); });
        }
        public static string GetFullPath(DirectoryType directory)
        {
            return HostingEnvironment.MapPath(GetPath(directory));
        }
        public static string Combine(DirectoryType directory, string fileName)
        {
            string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(GetFullPath(directory),fileName); 
        }

        public static string MimeType(string fileName)
        {
            int dotpos = fileName.LastIndexOf('.')+1;
            if (dotpos == 0) { return null; }
            switch (fileName.Substring(dotpos))
            {
                case "pdf":
                    return "application/pdf";
                case "doc":
                    return "application/msword";
                case "dot":
                    return "application/msword";
                case "docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "dotx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case "docm":
                    return "application/vnd.ms-word.document.macroEnabled.12";
                case "dotm":
                    return "application/vnd.ms-word.template.macroEnabled.12";
                case "xls":
                    return "application/vnd.ms-excel";
                case "xlt":
                    return "application/vnd.ms-excel";
                case "xla":
                    return "application/vnd.ms-excel";
                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "xltx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case "xlsm":
                    return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case "xltm":
                    return "application/vnd.ms-excel.template.macroEnabled.12";
                case "xlam":
                    return "application/vnd.ms-excel.addin.macroEnabled.12";
                case "xlsb":
                    return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case "ppt":
                    return "application/vnd.ms-powerpoint";
                case "pot":
                    return "application/vnd.ms-powerpoint";
                case "pps":
                    return "application/vnd.ms-powerpoint";
                case "ppa":
                    return "application/vnd.ms-powerpoint";
                case "pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case "potx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case "ppsx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case "ppam":
                    return "application/vnd.ms-powerpoint.addin.macroEnabled.12";
                case "pptm":
                    return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case "potm":
                    return "application/vnd.ms-powerpoint.template.macroEnabled.12";
                case "ppsm":
                    return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";

                default:
                    return null;
            }
        }
    }
}