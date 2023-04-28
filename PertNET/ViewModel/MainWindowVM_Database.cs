//-----------------------------------------------------------------------
// <copyright file="MainWindowVM_Database.cs" company="lifeprojects.de">
//     Class: MainWindowVM
//     Copyright © lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>01.07.2022 14:24:34</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.ExceptionHandling;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.IO;
    using EasyPrototypingNET.Pattern;
    using EasyPrototypingNET.WPF;

    using PertNET.Core;
    using PertNET.DataRepository;
    using PertNET.Model;

    public partial class MainWindowVM : ViewModelBase<MainWindowVM>, IViewModel
    {
        private void NewDatabaseHandler()
        {
            string outPathFile = string.Empty;
            string fileTyp = "eff";
            string initFolder = LastSavedFolder.Get("CreateDatabase");
            string initialFile = $"{nameof(MainWindowVM)}.eff";

            try
            {
                FileFilter fileFilter = new FileFilter();
                fileFilter.AddFilter("PERT", $"{fileTyp}", true);

                List<string> folders = LastSavedFolder.GetFolders();
                SelectFolderSettings settings = new SelectFolderSettings();
                settings.Owner = Application.Current.MainWindow;
                settings.HeaderText = "Speichern der Datenbanke als ...";
                settings.InstructionText = "Erstellen einer neuen PERT Datenbank";
                settings.DescriptionText = "Wählen Sie ein Verzeichnis aus der Liste oder ein neues Verszeichnis über den Button unten.";
                settings.SelectFolderText = "Wählen sie einen anderen Ordner...";
                settings.FileTyp = fileTyp;
                settings.FileFilter = fileFilter;
                settings.Folders = folders;
                settings.InitialFolder = initFolder;
                settings.InitialFile = initialFile;

                using (SelectFolderForDialog selectFolder = new SelectFolderForDialog(settings))
                {
                    outPathFile = selectFolder.SelectFolder;
                }

                if (string.IsNullOrEmpty(outPathFile) == false)
                {
                    Result<bool> createResult = null;
                    using (DatabaseManager dm = new DatabaseManager(outPathFile))
                    {
                        createResult = dm.CreateNewDatabase();
                    }

                    if (createResult.Success == false)
                    {
                        this.CurrentDatabaseFile = string.Empty;
                        this.IsDatabaseOpen = false;
                    }
                    else
                    {
                        this.CurrentDatabaseFile = outPathFile;
                        this.StatuslineDescription = outPathFile;
                        this.IsDatabaseOpen = true;
                        LastSavedFolder.GetOrSet("CreateDatabase", Path.GetDirectoryName(outPathFile));
                        LastSavedFolder.Save();
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        private void OpenDatabaseHandler()
        {
            string selectedDatabaseFile = string.Empty;
            string fileTyp = "eff";
            string initFolder = LastSavedFolder.Get("OpenDatabase");
            string initialImportFile = $"*.{fileTyp}";

            try
            {
                FileFilter fileFilter = new FileFilter();
                fileFilter.AddFilter("PERT", $"{fileTyp}", true);

                List<string> folders = LastSavedFolder.GetFolders();
                SelectFolderSettings settings = new SelectFolderSettings();
                settings.Owner = Application.Current.MainWindow;
                settings.HeaderText = "Auswahl Datenbank ...";
                settings.InstructionText = "Öffnen einer PERT Datenbank mit einer Aufwandsschätzungen...";
                settings.DescriptionText = "Wählen Sie ein Verzeichnis aus der Liste oder ein neues Verszeichnis über den Button unten.";
                settings.SelectFolderText = "Wählen sie einen anderen Ordner...";
                settings.FileFilter = fileFilter;
                settings.Folders = folders;
                settings.InitialFolder = initFolder;
                settings.InitialFile = initialImportFile;
                settings.FolderAction = SelectFolderAction.OpenFile;

                using (SelectFolderForDialog selectFolder = new SelectFolderForDialog(settings))
                {
                    selectedDatabaseFile = selectFolder.SelectFolder;
                }

                if (string.IsNullOrEmpty(selectedDatabaseFile) == false)
                {
                    Result<bool> openResult = null;
                    using (DatabaseManager dm = new DatabaseManager(selectedDatabaseFile))
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
                        this.CurrentDatabaseFile = selectedDatabaseFile;
                        this.StatuslineDescription = selectedDatabaseFile;
                        this.IsDatabaseOpen = true;
                        LastSavedFolder.GetOrSet("OpenDatabase", Path.GetDirectoryName(selectedDatabaseFile));
                        LastSavedFolder.Save();

                        if (this.IsDatabaseBackup == true)
                        {
                            using (DatabaseBackup backup = new DatabaseBackup())
                            {
                                backup.CheckAndRun();
                            }
                        }

                        this.LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        private bool CanCloseDatabaseHandler()
        {
            if (this.IsDatabaseOpen == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CloseDatabaseHandler()
        {
            try
            {
                Result<bool> closeResult = null;
                using (DatabaseManager dm = new DatabaseManager(this.CurrentDatabaseFile))
                {
                    closeResult = dm.Close();
                }

                if (closeResult != null && closeResult.Success == true)
                {
                    this.CurrentDatabaseFile = string.Empty;
                    this.StatuslineDescription = string.Empty;
                    this.IsDatabaseOpen = false;
                    this.DialogDataView = null;
                    this.MaxRowCount = 0;
                    this.IsFilterContentFound = true;
                    this.MinFullEffort = "0,00";
                    this.MidFullEffort = "0,00";
                    this.MaxFullEffort = "0,00";
                    this.PERTFullEffort = "0,00";
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        private void CloseDatabaseForCopy()
        {
            try
            {
                Result<bool> closeResult = null;
                using (DatabaseManager dm = new DatabaseManager(this.CurrentDatabaseFile))
                {
                    closeResult = dm.Close();
                }

                if (closeResult != null && closeResult.Success == true)
                {
                    this.IsDatabaseOpen = true;
                    this.DialogDataView = null;
                    this.MaxRowCount = 0;
                    this.IsFilterContentFound = true;
                    this.MinFullEffort = "0,00";
                    this.MidFullEffort = "0,00";
                    this.MaxFullEffort = "0,00";
                    this.PERTFullEffort = "0,00";
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        private bool CanSaveAsDatabaseHandler()
        {
            if (this.IsDatabaseOpen == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SaveAsDatabaseHandler()
        {
            string resultPath = string.Empty;

            try
            {
                string newDatabaseFile = $"{Path.GetFileNameWithoutExtension(this.CurrentDatabaseFile)}_{DateTime.Now.ToString("yyyyMMdd")}{Path.GetExtension(this.CurrentDatabaseFile)}";

                using (SaveFileDialogEx dlgFile = new SaveFileDialogEx())
                {
                    dlgFile.Title = "PERT Database speichern als";
                    dlgFile.InitialDirectory = Path.GetDirectoryName(this.CurrentDatabaseFile);
                    dlgFile.RestoreDirectory = true;
                    dlgFile.FileName = newDatabaseFile;
                    resultPath = dlgFile.OpenDialog();
                }

                if (string.IsNullOrEmpty(resultPath) == false)
                {
                    this.CloseDatabaseForCopy();
                    File.Copy(this.CurrentDatabaseFile, resultPath);
                    if (File.Exists(resultPath) == true)
                    {
                        this.LoadData();

                        MessageBoxEx.Show("PERT Database", $"Die Datenbank wurde erfolgreich kopiert.", $"Von '{this.CurrentDatabaseFile}' nach '{resultPath}'", MessageBoxButton.OK, InstructionIcon.Information, DialogResultsEx.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        private void LoadData()
        {
            if (this.IsDatabaseOpen == true)
            {
                try
                {
                    WorkUserInfo workUserInfo = null;
                    using (WorkUserInfoRepository repository = new WorkUserInfoRepository(this.CurrentDatabaseFile))
                    {
                        workUserInfo = repository.List().First();
                        this.ProjectName = workUserInfo.Project;
                        this.ExportCompanyName = workUserInfo.Company;
                        this.ExportProjectName = workUserInfo.Project;
                    }

                    using (EffortProjectRepository repository = new EffortProjectRepository(this.CurrentDatabaseFile))
                    {
                        IEnumerable<EffortProject> overviewSource = repository.List();
                        if (overviewSource != null)
                        {
                            this.DialogDataView = CollectionViewSource.GetDefaultView(overviewSource);
                            if (this.DialogDataView != null)
                            {
                                this.DialogDataView.Filter = rowItem => this.DataDefaultFilter(rowItem as EffortProject);
                                this.DialogDataView.SortDescriptions.Clear();
                                this.DialogDataView.SortDescriptions.Add(new SortDescription("ChapterA", ListSortDirection.Ascending));
                                this.DialogDataView.SortDescriptions.Add(new SortDescription("ChapterB", ListSortDirection.Ascending));
                                this.DialogDataView.SortDescriptions.Add(new SortDescription("ChapterC", ListSortDirection.Ascending));
                                this.DialogDataView.MoveCurrentToFirst();
                                this.MaxRowCount = this.DialogDataView.Count<EffortProject>();

                                if (this.MaxRowCount > 0)
                                {
                                    this.IsFilterContentFound = false;
                                    this.MinFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Min).ToString("0.00");
                                    this.MidFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Mid).ToString("0.00");
                                    this.MaxFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Max).ToString("0.00");

                                    double pertFull = 0;
                                    foreach (EffortProject item in this.DialogDataView.Cast<EffortProject>())
                                    {
                                        pertFull += this.CalculatePERTItemValue(item.Min, item.Mid, item.Max, item.Factor);
                                    }

                                    this.PERTFullEffort = pertFull.ToString("0.00");

                                    EffortProject lastEdit = this.DialogDataView.Cast<EffortProject>().OrderByDescending(x => x.ModifiedOn).FirstOrDefault();
                                    this.LastEdit = $"{lastEdit.ModifiedBy} am {lastEdit.ModifiedOn}";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionViewer.Show(ex, this.GetType().Name);
                }
            }
        }

        private bool DataDefaultFilter(EffortProject rowItem)
        {
            bool wordFound = false;

            if (rowItem == null)
            {
                this.IsFilterContentFound = false;
                return false;
            }

            string textFilterString = (this.FilterDefaultSearch ?? string.Empty).ToUpper();
            if (string.IsNullOrEmpty(textFilterString) == false)
            {
                string fullRow = rowItem.ToSearchFilter().ToUpper();
                if (string.IsNullOrEmpty(fullRow) == true)
                {
                    return true;
                }

                string[] words = textFilterString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words.AsParallel<string>())
                {
                    wordFound = fullRow.Contains(word);

                    if (wordFound == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void RefreshDefaultFilter(string value)
        {
            if (value != null && this.DialogDataView != null)
            {
                try
                {
                    this.DialogDataView.Refresh();
                    this.MaxRowCount = this.DialogDataView.Cast<EffortProject>().Count();
                    this.DialogDataView.MoveCurrentToFirst();

                    if (this.MaxRowCount > 0)
                    {
                        this.IsFilterContentFound = false;

                        this.MinFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Min).ToString("0.00");
                        this.MidFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Mid).ToString("0.00");
                        this.MaxFullEffort = this.DialogDataView.Cast<EffortProject>().Sum(s => s.Max).ToString("0.00");

                        double pertFull = 0;
                        foreach (EffortProject item in this.DialogDataView.Cast<EffortProject>())
                        {
                            pertFull += this.CalculatePERTItemValue(item.Min, item.Mid, item.Max, item.Factor);
                        }

                        this.PERTFullEffort = pertFull.ToString("0.00");
                    }
                    else
                    {
                        this.IsFilterContentFound = true;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionViewer.Show(ex, this.GetType().Name);
                }
            }
        }

        private bool CanClearFilterHandler()
        {
            return string.IsNullOrEmpty(this.FilterDefaultSearch) == false;
        }

        private void ClearFilterHandler()
        {
            this.FilterDefaultSearch = string.Empty;
        }

        private double CalculatePERTItemValue(double minEffort, double midEffort, double maxEffort, double factor)
        {
            double tempResult = 0;
            using (CalcPERT calc = new CalcPERT())
            {
                tempResult = calc.PERTEffort(minEffort, midEffort, maxEffort, factor);
            }

            return tempResult;
        }
    }
}
