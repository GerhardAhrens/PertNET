//-----------------------------------------------------------------------
// <copyright file="MainWindowVM_Working.cs" company="Lifeprojects.de">
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
    using System.Linq;
    using System.Windows;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.WPF;

    using PERT.ViewModel;

    using PertNET.Core;
    using PertNET.DataRepository;
    using PertNET.View;

    public partial class MainWindowVM : ViewModelBase<MainWindowVM>, IViewModel
    {
        private bool CanAddEntryHandler()
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

        private void AddEntryHandler()
        {
            try
            {
                using (DialogService ws = new DialogService())
                {
                    SwitchAnimations.FadeOut();

                    EffortProjectVM vm = new EffortProjectVM(Guid.Empty, false, false);
                    ws.Title = "Neuer Eintrag";
                    ws.ResizeMode = ResizeMode.NoResize;
                    ws.WindowStyle = WindowStyle.None;
                    ws.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    bool? dlgResult = ws.ShowDialog<EffortProjectView>(vm, this.mainWindow, MonitorSelect.Primary);
                    SwitchAnimations.FadeIn();
                }

                this.LoadData();
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private bool CanAddSubEntryHandler()
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

        private void AddSubEntryHandler()
        {
            try
            {
                int currentPos = this.DialogDataView.CurrentPosition;
                /*
                SwitchAnimations.FadeOut();
                EffortProjectView dlg = new EffortProjectView(this.CurrentSelectedItem, true);
                dlg.Owner = Application.Current.MainWindow;
                dlg.Title = $"Eintrag hinzufügen [{this.CurrentSelectedItem.FullName}]";
                dlg.ResizeMode = ResizeMode.NoResize;
                dlg.WindowStyle = WindowStyle.None;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                bool? dlgResult = dlg.ShowDialog();
                if (dlgResult.HasValue == true && dlgResult == true)
                {
                    this.LoadData();
                    this.DialogDataView.MoveCurrentToPosition(currentPos);
                }
                */

                SwitchAnimations.FadeIn();
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private bool CanEditEntryHandler()
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

        private void EditEntryHandler()
        {
            try
            {
                int currentPos = this.DialogDataView.CurrentPosition;

                using (DialogService ws = new DialogService())
                {
                    SwitchAnimations.FadeOut();

                    EffortProjectVM vm = new EffortProjectVM(this.CurrentSelectedItem.Id, false, false);

                    //ws.ResultContent = this.CurrentSelectedItem.Id;
                    ws.Title = $"Eintrag ändern [{this.CurrentSelectedItem.FullName}]";
                    ws.ResizeMode = ResizeMode.NoResize;
                    ws.WindowStyle = WindowStyle.None;
                    ws.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    bool? dlgResult = ws.ShowDialog<EffortProjectView>(vm, this.mainWindow);
                    SwitchAnimations.FadeIn();
                }

                this.LoadData();
                this.DialogDataView.MoveCurrentToPosition(currentPos);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private bool CanDeleteEntryHandler()
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

        private void DeleteEntryHandler()
        {
            if (this.CurrentSelectedItem == null)
            {
                AppMsgDialog.NoSelectedItem();
                return;
            }

            if (AppMsgDialog.DeleteItem(this.CurrentSelectedItem.FullName) == DialogResultsEx.Yes)
            {
                try
                {
                    using (EffortProjectRepository repository = new EffortProjectRepository(this.CurrentDatabaseFile))
                    {
                        repository.Delete(this.CurrentSelectedItem.Id);
                    }

                    this.LoadData();
                }
                catch (Exception ex)
                {
                    string errorText = ex.Message;
                    throw;
                }
            }
        }

        private bool CanCopyEntryHandler()
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

        private void CopyEntryHandler()
        {
            if (this.CurrentSelectedItem == null)
            {
                AppMsgDialog.NoSelectedItem();
                return;
            }

            if (AppMsgDialog.CopySelectedItem(this.CurrentSelectedItem.FullName) == DialogResultsEx.No)
            {
                return;
            }

            try
            {
                int currentPos = this.DialogDataView.CurrentPosition;

                using (DialogService ws = new DialogService())
                {
                    SwitchAnimations.FadeOut();

                    EffortProjectVM vm = new EffortProjectVM(this.CurrentSelectedItem.Id, true, false);

                    ws.Title = $"Eintrag kopieren [{this.CurrentSelectedItem.FullName}]";
                    ws.ResizeMode = ResizeMode.NoResize;
                    ws.WindowStyle = WindowStyle.None;
                    ws.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    bool? dlgResult = ws.ShowDialog<EffortProjectView>(vm, this.mainWindow, MonitorSelect.Primary);
                    SwitchAnimations.FadeIn();
                }

                this.LoadData();
                this.DialogDataView.MoveCurrentToPosition(currentPos);
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private bool CanInsertEntryHandler()
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

        private void InsertEntryHandler()
        {
        }
    }
}
