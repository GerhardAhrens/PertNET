//-----------------------------------------------------------------------
// <copyright file="TabStatisticsInfoVM.cs" company="Lifeprojects.de">
//     Class: TabStatisticsInfoVM
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>19.07.2022 15:15:48</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Versioning;

    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;

    using PertNET.Core;
    using PertNET.DataRepository;

    [SupportedOSPlatform("windows")]
    public class TabStatisticsInfoVM : ViewModelBase<TabStatisticsInfoVM>, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabStatisticsInfoVM"/> class.
        /// </summary>
        public TabStatisticsInfoVM()
        {
            this.InitCommands();
            this.LoadDataHandler();
        }

        public string Databasefile
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public DateTime LastAccess
        {
            get { return this.Get<DateTime>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string LastBackupInfo
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public int CountBackupDatabase
        {
            get { return this.Get<int>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public int CountAll
        {
            get { return this.Get<int>(); }
            set { this.Set(value); }
        }

        protected override void InitCommands()
        {
        }

        #region Load and Filter Data
        private void LoadDataHandler()
        {
            try
            {
                using (LocalSettingsManager sm = new LocalSettingsManager())
                {
                    this.Databasefile = sm.Fullname;
                    this.CountBackupDatabase = sm.MaxBackupFile;
                }

                using (EffortProjectRepository repository = new EffortProjectRepository(this.Databasefile))
                {
                    this.CountAll = repository.List().Count();
                    this.LastAccess = repository.List().Max(m => m.ModifiedOn);
                }

                using (DatabaseBackup backup = new DatabaseBackup())
                {
                    this.LastBackupInfo = backup.BackupInfo();
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }
        #endregion Load and Filter Data

        private bool CanSaveHandler()
        {
            return true;
        }
    }
}
