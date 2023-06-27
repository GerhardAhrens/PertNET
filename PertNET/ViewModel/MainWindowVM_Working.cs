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
    using DocumentFormat.OpenXml.EMMA;

    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.ExceptionHandling;
    using EasyPrototypingNET.Interface;
    using EasyPrototypingNET.WPF;

    using PERT.ViewModel;

    using PertNET.Core;
    using PertNET.DataRepository;
    using PertNET.Model;
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
                ExceptionViewer.Show(ex, this.GetType().Name);
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
                ExceptionViewer.Show(ex, this.GetType().Name);
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
                    List<EffortProject> chapters = null;
                    using (EffortProjectRepository repository = new EffortProjectRepository(this.CurrentDatabaseFile))
                    {
                        chapters = this.DialogDataView.Cast<EffortProject>().ToList();
                        chapters.SingleOrDefault(s => s.Id == this.CurrentSelectedItem.Id).ChapterDelete = true; ;
                        repository.Delete(this.CurrentSelectedItem.Id);
                    }

                    if (chapters != null)
                    {
                        List<EffortProject> renumber = ReNumberChapterDelete(chapters);
                        using (EffortProjectRepository repository = new EffortProjectRepository(this.CurrentDatabaseFile))
                        {
                            foreach (EffortProject item in renumber)
                            {
                                Guid id = item.Id;
                                EffortProject row = repository.ListById(id);
                                if (row != null)
                                {
                                    row.ChapterA = item.ChapterA;
                                    row.ChapterB = item.ChapterB;
                                    row.ChapterC = item.ChapterC;
                                    repository.Update(row);
                                }
                            }
                        }
                    }

                    this.LoadData();
                }
                catch (Exception ex)
                {
                    ExceptionViewer.Show(ex, this.GetType().Name);
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
                ExceptionViewer.Show(ex, this.GetType().Name);
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
            try
            {
                List<EffortProject> listbefore = this.DialogDataView.OfType<EffortProject>().ToList();
                listbefore.ForEach(f => f.ChapterInsert = false);
                EffortProject model = null;
                using (DialogService ws = new DialogService())
                {
                    SwitchAnimations.FadeOut();

                    EffortProjectVM vm = new EffortProjectVM(Guid.Empty, true, false);
                    ws.Title = "Eintrag einfügen";
                    ws.ResizeMode = ResizeMode.NoResize;
                    ws.WindowStyle = WindowStyle.None;
                    ws.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    bool? dlgResult = ws.ShowDialog<EffortProjectView>(vm, this.mainWindow, MonitorSelect.Primary);
                    model = ws.ResultContent as EffortProject;
                    SwitchAnimations.FadeIn();
                }

                if (model != null)
                {
                    List<EffortProject> renumber = ReNumberChapterInsert(listbefore, model);

                    using (EffortProjectRepository repository = new EffortProjectRepository(this.CurrentDatabaseFile))
                    {
                        foreach (EffortProject item in renumber)
                        {
                            Guid id = item.Id;
                            EffortProject row = repository.ListById(id);
                            if (row != null)
                            {
                                row.ChapterA = item.ChapterA;
                                row.ChapterB = item.ChapterB;
                                row.ChapterC = item.ChapterC;
                                repository.Update(row);
                            }
                        }
                    }
                }

                this.LoadData();
                this.DialogDataView.MoveCurrentTo(null);
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        private List<EffortProject> ReNumberChapterInsert(List<EffortProject> chapters, EffortProject insert)
        {
            chapters.Add(insert);
            chapters = chapters.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenByDescending(i => i.ChapterInsert).ToList();

            var query = chapters.Select((x, i) => new { Index = i, Value = x })
                   .GroupBy(x => new { x.Value.ChapterA, x.Value.ChapterB, x.Value.ChapterC })
                   .Where(x => x.Skip(1).Any()).ToList().FirstOrDefault();

            if (query != null)
            {
                int startIndex = 0;
                foreach (var group in query.Where(w => w.Value.ChapterInsert == true))
                {
                    startIndex = group.Index;
                }

                for (int i = startIndex + 1; i < chapters.Count; i++)
                {
                    EffortProject insertItem = chapters[startIndex];
                    EffortProject currentItem = chapters[i];

                    if (insertItem.ChapterA > 0 && insertItem.ChapterB == 0 && insertItem.ChapterC == 0)
                    {
                        chapters[i].ChapterA = chapters[i].ChapterA + 1;
                    }
                    else if (insertItem.ChapterA == currentItem.ChapterA && insertItem.ChapterB > 0 && insertItem.ChapterC == 0)
                    {
                        chapters[i].ChapterB = chapters[i].ChapterB + 1;
                    }
                    else if (insertItem.ChapterA == currentItem.ChapterA && insertItem.ChapterB == currentItem.ChapterB && insertItem.ChapterC > 0)
                    {
                        chapters[i].ChapterB = chapters[i].ChapterB + 1;
                    }
                }

                chapters.ForEach(f => f.ChapterInsert = false);
            }

            return chapters;
        }

        private List<EffortProject> ReNumberChapterDelete(List<EffortProject> chapters)
        {
            bool first = false;

            int startIndex = chapters.IndexOf(i => i.ChapterDelete == true);

            if (startIndex == 0)
            {
                first = true;
            }

            EffortProject startItem = chapters.Find(f => f.ChapterDelete == true);
            chapters.RemoveAt(startIndex);
            chapters = chapters.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenByDescending(i => i.ChapterDelete).ToList();

            for (int i = startIndex; i < chapters.Count; i++)
            {
                EffortProject currentItem = chapters[i];

                if (startItem.ChapterB == 0 && startItem.ChapterC == 0 && first == true)
                {
                    if (currentItem.ChapterB >= 0 && currentItem.ChapterC >= 0)
                    {
                        chapters[i].ChapterA = chapters[i].ChapterA - 1;
                    }
                }

                if (startItem.ChapterB == 0 && startItem.ChapterC == 0 && first == false)
                {
                    if (startItem.ChapterA == currentItem.ChapterA && currentItem.ChapterB > 0 && currentItem.ChapterC == 0)
                    {
                        chapters[i].ChapterB = chapters[i].ChapterB - 1;
                    }

                }

                if (startItem.ChapterB > 0 && startItem.ChapterC == 0 && first == false)
                {
                    if (startItem.ChapterA == currentItem.ChapterA && currentItem.ChapterB > 0 && currentItem.ChapterC == 0)
                    {
                        chapters[i].ChapterB = chapters[i].ChapterB - 1;
                    }

                }
            }

            chapters = chapters.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ToList();

            return chapters;
        }
    }
}
