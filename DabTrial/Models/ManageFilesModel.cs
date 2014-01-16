using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using DabTrial.Infrastructure.Validation;
using System.Web.Mvc;
using Foolproof;
using DabTrial.Infrastructure.FilesysServices;

namespace DabTrial.Models
{
    public enum DirectoryType { ForParentsAndCaregivers, ForHealthProfessionals, ForInvestigators }
    public class UploadFileModel: IFileModel
    {
        [RequiredIfNotEmpty("File")]
        public DirectoryType? SaveDirectory { get; set; }
        [StringLength(200, MinimumLength=2)]
        public string ServerFileName {get;set;}
        [PostedFileSize(10000000)]
        [PostedFileTypes(FileManagementService.AcceptedFileTypes)]
        public HttpPostedFileWrapper File { get; set; }
        public IEnumerable<SelectListItem> DirectoryList { get; set; }
    }
    public class ManageFileModel
    {
        [Required]
        public DirectoryType? SaveDirectory { get; set; } //nullable for interface compliance
        [Required]
        [StringLength(200, MinimumLength=2)]
        public string FileName {get;set;}
        public string FileType { get; set; }
    }
    public class ManageFileDetailsModel : ManageFileModel, IFileModel
    {
        [DisplayFormat(DataFormatString = "{0:s}")]
        public DateTime LastModified { get; set; }
        public string Src { get; set; }
        public IEnumerable<SelectListItem> DirectoryList { get; set; }
    }

    public interface IFileModel
    {
        DirectoryType? SaveDirectory { get; }
        IEnumerable<SelectListItem> DirectoryList { set; }
    }

    public class FileLink
    {
        public string FileName { get; set; }
        public string Href { get; set; }
    }
}