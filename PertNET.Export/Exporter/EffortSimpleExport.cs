//-----------------------------------------------------------------------
// <copyright file="EffortSimpleExport.cs" company="Lifeprojects.de">
//     Class: EffortSimpleExport
//     Copyright © Lifeprojects.de 2023
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>28.04.2003</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Export.Exporter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Windows;

    using ClosedXML.Excel;

    using EasyPrototypingNET.ExceptionHandling;
    using EasyPrototypingNET.IO;

    using PertNET.Data.Core;
    using PertNET.Model;

    [SupportedOSPlatform("windows")]
    public class EffortSimpleExport : ReportsExcelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffortSimpleExport"/> class.
        /// </summary>
        public EffortSimpleExport(DataTable objects) : base(objects)
        {
            this.SourceItems = objects;

        }

        public override void Run()
        {
            this.SourceItems.IsArgumentNull(nameof(this.SourceItems));
            this.Filename.IsArgumentNullOrEmpty(nameof(this.Filename));

            try
            {
                this.SheetName = Path.GetFileNameWithoutExtension(this.Filename);

                using (RunFileExplorer runFile = new RunFileExplorer())
                {
                    this.CreateDocument(this.SourceItems as DataTable, this.Filename, this.TranslateDictionary);
                    runFile.SelectFile(this.Filename);
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }

        public override void CreateDocument(ICollectionView objects, string fileName,
                                                    Dictionary<string, string> translateColumnText = null)
        {
            this.SourceItems.IsArgumentNull(nameof(objects));
            this.Filename.IsArgumentNullOrEmpty(nameof(fileName));

            string errorText = "Der Excel-Export für das SourceItems ICollectionView wurde nicht implementiert!";
            throw new NotImplementedException(errorText);
        }

        public override void CreateDocument(DataTable objects, string fileName,
                                            Dictionary<string, string> translateColumnText = null)
        {
            this.SourceItems.IsArgumentNull(nameof(objects));
            this.Filename.IsArgumentNullOrEmpty(nameof(fileName));

            int rowHeader = 1;
            string firstColumn = string.Empty;
            string lastColumn = string.Empty;

            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    IXLWorksheet ws = wb.Worksheets.Add(this.SheetName.TruncateLeft(30));
                    ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                    ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
                    ws.PageSetup.ShowGridlines = true;
                    ws.PageSetup.DraftQuality = true;
                    ws.PageSetup.ShowRowAndColumnHeadings = true;

                    ws.FreezeRows(rowHeader);
                    ws.FreezeColumns(1);

                    /* Cell Header */
                    ws.CellHeader($"A{rowHeader}", translateColumnText["Chapter"], 18);
                    ws.CellHeader($"B{rowHeader}", translateColumnText["Title"], 40);
                    ws.CellHeader($"C{rowHeader}", translateColumnText["Min"], 16);
                    ws.CellHeader($"D{rowHeader}", translateColumnText["Mid"], 17);
                    ws.CellHeader($"E{rowHeader}", translateColumnText["Max"], 16);
                    ws.CellHeader($"F{rowHeader}", translateColumnText["Factor"], 10);
                    ws.CellHeader($"G{rowHeader}", "PERT", 15);
                    ws.CellHeader($"H{rowHeader}", "Tag", 30);

                    var isDesc = objects.Rows.Cast<DataRow>().Any(a => a.Field<bool>("ShowDescription") == true);
                    if (isDesc == true)
                    {
                        ws.CellHeader($"I{rowHeader}", translateColumnText["Description"], 40);
                    }

                    ws.SetAutoFilter(rowHeader);

                    string cellRef = string.Empty;
                    int startRow = 1;
                    int startCol = 0;

                    string formatF2 = ClosedXMLExtension.NumberFormat("F2");

                    foreach (DataRow row in objects.Rows)
                    {
                        startCol++;
                        startRow++;

                        cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                        string chapter = row["Chapter"].ToString();
                        ws.SetValue<string>(cellRef, chapter);

                        startCol++;
                        cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                        string title = row["Title"].ToString();
                        ws.SetValue<string>(cellRef, title);

                        startCol++;
                        cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                        double min = row["Min"].ToOrDefault<double>();
                        ws.SetValue<double>(cellRef, min, formatF2);

                        startCol++;
                        cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                        double mid = row["Mid"].ToOrDefault<double>();
                        ws.SetValue<double>(cellRef, mid, formatF2);

                        startCol++;
                        cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                        double max = row["Max"].ToOrDefault<double>();
                        ws.SetValue<double>(cellRef, max, formatF2);

                        startCol++;
                        cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                        double factor = row["Factor"].ToOrDefault<double>();
                        if (factor < 1)
                        {
                            ws.SetValue<double>(cellRef, factor,XLColor.Green, formatF2);
                        }
                        else if (factor == 1)
                        {
                            ws.SetValue<double>(cellRef, factor, XLColor.Black, formatF2);
                        }
                        else if (factor > 1)
                        {
                            ws.SetValue<double>(cellRef, factor, XLColor.Red, formatF2);
                        }

                        startCol++;
                        cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                        double PERTeffort = 0D;
                        using (CalcPERT cp = new CalcPERT())
                        {
                            PERTeffort = cp.PERTEffort(min, mid, max, factor);
                        }
                        ws.SetValue<double>(cellRef, PERTeffort, formatF2);

                        startCol++;
                        cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                        string tagText = row["Tag"].ToOrDefault<string>();
                        ws.SetValue<string>(cellRef, tagText);

                        if (isDesc == true)
                        {
                            startCol++;
                            cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                            string description = row["Description"].ToOrDefault<string>();
                            ws.SetValue<string>(cellRef, description);
                        }

                        startCol = 0;
                    }

                    wb.SaveAs(fileName);
                }
            }
            catch (Exception ex)
            {
                ExceptionViewer.Show(ex, this.GetType().Name);
            }
        }
    }
}