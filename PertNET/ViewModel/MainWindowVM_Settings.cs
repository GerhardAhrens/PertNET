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
    using System.Windows;

    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.ExceptionHandling;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.WPF;

    using PertNET.View;

    public partial class MainWindowVM : ViewModelBase<MainWindowVM>, IViewModel
    {
        private bool CanSettingsHandler()
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

        private void SettingsHandler()
        {
            try
            {
                using (DialogService ws = new DialogService())
                {
                    SwitchAnimations.FadeOut();

                    ApplicationSettingsVM vm = new ApplicationSettingsVM();

                    ws.Title = "Neuer Eintrag";
                    ws.ResizeMode = ResizeMode.NoResize;
                    ws.WindowStyle = WindowStyle.None;
                    ws.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    bool? dlgResult = ws.ShowDialog<ApplicationSettingsView>(vm, this.mainWindow, MonitorSelect.Primary);
                    SwitchAnimations.FadeIn();
                }

                this.LoadData();
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }
    }
}
