//-----------------------------------------------------------------------
// <copyright file="MainWindowVM.cs" company="Lifeprojects.de">
//     Class: MainWindowVM
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>27.06.2022 14:24:34</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Input;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.Pattern;
    using EasyPrototypingNET.WPF;

    using PertNET;
    using PertNET.Core;
    using PertNET.DataRepository;
    using PertNET.Model;

    [SupportedOSPlatform("windows")]
    //[ViewModel]
    public partial class MainWindowVM : ViewModelBase<MainWindowVM>, IViewModel
    {
        private readonly Window mainWindow = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowVM"/> class.
        /// </summary>
        public MainWindowVM()
        {
            mainWindow = Application.Current.Windows.LastActiveWindow();

            this.InitCommands();
            this.ApplicationVersion = ApplicationProperties.VersionWithName;
            Mouse.OverrideCursor = null;

            using (LocalSettingsManager sm = new LocalSettingsManager())
            {
                this.ExitQuestion = sm.ExitQuestion;
                this.IsDatabaseBackup = sm.DatabaseBackup;
            }

            this.IsFilterContentFound = true;

            if (string.IsNullOrEmpty(App.CmdLineDatabase) == false)
            {
                this.OpenDatabaseFromCmdLine(App.CmdLineDatabase);
            }
        }

        #region Get/Set Properties
        [PropertyBinding]
        public string ApplicationVersion
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string StatuslineDescription
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public bool IsDatabaseOpen
        {
            get { return this.Get<bool>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public bool IsFilterContentFound
        {
            get { return this.Get<bool>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string FilterDefaultSearch
        {
            get { return this.Get<string>(); }
            set { this.Set(value, this.RefreshDefaultFilter); }
        }

        [PropertyBinding]
        public ICollectionView DialogDataView
        {
            get { return this.Get<ICollectionView>(); }
            private set { this.Set(value); }
        }

        [PropertyBinding]
        public EffortProject CurrentSelectedItem
        {
            get { return this.Get<EffortProject>(); }
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
        public string PERTFullEffort
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
        public int SelectedRows
        {
            get { return this.Get<int>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string SelectedRowHeader
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public bool? AllItemsChecked
        {
            get { return this.Get<bool?>(); }
            set { this.Set(value, this.ShowAllItemsChecked); }
        }

        [PropertyBinding]
        public string LastEdit
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public bool IsContextMenuEnabled
        {
            get { return this.Get<bool>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MenuTextAddSub
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MenuTextEdit
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MenuTextDelete
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        [PropertyBinding]
        public string MenuTextCopy
        {
            get { return this.Get<string>(); }
            set { this.Set(value); }
        }

        private string ExportCompanyName { get; set; }

        private string ExportProjectName { get; set; }

        private string CurrentDatabaseFile { get; set; }

        private bool IsDatabaseBackup { get; set; }

        private bool ExitQuestion { get; set; }
        #endregion Get/Set Properties

        protected sealed override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("WindowCloseCommand", new RelayCommand(p1 => this.WindowCloseHandler(), p2 => true));
            this.CmdAgg.AddOrSetCommand("HelpCommand", new RelayCommand(p1 => this.HelpHandler(), p2 => true));

            this.CmdAgg.AddOrSetCommand("CheckBoxCheckedCommand", new RelayCommand(p1 => this.CheckBoxCheckedHandler(p1), p2 => true));
            this.CmdAgg.AddOrSetCommand("AllRowsMarked", new RelayCommand(p1 => this.ShowAllItemsChecked(null), p2 => true));

            this.CmdAgg.AddOrSetCommand("SelectionChangedCommand", new RelayCommand(p1 => this.SelectionChangedHandler(p1), p2 => true));

            this.CmdAgg.AddOrSetCommand("NewDatabaseCommand", new RelayCommand(p1 => this.NewDatabaseHandler(), p2 => true));
            this.CmdAgg.AddOrSetCommand("OpenDatabaseCommand", new RelayCommand(p1 => this.OpenDatabaseHandler(), p2 => true));
            this.CmdAgg.AddOrSetCommand("CloseDatabaseCommand", new RelayCommand(p1 => this.CloseDatabaseHandler(), p2 => this.CanCloseDatabaseHandler()));
            this.CmdAgg.AddOrSetCommand("SaveAsDatabaseCommand", new RelayCommand(p1 => this.SaveAsDatabaseHandler(), p2 => this.CanSaveAsDatabaseHandler()));

            this.CmdAgg.AddOrSetCommand("ClearFilterCommand", new RelayCommand(p1 => this.ClearFilterHandler(), p2 => this.CanClearFilterHandler()));

            this.CmdAgg.AddOrSetCommand("AddEntryCommand", new RelayCommand(p1 => this.AddEntryHandler(), p2 => this.CanAddEntryHandler()));
            this.CmdAgg.AddOrSetCommand("AddSubEntryCommand", new RelayCommand(p1 => this.AddSubEntryHandler(), p2 => this.CanAddSubEntryHandler()));
            this.CmdAgg.AddOrSetCommand("EditEntryCommand", new RelayCommand(p1 => this.EditEntryHandler(), p2 => this.CanEditEntryHandler()));
            this.CmdAgg.AddOrSetCommand("DeleteEntryCommand", new RelayCommand(p1 => this.DeleteEntryHandler(), p2 => this.CanDeleteEntryHandler()));
            this.CmdAgg.AddOrSetCommand("CopyEntryCommand", new RelayCommand(p1 => this.CopyEntryHandler(), p2 => this.CanCopyEntryHandler()));
            this.CmdAgg.AddOrSetCommand("InsertEntryCommand", new RelayCommand(p1 => this.InsertEntryHandler(), p2 => this.CanInsertEntryHandler()));

            this.CmdAgg.AddOrSetCommand("SettingsCommand", new RelayCommand(p1 => this.SettingsHandler(), p2 => this.CanSettingsHandler()));
            this.CmdAgg.AddOrSetCommand("ExportCommand", new RelayCommand(p1 => this.ExportHandler(), p2 => this.CanExportHandler()));
        }

        private void WindowCloseHandler()
        {
            if (this.ExitQuestion == true)
            {
                DialogResultsEx dialogResult = AppMsgDialog.ApplicationExit();
                if (dialogResult == DialogResultsEx.Yes)
                {
                    Window currentWindow = Application.Current.Windows.LastActiveWindow();
                    if (currentWindow != null)
                    {
                        currentWindow.Close();
                    }
                }
            }
            else
            {
                Window currentWindow = Application.Current.Windows.LastActiveWindow();
                if (currentWindow != null)
                {
                    currentWindow.Close();
                }
            }
        }

        private void HelpHandler()
        {
            HelpViewOption options = new HelpViewOption();
            options.HeaderText = "PERT (Effort Tool)";
            options.HelptextFile = $"Help_{this.GetType().Name}.md";
            bool? result = HelpView.Show(options);
        }

        private void CheckBoxCheckedHandler(object argsItem)
        {
            if (argsItem is Collection<object>)
            {
                this.SelectedRows = this.DialogDataView.OfType<EffortProject>().Count(c => c.IsSelected == true);
                this.SelectedRowHeader = $"Sel: {this.SelectedRows}";
            }
            else if (argsItem is EffortProject)
            {
                if (this.CurrentSelectedItem.IsSelected == true)
                {
                    this.CurrentSelectedItem.IsSelected = false;
                }
                else
                {
                    this.CurrentSelectedItem.IsSelected = true;
                }

                this.SelectedRows = this.DialogDataView.OfType<EffortProject>().Count(c => c.IsSelected == true);
                this.SelectedRowHeader = $"Sel: {this.SelectedRows}";
            }
        }

        private void ShowAllItemsChecked(bool? obj)
        {
            if (this.DialogDataView?.OfType<EffortProject>().Count() > 0)
            {
                int selectedRows = 0;
                if (obj == null)
                {
                    int currentCheckedCount = this.DialogDataView.OfType<EffortProject>().Count(c => c.IsSelected == true);
                    if (currentCheckedCount == 0)
                    {
                        foreach (EffortProject item in this.DialogDataView)
                        {
                            item.IsSelected = true;
                            selectedRows++;
                        }
                    }
                    else
                    {
                        foreach (EffortProject item in this.DialogDataView)
                        {
                            item.IsSelected = false;
                        }

                        this.AllItemsChecked = false;
                    }
                }
                else
                {
                    foreach (EffortProject item in this.DialogDataView)
                    {
                        item.IsSelected = (bool)this.AllItemsChecked;
                        if (this.AllItemsChecked == true)
                        {
                            selectedRows++;
                        }
                    }
                }

                this.SelectedRowHeader = $"Sel: {selectedRows}";
            }
        }

        private void OpenDatabaseFromCmdLine(string outPathFile)
        {
            this.CurrentDatabaseFile = outPathFile;

            Result<bool> openResult = null;
            using (DatabaseManager dm = new DatabaseManager(outPathFile))
            {
                openResult = dm.OpenDatabase();
            }

            if (openResult.Success == false)
            {
                this.CurrentDatabaseFile = string.Empty;
                this.IsDatabaseOpen = false;
            }
            else
            {
                this.StatuslineDescription = outPathFile;
                this.IsDatabaseOpen = true;
                this.LoadData();
            }

        }

        private void SelectionChangedHandler(object commandParameter)
        {
            if (commandParameter != null)
            {
                IEnumerable<EffortProject> itemsCollection = ((Collection<object>)commandParameter).OfType<EffortProject>();
                if (itemsCollection.Count() == 0)
                {
                    this.IsContextMenuEnabled = false;

                    this.MinFullEffort = "0,00";
                    this.MidFullEffort = "0,00";
                    this.MaxFullEffort = "0,00";
                    this.PERTFullEffort = "0,00";
                }
                else if (itemsCollection.Count() == 1)
                {
                    this.IsContextMenuEnabled = true;
                    this.MinFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Min).ToString("0.00");
                    this.MidFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Mid).ToString("0.00");
                    this.MaxFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Max).ToString("0.00");

                    double pertFull = 0;
                    foreach (EffortProject item in this.DialogDataView.Cast<EffortProject>())
                    {
                        pertFull += this.CalculatePERTItemValue(item.Min, item.Mid, item.Max, item.Factor);
                    }

                    this.PERTFullEffort = pertFull.ToString("0.00");

                    EffortProject selectedItem = itemsCollection.FirstOrDefault();

                    this.MenuTextAddSub = $"Einfügen nach {selectedItem.ChapterA}.{selectedItem.ChapterB}.{selectedItem.ChapterC} erstellen";
                    this.MenuTextEdit = $"Eintrag zu {selectedItem.ChapterA}.{selectedItem.ChapterB}.{selectedItem.ChapterC} ändern";
                    this.MenuTextDelete = $"Eintrag zu {selectedItem.ChapterA}.{selectedItem.ChapterB}.{selectedItem.ChapterC} löschen";
                    this.MenuTextCopy = $"Eintrag zu {selectedItem.ChapterA}.{selectedItem.ChapterB}.{selectedItem.ChapterC} kopieren";
                }
                else if (itemsCollection.Count() > 1)
                {
                    this.IsContextMenuEnabled = false;

                    this.MinFullEffort = itemsCollection.Sum(c => c.Min).ToString("0.00");
                    this.MidFullEffort = itemsCollection.Sum(c => c.Mid).ToString("0.00");
                    this.MaxFullEffort = itemsCollection.Sum(c => c.Max).ToString("0.00");

                    double pertFull = 0;
                    foreach (EffortProject item in itemsCollection)
                    {
                        pertFull += this.CalculatePERTItemValue(item.Min, item.Mid, item.Max, item.Factor);
                    }

                    this.PERTFullEffort = pertFull.ToString("0.00");
                }
            }
        }
    }
}
