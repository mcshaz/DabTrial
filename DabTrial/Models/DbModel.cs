using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Foolproof;
using System;

namespace DabTrial.Models
{
    public class DbVersionBakModel
    {
        public string[] SelectedFileNames { get; set; }
        public IEnumerable<BakFileInfo> BackupFiles { get; set; }
        public bool DifferentialOnly { get; set; }
    }
    public class BakFileInfo
    {
        public string FileName { get; set; }
        [DisplayFormat(DataFormatString="{0:N0}")]
        public long Size { get; set; }
    }
    public class AutomatedBakInfo :BakFileInfo
    {
        [DisplayFormat(DataFormatString = "{0:F}")]
        public DateTime LastModified { get; set; }
        private bool _canPostBack = true;
        public bool CanPostBack { get { return _canPostBack; } set { _canPostBack = value; } }
    }
}