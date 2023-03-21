//-----------------------------------------------------------------------
// <copyright file="DatabaseBackup.cs" company="www.pta.de">
//     Class: DatabaseBackup
//     Copyright © www.pta.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.pta.de</author>
// <email>gerhard.ahrens@pta.de</email>
// <date>01.06.2022 14:14:36</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using EasyPrototyping.Core;

    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using EasyPrototypingNET.Pattern;
    using System.Runtime.Versioning;

    [SupportedOSPlatform("windows")]
    public sealed class DatabaseBackup : DisposableBase
    {
        private const string FILE_PATTERN = "*.eff";
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseBackup"/> class.
        /// </summary>
        public DatabaseBackup()
        {
            this.BackupFolder = this.CreateBackupFolder();

            using (LocalSettingsManager sm = new LocalSettingsManager())
            {
                this.Fullname = sm.Fullname;
                this.DatabaseFileBackup = sm.DatabaseBackup;
                this.MaxBackupFile = sm.MaxBackupFile;
            }
        }

        private string Fullname { get; set; }

        private string BackupFolder { get; set; }

        private bool DatabaseFileBackup { get; set; }

        private int MaxBackupFile { get; set; }

        public void CheckAndRun()
        {
            if (this.DatabaseFileBackup == false || this.MaxBackupFile == 0)
            {
                return;
            }

            try
            {
                DirectoryInfo directory = new DirectoryInfo(this.BackupFolder);
                if (directory.IsNullOrEmpty() == false)
                {
                    IEnumerable<FileInfo> files = directory.EnumerateFiles(FILE_PATTERN);
                    if (files.IsNullOrEmpty() == false)
                    {
                        if (files.CountEx() > this.MaxBackupFile)
                        {
                            this.CopyDatabase(this.Fullname, directory);
                            this.DeleteDatabase(files);
                        }
                        else
                        {
                            this.CopyDatabase(this.Fullname, directory);
                        }
                    }
                    else
                    {
                        this.CopyDatabase(this.Fullname, directory);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        public string BackupInfo()
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(BackupFolder) == true)
            {
                return string.Empty;
            }

            try
            {
                DirectoryInfo directory = new DirectoryInfo(this.BackupFolder);
                if (directory.IsNullOrEmpty() == false)
                {
                    IEnumerable<FileInfo> files = directory.EnumerateFiles(FILE_PATTERN);
                    if (files.IsNullOrEmpty() == false)
                    {
                        IEnumerable<FileInfo> filesSort = files.OrderByDescending(f => f.CreationTime);
                        FileInfo lastFile = filesSort.FirstOrDefault();
                        result = $"{lastFile.CreationTime}, {lastFile.FullName}";
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                result = "Keine Backupdatei gefunden.";
            }

            return result;
        }

        protected override void DisposeManagedResources()
        {
            /* Behandeln von Managed Resources bem verlassen der Klasse */
        }

        protected override void DisposeUnmanagedResources()
        {
            /* Behandeln von UnManaged Resources bem verlassen der Klasse */
        }

        private void CopyDatabase(string databaseName, DirectoryInfo backupDirectory)
        {
            DateTime copyFiledate = DateTime.Now;
            string tempBackupName = this.CreateBackupFile(databaseName, backupDirectory);
            File.Copy(databaseName, tempBackupName,true);
            FileInfo fi = new FileInfo(tempBackupName);
            fi.CreationTime = copyFiledate;
            fi.LastWriteTime = copyFiledate;
        }

        private void DeleteDatabase(IEnumerable<FileInfo> files)
        {
            IEnumerable<FileInfo> filesSort = files.OrderBy(f => f.LastWriteTime);
            IEnumerable<FileInfo> filesMax = filesSort.Take(files.Count() - this.MaxBackupFile);
            foreach (FileInfo file in filesMax)
            {
                File.Delete(file.FullName);
            }
        }

        private string CreateBackupFile(string databaseName, DirectoryInfo backupDirectory)
        {
            this.BackupFolder = $"{this.BackupFolder}\\{Path.GetFileName(databaseName)}";
            DecomposedFilePath testPath = new DecomposedFilePath(this.BackupFolder);
            IEnumerable<FileInfo> files = backupDirectory.EnumerateFiles(FILE_PATTERN);
            DecomposedFilePath nextFile = testPath.GetFirstFreeFilePath(files);

            return nextFile.FullFilePath;
        }

        private string CreateBackupFolder()
        {
            string backupPath = string.Empty;

            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            backupPath = $"{rootPath}\\{this.ApplicationName()}\\Backup";

            if (string.IsNullOrEmpty(backupPath) == false)
            {
                try
                {
                    if (Directory.Exists(backupPath) == false)
                    {
                        Directory.CreateDirectory(backupPath);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return backupPath;
        }

        private string ApplicationName()
        {
            string result = string.Empty;

            Assembly assm = Assembly.GetEntryAssembly();
            result = assm.GetName().Name;
            return result;
        }
    }
}
