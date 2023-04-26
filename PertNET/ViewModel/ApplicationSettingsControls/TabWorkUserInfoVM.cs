//-----------------------------------------------------------------------
// <copyright file="TabWorkUserInfoVM.cs" company="Lifeprojects.de">
//     Class: TabWorkUserInfoVM
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

namespace PERT.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Versioning;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.Pattern;
    using EasyPrototypingNET.WPF;

    using PERT.Core;

    using PertNET.Core;
    using PertNET.DataRepository;
    using PertNET.Model;

    [SupportedOSPlatform("windows")]
    public class TabWorkUserInfoVM : ViewModelBase<TabWorkUserInfoVM>, IViewModel, IDataErrorInfo
    {
        private readonly Dictionary<string, Func<Result<string>>> validationDelegates = new Dictionary<string, Func<Result<string>>>();
        private readonly Dictionary<string, string> validErrors = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TabWorkUserInfoVM"/> class.
        /// </summary>
        public TabWorkUserInfoVM()
        {
            using (LocalSettingsManager sm = new LocalSettingsManager())
            {
                this.Databasefile = sm.Fullname;
            }

            this.InitCommands();
            this.RegisterValidations();

            if (string.IsNullOrEmpty(this.Databasefile) == false)
            {
                this.LoadDataHandler();
            }
        }

        #region Get/Set Properties
        [PropertyBinding]
        public WorkUserInfo CurrentSelectedItem
        {
            get { return this.Get<WorkUserInfo>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string Firstname
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string Lastname
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string Email
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string UserId
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string Company
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string ProjectName
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string Description
        {
            get { return this.Get<string>(); }
            set { this.Set(value, this.CheckContent); }
        }

        [PropertyBinding]
        public DateTime LastDate
        {
            get { return this.Get<DateTime>(); }
            set { this.Set(value, this.CheckContent); }
        }

        [PropertyBinding]
        public int ErrorsCount
        {
            get { return this.Get<int>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public bool HasValidationsErrors
        {
            get { return this.Get<bool>(); }
            set { this.Set(value); }
        }

        private string Databasefile { get; set; }

        private bool IsDirty { get; set; }

        public string this[string propName]
        {
            get
            {
                Func<Result<string>> function = null;
                if (validationDelegates.TryGetValue(propName, out function) == true)
                {
                    Result<string> ruleText = this.DoValidation(function, propName);
                    if (string.IsNullOrEmpty(ruleText.Value) == false)
                    {
                        this.HasValidationsErrors = (bool)ruleText.ResultState;
                        validErrors.AddIfNotExists(propName, ruleText.Value);
                        return ruleText.Value;
                    }
                    else
                    {
                        validErrors.DeleteIfExistsKey(propName);
                        this.HasValidationsErrors = false;
                        return string.Empty;
                    }
                }
                else
                {
                    validErrors.DeleteIfExistsKey(propName);
                    this.HasValidationsErrors = false;
                    return string.Empty;
                }
            }
        }
        #endregion Get/Set Properties

        protected override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("SaveCommand", new RelayCommand(p1 => this.SaveHandler(), p2 => this.CanSaveHandler()));
        }

        #region Load and Filter Data
        private void LoadDataHandler()
        {
            try
            {
                using (WorkUserInfoRepository repository = new WorkUserInfoRepository(this.Databasefile))
                {
                    this.CurrentSelectedItem = repository.List().FirstOrDefault();
                    if (this.CurrentSelectedItem != null)
                    {
                        this.ShowData();
                    }
                    else
                    {
                        this.CurrentSelectedItem = new WorkUserInfo();
                        this.CurrentSelectedItem.UserId = UserInfo.TS().CurrentDomainUser;
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void ShowData()
        {
            this.Firstname = this.CurrentSelectedItem.Firstname;
            this.Lastname = this.CurrentSelectedItem.Lastname;
            this.Email = this.CurrentSelectedItem.Email;
            this.UserId = this.CurrentSelectedItem.UserId;
            this.Company = this.CurrentSelectedItem.Company;
            this.ProjectName = this.CurrentSelectedItem.Project;
            this.Description = this.CurrentSelectedItem.Description;

            if (string.IsNullOrEmpty(this.CurrentSelectedItem.UserId) == true)
            {
                this.UserId = UserInfo.TS().CurrentDomainUser;
            }

            this.LastDate = this.CurrentSelectedItem.StartDate;
        }
        #endregion Load and Filter Data

        private bool CanSaveHandler()
        {
            return true;
        }

        private void SaveHandler()
        {
            using (WorkUserInfoRepository repository = new WorkUserInfoRepository(this.Databasefile))
            {
                this.CurrentSelectedItem.Firstname = this.Firstname;
                this.CurrentSelectedItem.Lastname = this.Lastname;
                this.CurrentSelectedItem.Email = this.Email;
                this.CurrentSelectedItem.UserId = this.UserId;
                this.CurrentSelectedItem.Company = this.Company;
                this.CurrentSelectedItem.Project = this.ProjectName;
                this.CurrentSelectedItem.Description = this.Description;
                this.CurrentSelectedItem.StartDate = this.LastDate;

                if (repository.List().Any() == true)
                {
                    this.CurrentSelectedItem.ModifiedBy = UserInfo.TS().CurrentUser;
                    this.CurrentSelectedItem.ModifiedOn = UserInfo.TS().CurrentTime;
                    repository.Update(this.CurrentSelectedItem);
                }
                else
                {
                    this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentUser;
                    this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                    repository.Add(this.CurrentSelectedItem);
                }
            }
        }

        #region Validierung

        private void RegisterValidations()
        {
            this.validationDelegates.Add(nameof(this.Firstname), () =>
            {
                return WorkUserInfoValidation<TabWorkUserInfoVM>.This(this).NotEmpty(x => x.Firstname, "Vorname");
            });
        }

        #endregion Validierung

        private void CheckContent<T>(T value, string propertyName)
        {
            if (this.CurrentSelectedItem == null)
            {
                return;
            }

            PropertyInfo propInfo = this.CurrentSelectedItem.GetType().GetProperties().FirstOrDefault(p => p.Name == propertyName);
            if (propInfo == null)
            {
                this.IsDirty = false;
                return;
            }

            var propValue = propInfo.GetValue(this.CurrentSelectedItem);
            if (propValue == null)
            {
                this.IsDirty = true;
                return;
            }

            if (propValue.Equals(value) == false)
            {
                this.IsDirty = true;
            }
        }
    }
}
