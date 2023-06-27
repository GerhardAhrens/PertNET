//-----------------------------------------------------------------------
// <copyright file="TabApplicationInfoVM.cs" company="Lifeprojects.de">
//     Class: TabApplicationInfoVM
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

    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;

    [SupportedOSPlatform("windows")]
    public class TabApplicationInfoVM : ViewModelBase<TabApplicationInfoVM>, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabApplicationInfoVM"/> class.
        /// </summary>
        public TabApplicationInfoVM()
        {
            this.InitCommands();
            this.LoadDataHandler();
        }

        [PropertyBinding]
        public string ApplicationName
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string ApplicationVersion
        {
            get { return this.Get<string>(); }
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
                this.ApplicationName = ApplicationProperties.AssemblyName;
                this.ApplicationVersion = ApplicationProperties.VersionNameBuildDateTime;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }
        #endregion Load and Filter Data
    }
}
