using DabTrial.Domain.Providers;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Hosting;

namespace DabTrial.Infrastructure.FilesysServices
{
    public class DbBackupService
    {
        public static string[] BackupFileTypes = new string[] {".bak", ".sdf", ".zip" };
        private IValidationDictionary _validatonDictionary;
        public DbBackupService(IValidationDictionary valDictionary)
        {
            _validatonDictionary = valDictionary ?? new EmptyValidationShell();
        }
        private string _dbBakDirectory = "~/App_Data/DbBackups/";
        public string DbBakDirectory
        {
            get { return _dbBakDirectory; }
            set { _dbBakDirectory = value; }
        }
        public const string VersionFormat = "yyyy-MM-dd";
        public static string Version()
        {
            return DateTime.Now.ToString(DbBackupService.VersionFormat, CultureInfo.InvariantCulture);
        }
        private const string FullName = " FULL";
        private const string DifferentialName = " (dif)";
        public string CreateDbBackup(bool differential = false)
        {
            //Environment.MachineName
            string outputFileName = Path.Combine(HostingEnvironment.MapPath(DbBakDirectory), Version() + (differential ? DifferentialName : FullName));
            
            SqlConnectionStringBuilder conStr = new SqlConnectionStringBuilder();
            using (DbContext context = new DataContext())
            {
                conStr.ConnectionString = context.Database.Connection.ConnectionString;
                string version = SqlVersion(context.Database).ToLower();
                if (version.Contains("compact"))
                {
                    string inputFileName = conStr.DataSource.Replace("|DataDirectory|", HostingEnvironment.MapPath("~/App_Data/"));
                    System.IO.File.Copy(inputFileName, outputFileName + ".sdf");
                }
                else
                {
                    BackupDataBase(context.Database, conStr.InitialCatalog, outputFileName, differential);
                }
            }
            return outputFileName;
        }
        public string GetFullPath(string fileName)
        {
            string fullPath = Path.Combine(HostingEnvironment.MapPath(DbBakDirectory), fileName);
            if (!System.IO.File.Exists(fullPath))
            {
                _validatonDictionary.AddError(fileName, "Could not be found.");
                return null;
            }
            return fullPath;
        }
        public Stream zipDbFiles(IEnumerable<string> selectedFileNames)
        {
            if (selectedFileNames == null || !selectedFileNames.Any())
            {
                _validatonDictionary.AddError("SelectedFileNames", "At least 1 file must be selected");
                return null;
            }
            MemoryStream outputStream = new MemoryStream();
            using (ZipFile zip = new ZipFile())
            {
                foreach (string fileName in selectedFileNames)
                {
                    zip.AddFile(fileName,"");
                }
                zip.Save(outputStream);
            }
            return outputStream;
        }
        public IEnumerable<AutomatedBakInfo> GetAllBackupNames()
        {
            var suffix = BackupFileTypes;
            return (new DirectoryInfo(HostingEnvironment.MapPath(DbBakDirectory)))
                        .EnumerateFiles()
                        .Where(f=>f.Name!="Elmah.sdf" && suffix.Any(s=>f.Name.EndsWith(s)))
                        .Select(f => new AutomatedBakInfo { 
                            FileName = f.Name, 
                            Size = f.Length / 1000,
                            LastModified = f.LastWriteTime});
        }
        public string Fullpath(string fileName)
        {
            return Path.Combine(HostingEnvironment.MapPath(DbBakDirectory), fileName);
        }
        public HttpStatusCode DeleteDbBackup(string fileName)
        {
            fileName = Fullpath(fileName);
            if (!System.IO.File.Exists(fileName)) { return HttpStatusCode.NotFound; }
            System.IO.File.Delete(fileName);
            return HttpStatusCode.OK;
        }
        //http://cavedweller92.wordpress.com/2012/12/13/create-backup-restoring-database-in-mssql-asp-net-c-2/
        //
        public static int BackupDataBase(Database db, string dbName, string outputPath, bool differential=false)
        {
            if (!outputPath.EndsWith(".bak")) { outputPath += ".bak"; }
            int sqlReturn = db.ExecuteSqlCommand(
                    "BACKUP DATABASE [" + dbName + "] TO  DISK = N'" + outputPath + "' WITH " + (differential ? "DIFFERENTIAL," : "") + " CHECKSUM");
            if (sqlReturn != -1) { return sqlReturn; }
            return db.ExecuteSqlCommand("RESTORE VERIFYONLY FROM  DISK = N'" + outputPath + "'");
        }
        public static string SqlVersion(Database db)
        {
            try
            {
                return db.SqlQuery(typeof(string), "SELECT @@version").Cast<string>().FirstOrDefault();
            }
            catch (System.Data.SqlServerCe.SqlCeException)
            {
                return "Sql Server Compact Edition (version unspecified)"; // could return the data provider, ie 'Data Provider for Microsoft SQL Server Compact' with e.Source
            }
        }
    }
}