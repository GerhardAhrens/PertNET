//-----------------------------------------------------------------------
// <copyright file="PERTEffortExport.cs" company="Lifeprojects.de">
//     Class: PERTEffortExport
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>18.07.2022 07:25:06</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Export.Exporter
{
    using ClosedXML.Excel;

    using EasyPrototypingNET.IO;

    using PertNET.Model;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    [SupportedOSPlatform("windows")]
    public class PERTEffortExport : ReportsExcelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PERTEffortExport"/> class.
        /// </summary>
        public PERTEffortExport(DataTable objects) : base(objects)
        {
            this.SourceItems = objects;

        }

        public WorkUserInfo WorkUserInfo  { get; set; }

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
                MessageBox.Show(ex.Message);
                throw;
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

            int rowHeader = 3;
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
                    string projectname = $"{WorkUserInfo?.Company}, {WorkUserInfo?.Project}";
                    string ersteller = $"{WorkUserInfo?.Firstname} {WorkUserInfo?.Lastname}";
                    string lastDate = WorkUserInfo?.StartDate.ToShortDateString();
                    ws.SetValue<string>("A1",$"Aufwandsschätzung zu {projectname}");
                    ws.Range("A1", "B1").Merge().Row(1);

                    ws.SetValue<string>("D1", $"Ersteller: {ersteller}");
                    ws.Range("D1", "E1").Merge().Row(1);

                    ws.SetValue<string>("F1", $"Stand: {lastDate}");
                    ws.Range("F1", "G1").Merge().Row(1);

                    ws.CellHeader("A3", translateColumnText["Chapter"], 18);
                    ws.CellHeader("B3", translateColumnText["Title"], 40);
                    ws.CellHeader("C3", translateColumnText["Min"], 16);
                    ws.CellHeader("D3", translateColumnText["Mid"], 17);
                    ws.CellHeader("E3", translateColumnText["Max"], 16);
                    ws.CellHeader("F3", translateColumnText["Factor"], 10);
                    ws.CellHeader("G3", "PERT", 15);
                    ws.CellHeader("H3", "Tag", 30);

                    var isDesc = objects.Rows.Cast<DataRow>().Any(a => a.Field<bool>("ShowDescription") == true);
                    if (isDesc == true)
                    {
                        ws.CellHeader("I3", translateColumnText["Description"], 40);
                    }

                    ws.SetAutoFilter(rowHeader);

                    string cellRef = string.Empty;
                    int startRow = 3;
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

                    startRow++;
                    double sumMin = objects.AsEnumerable().Sum(x => x.Field<double>("Min"));
                    double sumMid = objects.AsEnumerable().Sum(x => x.Field<double>("Mid"));
                    double sumMax = objects.AsEnumerable().Sum(x => x.Field<double>("Max"));
                    double sumPERT = 0;
                    foreach (DataRow row in objects.Rows)
                    {
                        using (CalcPERT cp = new CalcPERT())
                        {
                            sumPERT += cp.PERTEffort(row.Field<double>("Min"), row.Field<double>("Mid"), row.Field<double>("Max"), row.Field<double>("Factor"));
                        }
                    }

                    ws.SetValue<string>($"B{startRow}", "Summe",true);
                    ws.SetValue<double>($"C{startRow}", sumMin, formatF2, true);
                    ws.SetValue<double>($"D{startRow}", sumMid, formatF2, true);
                    ws.SetValue<double>($"E{startRow}", sumMax, formatF2, true);
                    ws.SetValue<double>($"G{startRow}", sumPERT, formatF2, true);
                    ws.Range($"A{startRow}:G{startRow}").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    startRow++;
                    ws.Range($"C{startRow}:G{startRow}").Style.Border.TopBorder = XLBorderStyleValues.Double;

                    wb.SaveAs(fileName);
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }
    }
}