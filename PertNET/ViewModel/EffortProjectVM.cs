//-----------------------------------------------------------------------
// <copyright file="EffortProjectVM.cs" company="Lifeprojects.de">
//     Class: EffortProjectVM
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>01.07.2022 16:24:34</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------


namespace PERT.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Media;

    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.ExceptionHandling;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.Pattern;
    using EasyPrototypingNET.WPF;

    using PertNET.Core;
    using PertNET.DataRepository;
    using PertNET.Model;

    [SupportedOSPlatform("windows")]
    public class EffortProjectVM : ViewModelBase<EffortProjectVM>, IViewModel, IDataErrorInfo
    {
        private const string DOUBLEF2 = "0.00";
        private const string NOCOLORSELECTED = "Transparent";
        private readonly Dictionary<string, Func<Result<string>>> validationDelegates = new Dictionary<string, Func<Result<string>>>();
        //private readonly Dictionary<string, string> validErrors = new Dictionary<string, string>();

        public EffortProjectVM(Guid id, bool subItem, bool copyItem = false)
        {
            this.currentId = id;
            this.IsCopyItem = copyItem;
            this.AddSubItem = subItem;

            using (LocalSettingsManager sm = new LocalSettingsManager())
            {
                this.Databasefile = sm.Fullname;
            }

            this.InitCommands();
            this.RegisterValidations();
            this.LoadDataHandler();
        }

        #region Get/Set Properties
        [PropertyBinding]
        public EffortProject CurrentSelectedItem
        {
            get { return this.Get<EffortProject>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public int ChapterA
        {
            get { return this.Get<int>(); }
            set { this.Set(value, this.CheckContent); }
        }

        [PropertyBinding]
        public int ChapterB
        {
            get { return this.Get<int>(); }
            set { this.Set(value, this.CheckContent); }
        }

        [PropertyBinding]
        public int ChapterC
        {
            get { return this.Get<int>(); }
            set { this.Set(value, this.CheckContent); }
        }

        [PropertyBinding]
        public string Title
        {
            get { return this.Get<string>(); }
            set { this.Set(value, this.CheckContent); }
        }

        [PropertyBinding]
        public string Description
        {
            get { return this.Get<string>(); }
            set { this.Set(value, this.CheckContent); }
        }

        [PropertyBinding]
        public bool ShowDescription
        {
            get { return this.Get<bool>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public int BackgroundColorSelected
        {
            get { return this.Get<int>(); }
            set { this.Set(value, this.CheckContent); }
        }

        [PropertyBinding]
        public string MinEffort
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MidEffort
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MaxEffort
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MinFullEffort
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MidFullEffort
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MaxFullEffort
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public Dictionary<double,string> FactorSource
        {
            get { return this.Get<Dictionary<double, string>>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public double FactorSelected
        {
            get { return this.Get<double>(); }
            set { this.Set(value); }
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

        public string TagText { get; set; }

        private Guid currentId { get; set; }

        private bool IsCopyItem { get; set; }

        private bool AddSubItem { get; set; }

        private string Databasefile { get; set; }

        private bool IsDirty { get; set; }


        public string this[string propName]
        {
            get
            {
                Func<Result<string>> function = null;
                if (validationDelegates.TryGetValue(propName, out function) == true)
                {
                    this.HasValidationsErrors = this.CounFieldError();

                    Result<string> ruleText = this.DoValidation(function, propName);
                    if (string.IsNullOrEmpty(ruleText.Value) == false)
                    {
                        this.HasValidationsErrors = (bool)ruleText.ResultState;
                        return ruleText.Value;
                    }
                    else
                    {
                        return ruleText.Value;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        #endregion Get/Set Properties

        protected override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("DialogCloseCommand", new RelayCommand(p1 => this.DialogCloseHandler(), p2 => true));
            this.CmdAgg.AddOrSetCommand("SaveCommand", new RelayCommand(p1 => this.SaveHandler(), p2 => this.CanSaveHandler()));
            this.CmdAgg.AddOrSetCommand("HelpCommand", new RelayCommand(p1 => this.HelpHandler(), p2 => true));
        }

        #region Load and Filter Data
        private void LoadDataHandler()
        {
            try
            {
                using (EffortFactors factors = new EffortFactors())
                {
                    this.FactorSource = factors.Get();
                }

                this.FactorSelected = 1.00;

                if (this.currentId != Guid.Empty)
                {
                    using (EffortProjectRepository repository = new EffortProjectRepository(this.Databasefile))
                    {
                        this.CurrentSelectedItem = repository.ListById(this.currentId);
                        if (this.CurrentSelectedItem != null)
                        {
                            this.CurrentSelectedItem.ChapterInsert = false;
                            this.ShowData();
                        }
                    }
                }
                else
                {
                    int maxChapterA = 0;
                    using (EffortProjectRepository repository = new EffortProjectRepository(this.Databasefile))
                    {
                        if (this.AddSubItem == false)
                        {
                            var isDataFound = repository.List().Any();
                            if (isDataFound == true)
                            {
                                maxChapterA = repository.List().ToList().Max(s => s.ChapterA) + 1;
                            }
                            else
                            {
                                maxChapterA = 1;
                            }
                        }
                        else
                        {
                            maxChapterA = 0;
                        }
                    }

                    this.ChapterA = maxChapterA;
                    this.CurrentSelectedItem = new EffortProject();
                    this.CurrentSelectedItem.ModifiedBy = UserInfo.TS().CurrentDomainUser;
                    this.CurrentSelectedItem.ModifiedOn = UserInfo.TS().CurrentTime;
                    if (this.AddSubItem == true)
                    {
                        this.CurrentSelectedItem.ChapterInsert = true;
                    }
                    else
                    {
                        this.CurrentSelectedItem.ChapterInsert = false;
                    }

                    this.BackgroundColorSelected = this.ConvertColorNameToIndex(NOCOLORSELECTED);
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        private void ShowData()
        {
            this.ChapterA = this.CurrentSelectedItem.ChapterA;
            this.ChapterB = this.CurrentSelectedItem.ChapterB;
            this.ChapterC = this.CurrentSelectedItem.ChapterC;
            this.Title = this.CurrentSelectedItem.Title;
            this.Description = this.CurrentSelectedItem.Description;
            this.ShowDescription = this.CurrentSelectedItem.ShowDescription;
            this.BackgroundColorSelected = this.ConvertColorNameToIndex(this.CurrentSelectedItem.BackgroundColor);
            this.MinEffort = this.CurrentSelectedItem.Min.ToString(DOUBLEF2);
            this.MidEffort = this.CurrentSelectedItem.Mid.ToString(DOUBLEF2);
            this.MaxEffort = this.CurrentSelectedItem.Max.ToString(DOUBLEF2);
            this.FactorSelected = this.CurrentSelectedItem.Factor;
            this.TagText = this.CurrentSelectedItem.Tag;

            using (EffortProjectRepository repository = new EffortProjectRepository(this.Databasefile))
            {
                Tuple<double, double, double>  effortFullSum = repository.BuildFullSum();
                this.MinFullEffort = effortFullSum.Item1.ToString(DOUBLEF2);
                this.MidFullEffort = effortFullSum.Item2.ToString(DOUBLEF2);
                this.MaxFullEffort = effortFullSum.Item3.ToString(DOUBLEF2);
            }

            if (string.IsNullOrEmpty(this.TagText) == false)
            {
                this.EventAgg.Publish<TagOutEventArgs<IViewModel>>(new TagOutEventArgs<IViewModel> 
                { 
                    Sender = this as IViewModel,
                    Text = this.TagText 
                });
            }

            this.IsDirty = false;
        }

        #endregion Load and Filter Data

        private void DialogCloseHandler()
        {
            if (this.IsDirty == true)
            {
                DialogResultsEx dialogResult = AppMsgDialog.LastChangedNotSave();
                if (dialogResult == DialogResultsEx.Yes)
                {
                    Window currentWindow = Application.Current.Windows.LastActiveWindow();
                    if (currentWindow != null)
                    {
                        if (currentWindow.IsActive == true)
                        {
                            currentWindow.DialogResult = true;
                            currentWindow.Tag = this.CurrentSelectedItem;
                            currentWindow.Close();
                        }
                    }
                }
            }
            else
            {
                Window currentWindow = Application.Current.Windows.LastActiveWindow();
                if (currentWindow != null)
                {
                    if (currentWindow.IsActive == true)
                    {
                        currentWindow.DialogResult = true;
                        currentWindow.Tag = this.CurrentSelectedItem;
                        currentWindow.Close();
                    }
                }
            }
        }

        private bool CanSaveHandler()
        {
            return this.HasValidationsErrors == true ? false : true;
        }

        private void SaveHandler()
        {
            try
            {
                if (this.CurrentSelectedItem != null)
                {
                    this.EventAgg.Publish<TagInEventArgs<IViewModel>>(new TagInEventArgs<IViewModel>
                    {
                        Sender = this as IViewModel,
                        Text = string.Empty
                    });

                    EffortProject original = EffortProject.ToClone(this.CurrentSelectedItem);

                    this.CurrentSelectedItem.ChapterA = this.ChapterA;
                    this.CurrentSelectedItem.ChapterB = this.ChapterB;
                    this.CurrentSelectedItem.ChapterC = this.ChapterC;
                    this.CurrentSelectedItem.Title = this.Title;
                    this.CurrentSelectedItem.Description = this.Description;
                    this.CurrentSelectedItem.ShowDescription = this.ShowDescription;
                    this.CurrentSelectedItem.BackgroundColor = this.ConvertIndexToColorName(this.BackgroundColorSelected);
                    this.CurrentSelectedItem.Min = Convert.ToDouble(this.MinEffort);
                    this.CurrentSelectedItem.Mid = Convert.ToDouble(this.MidEffort);
                    this.CurrentSelectedItem.Max = Convert.ToDouble(this.MaxEffort);
                    this.CurrentSelectedItem.Factor = this.FactorSelected;
                    this.CurrentSelectedItem.Tag = this.TagText;

                    using (EffortProjectRepository repository = new EffortProjectRepository(this.Databasefile))
                    {
                        if (this.CurrentSelectedItem.Id == Guid.Empty && this.IsCopyItem == false)
                        {
                            if (this.CurrentSelectedItem.ChapterInsert == false)
                            {
                                if (repository.ExistChapter(this.ChapterA, this.ChapterB, this.ChapterC) == true)
                                {
                                    AppMsgDialog.ChapterIsFound(this.CurrentSelectedItem.Chapter);
                                    return;
                                }
                            }

                            this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentUser;
                            this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                            this.CurrentSelectedItem.Id = Guid.NewGuid();
                            repository.Add(this.CurrentSelectedItem);
                            this.IsDirty = false;
                            this.DialogCloseHandler();
                        }
                        else if (this.CurrentSelectedItem.Id != Guid.Empty && this.IsCopyItem == false)
                        {
                            if (original.Chapter != this.CurrentSelectedItem.Chapter)
                            {
                                if (repository.ExistChapter(this.ChapterA, this.ChapterB, this.ChapterC) == true)
                                {
                                    AppMsgDialog.ChapterIsFound(this.CurrentSelectedItem.Chapter);
                                    return;
                                }
                            }

                            this.CurrentSelectedItem.ModifiedBy = UserInfo.TS().CurrentUser;
                            this.CurrentSelectedItem.ModifiedOn = UserInfo.TS().CurrentTime;
                            repository.Update(this.CurrentSelectedItem);
                            this.IsDirty = false;
                            this.DialogCloseHandler();
                        }
                        else if (this.CurrentSelectedItem.Id != Guid.Empty && this.IsCopyItem == true)
                        {
                            if (repository.ExistChapter(this.ChapterA, this.ChapterB, this.ChapterC) == true)
                            {
                                AppMsgDialog.ChapterIsFound(this.CurrentSelectedItem.Chapter);
                                return;
                            }

                            this.CurrentSelectedItem.CreatedBy = UserInfo.TS().CurrentUser;
                            this.CurrentSelectedItem.CreatedOn = UserInfo.TS().CurrentTime;
                            this.CurrentSelectedItem.Id = Guid.NewGuid();
                            repository.Add(this.CurrentSelectedItem);
                            this.IsDirty = false;
                            this.DialogCloseHandler();
                        }
                    }

                    using (WorkUserInfoRepository repository = new WorkUserInfoRepository(this.Databasefile))
                    {
                        WorkUserInfo wi = repository.List().First();
                        wi.StartDate = DateTime.Now;
                        repository.Update(wi);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        private void HelpHandler()
        {
            HelpViewOption options = new HelpViewOption();
            options.HeaderText = "PERT (Effort Tool)";
            options.HelptextFile = $"Help_{this.GetType().Name}.md";
            bool? result = HelpView.Show(options);
        }

        #region Validierung

        private void RegisterValidations()
        {
            this.validationDelegates.Add(nameof(this.Title), () =>
            {
                return EffortProjectValidation<EffortProjectVM>.This(this).NotEmpty(x => x.Title, "Titel");
            });

            this.validationDelegates.Add(nameof(this.ChapterA), () =>
            {
                return EffortProjectValidation<EffortProjectVM>.This(this).InRangeChapter(x => x.ChapterA, 1,999);
            });

            this.validationDelegates.Add(nameof(this.ChapterB), () =>
            {
                return EffortProjectValidation<EffortProjectVM>.This(this).InRangeChapter(x => x.ChapterB, 0, 999);
            });

            this.validationDelegates.Add(nameof(this.ChapterC), () =>
            {
                return EffortProjectValidation<EffortProjectVM>.This(this).InRangeChapter(x => x.ChapterC, 0, 999);
            });

            this.validationDelegates.Add(nameof(this.MinEffort), () =>
            {
                return EffortProjectValidation<EffortProjectVM>.This(this).ValueGreaterThanZero(x => x.MinEffort, 0);
            });

            this.validationDelegates.Add(nameof(this.MidEffort), () =>
            {
                return EffortProjectValidation<EffortProjectVM>.This(this).ValueGreaterThanZero(x => x.MidEffort, 0);
            });

            this.validationDelegates.Add(nameof(this.MaxEffort), () =>
            {
                return EffortProjectValidation<EffortProjectVM>.This(this).ValueGreaterThanZero(x => x.MaxEffort, 0);
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

        private bool CounFieldError()
        {
            int countError = 0;
            foreach (var item in validationDelegates)
            {
                Result<string> ruleResult = item.Value.Invoke();
                if (ruleResult.Value.IsEmpty() == false)
                {
                    countError++;
                }
            }

            return countError.ToBool();
        }


        private int ConvertColorNameToIndex(string colorName)
        {
            PropertyInfo[] colors = typeof(Brushes).GetProperties();
            int indexColor = Array.FindIndex(colors, x => x.Name.ToUpper() == colorName.ToUpper());
            return indexColor;
        }

        private string ConvertIndexToColorName(int colorIndex)
        {
            PropertyInfo[] colors = typeof(Brushes).GetProperties();
            string colorName = colors[colorIndex].Name;
            return colorName;
        }

        private Brush ConvertIndexToBrush(int colorIndex)
        {
            PropertyInfo[] colors = typeof(Brushes).GetProperties();
            string colorName = colors[colorIndex].Name;
            Color col = (Color)ColorConverter.ConvertFromString(colorName);
            Brush brushColor = new SolidColorBrush(col);
            return brushColor;
        }
    }
}
