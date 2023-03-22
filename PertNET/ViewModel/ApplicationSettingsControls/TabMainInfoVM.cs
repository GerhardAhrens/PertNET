//-----------------------------------------------------------------------
// <copyright file="TabMainInfoVM.cs" company="Lifeprojects.de">
//     Class: TabMainInfoVM
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
    using System.Runtime.Versioning;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.WPF;

    using PertNET.Core;

    [SupportedOSPlatform("windows")]
    public class TabMainInfoVM : ViewModelBase<TabMainInfoVM>, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabMainInfoVM"/> class.
        /// </summary>
        public TabMainInfoVM()
        {
            this.InitCommands();
            this.LoadDataHandler();
        }

        [PropertyBinding]
        public bool ExitQuestion
        {
            get { return this.Get<bool>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public bool ApplicationPosition
        {
            get { return this.Get<bool>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public bool IsBackupDatabase
        {
            get { return this.Get<bool>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public int CountBackupDatabase
        {
            get { return this.Get<int>(); }
            set { this.Set(value); }
        }

        protected override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("SaveCommand", new RelayCommand(p1 => this.SaveHandler(), p2 => true));
        }

        #region Load and Filter Data
        private void LoadDataHandler()
        {
            try
            {
                using (LocalSettingsManager sm = new LocalSettingsManager())
                {
                    this.ExitQuestion = sm.ExitQuestion;
                    this.ApplicationPosition = sm.ApplicationPosition;
                    this.IsBackupDatabase = sm.DatabaseBackup;
                    this.CountBackupDatabase = sm.MaxBackupFile;
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }
        #endregion Load and Filter Data

        private void SaveHandler()
        {
            using (LocalSettingsManager sm = new LocalSettingsManager())
            {
                sm.ExitQuestion = this.ExitQuestion;
                sm.ApplicationPosition = this.ApplicationPosition;
                sm.DatabaseBackup = this.IsBackupDatabase;
                sm.MaxBackupFile = this.CountBackupDatabase;
            }
        }

    }
}
