//-----------------------------------------------------------------------
// <copyright file="MainWindowVM_Settings.cs" company="Lifeprojects.de">
//     Class: MainWindowVM
//     Copyright © Lifeprojects.de 2022
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
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Windows;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.IO;
    using EasyPrototypingNET.WPF;

    using PertNET.Core;
    using PertNET.DataRepository;
    using PertNET.Export.Exporter;
    using PertNET.Model;

    public partial class MainWindowVM : ViewModelBase<MainWindowVM>, IViewModel
    {
        private bool CanExportHandler()
        {
            if (this.IsDatabaseOpen == true && this.DialogDataView.Count<EffortProject>() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExportHandler()
        {
            string outPathFile = string.Empty;

            if (this.CurrentSelectedItem == null)
            {
                AppMsgDialog.DataNotFound();
                return;
            }
            IDictionary<string, Type> columns = EffortProject.ExportFieldsFilter;
            if (columns == null || columns.Count == 0)
            {
                AppMsgDialog.NoMarkedColumns();
                return;
            }

            string initFolder = LastSavedFolder.Get(typeof(EffortProject).Name);
            string initialExportFile = $"{this.ExportCompanyName}_{this.ExportProjectName}.xlsx";

            outPathFile = this.ExportFolder(initFolder, initialExportFile);

            try
            {
                if (string.IsNullOrEmpty(outPathFile) == false)
                {
                    WorkUserInfo workUserInfo = null;
                    using (WorkUserInfoRepository repository = new WorkUserInfoRepository(this.CurrentDatabaseFile))
                    {
                        workUserInfo = repository.List().First();
                        if (string.IsNullOrEmpty(workUserInfo.Company) == false || string.IsNullOrEmpty(workUserInfo.Project) == false)
                        {
                            initialExportFile = $"{this.ExportCompanyName}_{this.ExportProjectName}.xlsx";
                        }
                        else
                        {
                            initialExportFile = "Unbekannt.xlsx";
                        }
                    }

                    DataTable dt = null;
                    if (this.SelectedRows == 0)
                    {
                        List<EffortProject> locList = this.DialogDataView.OfType<EffortProject>().ToList<EffortProject>();
                        dt = locList.ToDataTable<EffortProject>(nameof(EffortProject), columns);
                    }
                    else
                    {
                        List<EffortProject> locList = this.DialogDataView.OfType<EffortProject>().Where(w => w.IsSelected == true).ToList<EffortProject>();
                        dt = locList.ToDataTable<EffortProject>(nameof(EffortProject), columns);
                    }

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        AppMsgDialog.NoMarkedRows();
                        return;
                    }


                    Dictionary<string, string> translateText = null;
                    using (TranslateDictionary translateDic = new TranslateDictionary(dt.Columns))
                    {
                        translateDic.AddTranslateText("Chapter", "Aufwandspunkt");
                        translateDic.AddTranslateText("Title", "Bezeichnung");
                        translateDic.AddTranslateText("Min", "optimistisch");
                        translateDic.AddTranslateText("Mid", "wahrscheinlich");
                        translateDic.AddTranslateText("Max", "pessimistisch");
                        translateDic.AddTranslateText("Factor", "Faktor");
                        translateDic.AddTranslateText("Description", "Beschreibung");
                        translateText = translateDic.Translate();
                    }

                    LastSavedFolder.GetOrSet(typeof(EffortProject).Name, Path.GetDirectoryName(outPathFile));

                    using (PERTEffortExport exportExcel = new PERTEffortExport(dt))
                    {
                        exportExcel.WorkUserInfo = workUserInfo;
                        exportExcel.Filename = outPathFile;
                        exportExcel.TranslateDictionary = translateText;
                        exportExcel.Run();
                        LastSavedFolder.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        private string ExportFolder(string initFolder, string initialExportFile)
        {
            string outPathFile = string.Empty;

            List<string> folders = LastSavedFolder.GetFolders();
            SelectFolderSettings settings = new SelectFolderSettings();
            settings.Owner = Application.Current.MainWindow;
            settings.FileTyp = "xlsx";
            settings.HeaderText = "Export save ...";
            settings.InstructionText = "Export data as a file for further editing";
            settings.DescriptionText = "Wählen Sie ein Verzeichnis aus der Liste oder ein neues Verszeichnis über den Button unten.";
            settings.SelectFolderText = "Wählen sie einen anderen Ordner...";
            settings.Folders = folders;
            settings.InitialFolder = initFolder;
            settings.InitialFile = initialExportFile;

            using (SelectFolderForDialog selectFolder = new SelectFolderForDialog(settings))
            {
                outPathFile = selectFolder.SelectFolder;
            }

            return outPathFile;
        }
    }
}
