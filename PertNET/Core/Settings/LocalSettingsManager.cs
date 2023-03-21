
//-----------------------------------------------------------------------
// <copyright file="LocalSettingsManager.cs" company="Lifeprojects.de">
//     Class: LocalSettingsManager
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>27.06.2022</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System;
    using System.IO;
    using System.Runtime.Versioning;

    using EasyPrototypingNET.Pattern;

    [SupportedOSPlatform("windows")]
    public sealed class LocalSettingsManager : DisposableBase
    {
        private LocalSettings localSettings = null;
        private string database = string.Empty;
        private string databasePath = string.Empty;
        private DateTime lastAccess;
        private string lastUser = string.Empty;
        private bool exitQuestion;
        private bool applicationPosition;
        private bool databaseBackup;
        private int maxBackupFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalSettingsManager"/> class.
        /// </summary>
        public LocalSettingsManager()
        {
            this.localSettings = new LocalSettings();
            if (this.localSettings != null)
            {
                this.InitSetting();
            }
        }

        public string Database
        {
            get { return this.database; }
            set
            {
                this.database = value;
                this.localSettings.AddOrSet("Database", typeof(string), this.database);
                this.localSettings.Save();
            }
        }

        public string DatabasePath
        {
            get { return this.databasePath; }
            set
            {
                this.databasePath = value;
                this.localSettings.AddOrSet("Path", typeof(string), this.databasePath);
                this.localSettings.Save();
            }
        }

        public string Fullname { get; private set; } = string.Empty;

        public DateTime LastAccess
        {
            get { return this.lastAccess; }
            set
            { 
                this.lastAccess = value;
                this.localSettings.AddOrSet("LastAccess", typeof(DateTime), this.lastAccess);
                this.localSettings.Save();
            }
        }

        public string LastUser
        {
            get { return this.lastUser; }
            set
            {
                this.lastUser = value;
                this.localSettings.AddOrSet("LastUser", typeof(string), this.lastUser);
                this.localSettings.Save();
            }
        }

        public bool ExitQuestion
        {
            get { return this.exitQuestion; }
            set
            {
                this.exitQuestion = value;
                this.localSettings.AddOrSet("ExitQuestion", typeof(bool), this.exitQuestion);
                this.localSettings.Save();
            }
        }

        public bool ApplicationPosition
        {
            get { return this.applicationPosition; }
            set
            {
                this.applicationPosition = value;
                this.localSettings.AddOrSet("ApplicationPosition", typeof(bool), this.applicationPosition);
                this.localSettings.Save();
            }
        }

        public bool DatabaseBackup
        {
            get { return this.databaseBackup; }
            set
            {
                this.databaseBackup = value;
                this.localSettings.AddOrSet("DatabaseBackup", typeof(bool), this.databaseBackup);
                this.localSettings.Save();
            }
        }

        public int MaxBackupFile
        {
            get { return this.maxBackupFile; }
            set
            {
                this.maxBackupFile = value;
                this.localSettings.AddOrSet("MaxBackupFile", typeof(int), this.maxBackupFile);
                this.localSettings.Save();
            }
        }

        protected override void DisposeManagedResources()
        {
            /* Behandeln von Managed Resources bem verlassen der Klasse */
            if (this.localSettings != null)
            {
                this.localSettings = null;
            }
        }

        protected override void DisposeUnmanagedResources()
        {
            /* Behandeln von UnManaged Resources bem verlassen der Klasse */
        }

        private void InitSetting()
        {
            if (this.localSettings.IsExitSettings() == false)
            {
                this.localSettings.AddOrSet("Database", typeof(string), string.Empty);
                this.localSettings.AddOrSet("Path", typeof(string), string.Empty);
                this.localSettings.AddOrSet("SettingFilename", typeof(string), localSettings.Filename);
                this.localSettings.AddOrSet("Timeout", typeof(int), 60);
                this.localSettings.AddOrSet("LastAccess", typeof(DateTime), DateTime.Now.DefaultDate());
                this.localSettings.AddOrSet("LastUser", typeof(string), string.Empty);
                this.localSettings.AddOrSet("ExitQuestion", typeof(bool), false);
                this.localSettings.AddOrSet("ApplicationPosition", typeof(bool), false);
                this.localSettings.AddOrSet("DatabaseBackup", typeof(bool), false);
                this.localSettings.AddOrSet("MaxBackupFile", typeof(int), 5);
                this.localSettings.Save();
            }
            else
            {
                this.localSettings.Load();
            }

            if (localSettings.Exists("Path") == true)
            {
                if (localSettings["Path"] != null)
                {
                    this.DatabasePath = localSettings["Path"].ToString();
                }
                else
                {
                    this.DatabasePath = string.Empty;
                }
            }

            if (localSettings.Exists("Database") == true)
            {
                if (localSettings["Database"] != null)
                {
                    this.Database = localSettings["Database"].ToString();
                }
                else
                {
                    this.Database = string.Empty;
                }
            }

            if (string.IsNullOrEmpty(this.DatabasePath) == false && string.IsNullOrEmpty(this.Database) == false)
            {
                this.Fullname = Path.Combine(this.DatabasePath, this.Database);
            }

            this.LastAccess = localSettings["LastAccess"].ToDateTime();
            if (localSettings.Exists("LastUser") == true)
            {
                this.LastUser = localSettings["LastUser"].ToString();
            }

            if (localSettings.Exists("ExitQuestion") == true)
            {
                this.ExitQuestion = localSettings["ExitQuestion"].ToBool();
            }

            if (localSettings.Exists("ApplicationPosition") == true)
            {
                this.ApplicationPosition = localSettings["ApplicationPosition"].ToBool();
            }

            if (localSettings.Exists("DatabaseBackup") == true)
            {
                this.DatabaseBackup = localSettings["DatabaseBackup"].ToBool();
            }

            if (localSettings.Exists("MaxBackupFile") == true)
            {
                this.MaxBackupFile = localSettings["MaxBackupFile"].ToInt();
            }
        }
    }
}
