//-----------------------------------------------------------------------
// <copyright file="ReportsExcelDefault.cs" company="Lifeprojects.de">
//     Class: ReportsExcelDefault
//     Copyright © Lifeprojects.de 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>28.09.2020</date>
//
// <summary>Class for Default Excel Reporting</summary>
//-----------------------------------------------------------------------

namespace PertNET.Export.Exporter
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Versioning;
    using System.Windows.Input;

    using ClosedXML.Excel;

    [SupportedOSPlatform("windows")]
    public class ReportsExcelDefault : ReportsExcelBase
    {
        private Dictionary<string, string> translateText = null;

        public ReportsExcelDefault(DataTable objects) : base(objects)
        {
        }

        public ReportsExcelDefault(ICollectionView objects) : base(objects)
        {
        }

        public override void CreateDocument(ICollectionView objects, string fileName, Dictionary<string, string> translateColumnText = null)
        {
            objects.IsArgumentNull(nameof(objects));

            string tableName = Path.GetFileNameWithoutExtension(fileName);

            try
            {
                PropertyInfo[] fields = objects.CurrentItem.GetType().GetProperties();

                if (translateColumnText == null)
                {
                    this.translateText = new Dictionary<string, string>();
                    foreach (PropertyInfo propInfo in fields)
                    {
                        this.translateText.Add(propInfo.Name, propInfo.Name);
                    }
                }
                else
                {
                    this.translateText = translateColumnText;
                }

                string firstColumn = string.Empty;
                string lastColumn = string.Empty;

                Mouse.OverrideCursor = Cursors.Wait;

                using (XLWorkbook wb = new XLWorkbook())
                {
                    IXLWorksheet ws = wb.Worksheets.Add(tableName);
                    ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                    ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
                    ws.PageSetup.ShowGridlines = true;
                    ws.PageSetup.DraftQuality = true;
                    ws.PageSetup.ShowRowAndColumnHeadings = true;

                    ws.FreezeRows(this.FreezeRows);
                    ws.FreezeColumns(this.FreezeColumns);

                    string cellRef = string.Empty;
                    int col = 0;
                    objects.MoveCurrentToFirst();

                    firstColumn = ws.GetExcelColumnName(col);
                    int headerRow = 1;
                    foreach (PropertyInfo propInfo in fields)
                    {
                        string columnName = string.Empty;
                        if (this.translateText.ContainsKey(propInfo.Name) == true)
                        {
                            columnName = this.translateText[propInfo.Name];
                        }

                        col++;
                        cellRef = $"{ws.GetExcelColumnName(col)}{headerRow}";
                        ws.CellHeader(cellRef, columnName, 20);
                    }

                    col = fields.CountEx();
                    lastColumn = ws.GetExcelColumnName(col);

                    ws.SetAutoFilter(1);

                    string formatValue = string.Empty;
                    int startRow = 2;
                    while (objects.IsCurrentAfterLast == false)
                    {
                        PropertyInfo columnName = objects.CurrentItem.GetType().GetProperties().SingleOrDefault(c => c.Name == "HiddenFormat");
                        if (columnName != null)
                        {
                            formatValue = objects.CurrentItem.GetType().GetProperty(columnName.Name).GetValue(objects.CurrentItem, null).ToString();
                        }

                        if (objects.CurrentItem.GetType().IsVisible)
                        {
                            int startCol = 0;
                            foreach (PropertyInfo propInfo in fields)
                            {
                                startCol++;
                                var newValue = objects.CurrentItem.GetType().GetProperty(propInfo.Name).GetValue(objects.CurrentItem, null);
                                cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";
                                if (propInfo.PropertyType == typeof(DateTime))
                                {
                                    if (newValue.ToString().Contains(".1900") == true || newValue.ToString().Contains(".0001") == true)
                                    {
                                        ws.Cell(cellRef).Value = string.Empty;
                                    }
                                    else
                                    {
                                        ws.Cell(cellRef).Value = ((DateTime)newValue).ToShortDateString();
                                    }

                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    //ws.Cell(cellRef).DataType = XLDataType.DateTime;
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                }
                                else if (propInfo.PropertyType == typeof(bool))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    ws.Cell(cellRef).Value = (bool)newValue;
                                    //ws.Cell(cellRef).DataType = XLDataType.Boolean;
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                }
                                else if (propInfo.PropertyType == typeof(int))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    ws.Cell(cellRef).Value = (int)newValue;
                                    //ws.Cell(cellRef).DataType = XLDataType.Number;
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                }
                                else if (propInfo.PropertyType == typeof(double))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    ws.Cell(cellRef).Value = (double)newValue;
                                    //ws.Cell(cellRef).DataType = XLDataType.Number;
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                }
                                else if (propInfo.PropertyType == typeof(decimal))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    ws.Cell(cellRef).Value = (decimal)newValue;
                                    //ws.Cell(cellRef).DataType = XLDataType.Number;
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    if (this.IsDecimalCurrency == true)
                                    {
                                        ws.Cell(cellRef).Style.NumberFormat.Format = "#.00 €";
                                    }
                                }
                                else
                                {
                                    if (columnName?.Name != "HiddenFormat")
                                    {
                                        ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                        ws.Cell(cellRef).Value = newValue.ToString();
                                        //ws.Cell(cellRef).DataType = XLDataType.Text;
                                        ws.Cell(cellRef).Style.Font.Bold = false;
                                        ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(formatValue) == false)
                            {
                                ws.Cell(cellRef).Style.Fill.BackgroundColor = XLColor.FromName(formatValue.Replace("Bold", string.Empty));

                                if (formatValue.ToLower().Contains("bold") == true)
                                {
                                    ws.Cell(cellRef).Style.Font.Bold = true;
                                }
                            }
                        }

                        startRow++;
                        objects.MoveCurrentToNext();
                    }

                    wb.SaveAs(fileName);
                }
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        public override void CreateDocument(DataTable objects, string fileName, Dictionary<string, string> translateColumnText = null)
        {
            string tableName = Path.GetFileNameWithoutExtension(fileName);

            try
            {
                if (translateColumnText == null)
                {
                    this.translateText = new Dictionary<string, string>();
                    foreach (string itemText in this.Columns)
                    {
                        this.translateText.Add(itemText, itemText);
                    }
                }
                else
                {
                    this.translateText = translateColumnText;
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    IXLWorksheet ws = wb.Worksheets.Add(tableName);
                    ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                    ws.PageSetup.PaperSize = XLPaperSize.A4Paper;
                    ws.PageSetup.ShowGridlines = true;
                    ws.PageSetup.DraftQuality = true;
                    ws.PageSetup.ShowRowAndColumnHeadings = true;

                    ws.FreezeRows(this.FreezeRows);
                    ws.FreezeColumns(this.FreezeColumns);

                    string cellRef = string.Empty;

                    int col = 0;
                    int headerRow = 1;
                    foreach (DataColumn column in objects.Columns)
                    {
                        string columnName = string.Empty;
                        if (this.translateText.ContainsKey(column.ColumnName) == true)
                        {
                            columnName = this.translateText[column.ColumnName];
                        }

                        col++;
                        cellRef = $"{ws.GetExcelColumnName(col)}{headerRow}";
                        ws.CellHeader(cellRef, columnName, 20);
                    }

                    ws.SetAutoFilter(1);

                    int startRow = 2;
                    foreach (DataRow row in objects.Rows)
                    {
                        int startCol = 0;
                        foreach (DataColumn rowColumn in row.Table.Columns)
                        {
                            startCol++;
                            cellRef = $"{ws.GetExcelColumnName(startCol)}{startRow}";

                            if (this.translateText.ContainsKey(rowColumn.ColumnName) == true)
                            {
                                if (rowColumn.DataType == typeof(DateTime))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    //ws.Cell(cellRef).DataType = XLDataType.DateTime;
                                    ws.SetCellValue(cellRef, row[rowColumn]);
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(cellRef).Style.Alignment.ShrinkToFit = true;
                                }
                                else if (rowColumn.DataType == typeof(bool))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    //ws.Cell(cellRef).DataType = XLDataType.Boolean;
                                    ws.SetCellValue(cellRef, row[rowColumn]);
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(cellRef).Style.Alignment.ShrinkToFit = true;
                                }
                                else if (rowColumn.DataType.In(typeof(int), typeof(long)))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    //ws.Cell(cellRef).DataType = XLDataType.Number;
                                    ws.SetCellValue(cellRef, row[rowColumn]);
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(cellRef).Style.Alignment.ShrinkToFit = true;
                                }
                                else if (rowColumn.DataType.In(typeof(double), typeof(float)))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    //ws.Cell(cellRef).DataType = XLDataType.Number;
                                    ws.SetCellValue(cellRef, row[rowColumn]);
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(cellRef).Style.Alignment.ShrinkToFit = true;
                                    ws.Cell(cellRef).Style.NumberFormat.Format = "#,##0.00";
                                }
                                else if (rowColumn.DataType.In(typeof(decimal)))
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    //ws.Cell(cellRef).DataType = XLDataType.Number;
                                    ws.SetCellValue(cellRef, row[rowColumn]);
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(cellRef).Style.Alignment.ShrinkToFit = true;
                                    if (this.IsDecimalCurrency == true)
                                    {
                                        ws.Cell(cellRef).Style.NumberFormat.Format = "#,##0.00 €";
                                    }
                                    else
                                    {
                                        ws.Cell(cellRef).Style.NumberFormat.Format = "#,##0.00";
                                    }
                                }
                                else
                                {
                                    ws.Cell(cellRef).Style.Font.FontName = "Minion";
                                    //ws.Cell(cellRef).DataType = XLDataType.Text;
                                    ws.SetCellValue(cellRef, row[rowColumn]);
                                    ws.Cell(cellRef).Style.Font.Bold = false;
                                    ws.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                                    ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                                    ws.Cell(cellRef).Style.Alignment.ShrinkToFit = true;
                                    ws.Cell(cellRef).Style.Alignment.WrapText = true;
                                }
                            }
                        }

                        startRow++;
                    }

                    wb.SaveAs(fileName);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}