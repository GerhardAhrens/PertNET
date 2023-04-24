//-----------------------------------------------------------------------
// <copyright file="ApplicationSettingsVM.cs" company="Lifeprojects.de">
//     Class: ApplicationSettingsVM
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>15.07.2022 11:05:23</date>
//
// <summary>
// ViewModel Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Core.Collection;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.WPF;

    using PertNET.Core;

    public class ApplicationSettingsVM : ViewModelBase<ApplicationSettingsVM>, IViewModel
    {
        private LexiconCollection<SettingsTabItem, string> searchList;
        private readonly ObservableCollectionEx<Visibility> visibilityAgg;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationSettingsVM"/> class.
        /// </summary>
        public ApplicationSettingsVM()
        {
            this.visibilityAgg = new ObservableCollectionEx<Visibility>();
            this.CreateVisibilityList();
            this.TabItemSearchList();
            this.InitCommands();
        }

        [PropertyBinding]
        public string FilterDefaultSearch
        {
            get { return this.Get<string>(); }
            set { this.Set(value, this.DefaultSearch); }
        }

        public ObservableCollectionEx<Visibility> VisibilityAgg
        {
            get
            {
                return this.visibilityAgg;
            }
        }

        protected override void InitCommands()
        {
            this.CmdAgg.AddOrSetCommand("WindowCloseCommand", new RelayCommand(p1 => this.WindowCloseHandler(), p2 => true));
            this.CmdAgg.AddOrSetCommand("HelpCommand", new RelayCommand(p1 => this.HelpHandler(), p2 => true));
        }

        private void WindowCloseHandler()
        {
            Window currentWindow = Application.Current.Windows.LastActiveWindow();
            if (currentWindow != null)
            {
                currentWindow.Close();
            }
        }

        private void HelpHandler()
        {
            HelpViewOption options = new HelpViewOption();
            options.HeaderText = "PERT (Effort Tool)";
            options.HelptextFile = $"Help_{this.GetType().Name}.md";
            bool? result = HelpView.Show(options);
        }

        private void DefaultSearch(string obj)
        {
            if (obj == string.Empty)
            {
                this.VisibilityAgg[0] = Visibility.Visible;
                this.VisibilityAgg[1] = Visibility.Visible;
                this.VisibilityAgg[2] = Visibility.Visible;
                this.VisibilityAgg[3] = Visibility.Visible;
                return;
            }
            else
            {
                this.VisibilityAgg[0] = Visibility.Collapsed;
                this.VisibilityAgg[1] = Visibility.Collapsed;
                this.VisibilityAgg[2] = Visibility.Collapsed;
                this.VisibilityAgg[3] = Visibility.Collapsed;
            }

            var entry = this.searchList.Values.Where(f => f.ToLower().Contains(obj) == true);
            if (entry != null && entry.Any() == true)
            {
                foreach (string item in entry)
                {
                    IEnumerable<KeyValuePair<SettingsTabItem, int>> indexList = searchList.FindKeyIndexPairs(item);
                    foreach (KeyValuePair<SettingsTabItem, int> itemFund in indexList)
                    {
                        if (itemFund.Key >= 0 && (int)itemFund.Key < this.VisibilityAgg.Count)
                        {
                            this.VisibilityAgg[(int)itemFund.Key] = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void CreateVisibilityList()
        {
            this.VisibilityAgg.Add(Visibility.Visible);
            this.VisibilityAgg.Add(Visibility.Visible);
            this.VisibilityAgg.Add(Visibility.Visible);
            this.VisibilityAgg.Add(Visibility.Visible);
        }

        private void TabItemSearchList()
        {
            searchList = new LexiconCollection<SettingsTabItem, string>();
            searchList.Add(SettingsTabItem.Allgemein, "Allgemein");
            searchList.Add(SettingsTabItem.Allgemein, "Position");
            searchList.Add(SettingsTabItem.Allgemein, "BeendenDialog");
            searchList.Add(SettingsTabItem.Allgemein, "exit");
            searchList.Add(SettingsTabItem.Allgemein, "Backup");
            searchList.Add(SettingsTabItem.Allgemein, "Sicherung");
            searchList.Add(SettingsTabItem.UserProjectInfo, "Name");
            searchList.Add(SettingsTabItem.UserProjectInfo, "bearbeiter");
            searchList.Add(SettingsTabItem.UserProjectInfo, "UserId");
            searchList.Add(SettingsTabItem.UserProjectInfo, "email");
            searchList.Add(SettingsTabItem.UserProjectInfo, "Projekt");
            searchList.Add(SettingsTabItem.UserProjectInfo, "stand");
            searchList.Add(SettingsTabItem.Statistik, "Anzahl");
            searchList.Add(SettingsTabItem.Statistik, "Count");
            searchList.Add(SettingsTabItem.Statistik, "Statistik");
            searchList.Add(SettingsTabItem.Statistik, "Letzte Änderung");
            searchList.Add(SettingsTabItem.Statistik, "Backup");
            searchList.Add(SettingsTabItem.Statistik, "Sicherung");
            searchList.Add(SettingsTabItem.Statistik, "Zugriff");
            searchList.Add(SettingsTabItem.ApplicationInfo, "Applikation Info");
            searchList.Add(SettingsTabItem.ApplicationInfo, "Version");
        }
    }
}
