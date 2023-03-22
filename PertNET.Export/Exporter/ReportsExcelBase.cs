//-----------------------------------------------------------------------
// <copyright file="ReportsExcelBase.cs" company="Lifeprojects.de">
//     Class: ReportsExcelBase
//     Copyright © Lifeprojects.de 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>28.09.2016</date>
//
// <summary>Abstract Class for Excel Reporting</summary>
//-----------------------------------------------------------------------

namespace PertNET.Export.Exporter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Windows.Input;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.ExceptionHandling;
    using EasyPrototypingNET.IO;

    [SupportedOSPlatform("windows")]
    public abstract class ReportsExcelBase : IReportsExcel, IDisposable
    {
        private readonly Cursor previousCursor = null;
        private bool classIsDisposed = false;

        public ReportsExcelBase(DataTable objects)
        {
            if (objects != null)
            {
                this.previousCursor = Mouse.OverrideCursor;
                Mouse.OverrideCursor = Cursors.Wait;

                this.SourceItems = objects;
                this.Columns = objects.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            }
        }

        public ReportsExcelBase(ICollectionView objects)
        {
            if (objects != null)
            {
                this.previousCursor = Mouse.OverrideCursor;
                Mouse.OverrideCursor = Cursors.Wait;

                PropertyInfo[] fields = objects.CurrentItem.GetType().GetProperties();

                this.SourceItems = objects;
                this.Columns = fields.Cast<PropertyInfo>().Select(x => x.Name).ToList();
            }
        }

        public int FreezeRows { get; set; } = 1;

        public int FreezeColumns { get; set; } = 1;

        public object SourceItems { get; set; }

        public List<string> Columns { get; set; }

        public bool IsDecimalCurrency { get; set; } = false;

        public string SheetName { get; set; }

        public string Filename { get; set; }

        public Dictionary<string, string> TranslateDictionary { get; set; }

        public bool IsExcelRunning(string fileName)
        {
            bool result = false;

            using(ApplicationProcess ap = new ApplicationProcess())
            {
                IEnumerable<Process> currentProcesses = ap.GetProcesses();
                result = currentProcesses.IsExcelRunning(fileName);
            }

            return result;
        }

        public bool IsLocked(string fileName)
        {
            bool result = false;

            using (LockFile fl = new LockFile())
            {
                result = fl.CanReadFile(fileName) == false;
            }

            return result;
        }

        public virtual void Run()
        {
            try
            {
                this.SourceItems.IsArgumentNull(nameof(this.SourceItems));
                this.Filename.IsArgumentNullOrEmpty(nameof(this.Filename));

                this.SheetName = Path.GetFileNameWithoutExtension(this.Filename);

                if (this.IsLocked(this.Filename) == true)
                {
                    throw new FileLockException($"Es ist ein Fehler beim Bearbeiten der Datei '{this.Filename}' aufgetreten. Datei ist gesperrt.", this.Filename);
                }

                using (RunFileExplorer runFile = new RunFileExplorer())
                {
                    if (this.SourceItems is DataTable)
                    {
                        if (this.TranslateDictionary != null)
                        {
                            this.CreateDocument(this.SourceItems as DataTable, this.Filename, this.TranslateDictionary);
                        }
                        else
                        {
                            this.CreateDocument(this.SourceItems as DataTable, this.Filename);
                        }
                    }
                    else if (this.SourceItems is ICollectionView)
                    {
                        if (this.TranslateDictionary != null)
                        {
                            this.CreateDocument(this.SourceItems as ICollectionView, this.Filename, this.TranslateDictionary);
                        }
                        else
                        {
                            this.CreateDocument(this.SourceItems as ICollectionView, this.Filename);
                        }
                    }

                    runFile.SelectFile(this.Filename);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        public abstract void CreateDocument(DataTable objects, string outPath, Dictionary<string,string> translateColumnText = null);

        public abstract void CreateDocument(ICollectionView objects, string outPath, Dictionary<string, string> translateColumnText = null);

        public virtual void CreateDocument(DataTable objects, string outPath,string sheetName, Dictionary<string, string> translateColumnText = null)
        {
        }

        public virtual void CreateDocument(ICollectionView objects, string outPath, string sheetName, Dictionary<string, string> translateColumnText = null)
        {
        }

        #region Dispose
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool classDisposing = false)
        {
            if (this.classIsDisposed == false)
            {
                if (classDisposing == true)
                {
                    Mouse.OverrideCursor = this.previousCursor;
                }

                /*
                 * Hier unmanaged Objekte freigeben (z.B. IntPtr)
                */
            }

            this.classIsDisposed = true;
        }
        #endregion Dispose
    }
}