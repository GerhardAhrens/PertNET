//-----------------------------------------------------------------------
// <copyright file="ClosedXMLExtension.cs" company="Lifeprojects.de">
//     Class: ClosedXMLExtension
//     Copyright © Lifeprojects.de 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>6.8.2020</date>
//
// <summary>Class for ClosedXMLExtension</summary>
//-----------------------------------------------------------------------

namespace ClosedXML.Excel
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using DocumentFormat.OpenXml.Spreadsheet;

    public static class ClosedXMLExtension
    {
        public static void FreezeRows(this IXLWorksheet @this, int row)
        {
            @this.SheetView.FreezeRows(row);
        }

        public static void FreezeColumns(this IXLWorksheet @this, int col)
        {
            @this.SheetView.FreezeColumns(col);
        }

        public static void SetAutoFilter(this IXLWorksheet @this, int row)
        {
            IXLRows rowRange = @this.Rows(row, row);
            if (rowRange != null)
            {
                IXLCells cells = rowRange.CellsUsed();
                if (cells != null && cells.Count() > 0)
                {
                    string firstCell = $"{cells.First().Address.ColumnLetter}{cells.First().Address.RowNumber}";
                    string lastCell = $"{cells.Last().Address.ColumnLetter}{cells.Last().Address.RowNumber}";
                    @this.Range(firstCell, lastCell).SetAutoFilter(true);
                }
            }
        }

        public static void CellArguments(this IXLWorksheet ws, string cellRef, string value, XLColor backColor = null)
        {
            if (backColor == null)
            {
                backColor = XLColor.LightGray;
            }

            ws.Cell(cellRef).Value = value;
            ws.Cell(cellRef).Style.CellArgumentStyle(backColor);
        }

        public static IXLStyle CellArgumentStyle(this IXLStyle style, XLColor backColor = null)
        {
            if (backColor == null)
            {
                backColor = XLColor.Transparent;
            }

            style.Font.Bold = true;
            style.Fill.SetBackgroundColor(backColor);
            style.Border.BottomBorder = XLBorderStyleValues.Thick;
            style.Border.BottomBorderColor = XLColor.Black;
            return style;
        }

        public static int GetHeadersCount(this IXLWorksheet @this)
        {
            IXLRow firstRow = @this.Row(1);

            IXLRange rangeWithoutHeader = @this.Range(firstRow.FirstCellUsed(), firstRow.LastCellUsed());

            return rangeWithoutHeader.ColumnCount();
        }

        public static string ColumnName(this int @this)
        {
            int dividend = @this;
            string columnName = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static IXLWorksheet SetValue<TTyp>(this IXLWorksheet @this, string cellRef, object value, XLColor backgroundColor, XLColor fontColor,bool setBold, string format = "", bool setBorder = false)
        {
            if (setBorder == true)
            {
                @this.Cell(cellRef).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                @this.Cell(cellRef).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                @this.Cell(cellRef).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                @this.Cell(cellRef).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            }
            else
            {
                @this.Cell(cellRef).Style.Border.BottomBorder = XLBorderStyleValues.Hair;
                @this.Cell(cellRef).Style.Border.TopBorder = XLBorderStyleValues.Hair;
                @this.Cell(cellRef).Style.Border.LeftBorder = XLBorderStyleValues.Hair;
                @this.Cell(cellRef).Style.Border.RightBorder = XLBorderStyleValues.Hair;
            }

            if (value == DBNull.Value)
            {
                @this.Cell(cellRef).Value = string.Empty;
                @this.Cell(cellRef).Style.Font.Bold = setBold;
                @this.Cell(cellRef).Style.Font.SetFontColor(fontColor);
                @this.Cell(cellRef).Style.Alignment.WrapText = true;
                @this.Cell(cellRef).Style.Fill.BackgroundColor = backgroundColor;
                @this.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                @this.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                @this.Cell(cellRef).Style.Alignment.ShrinkToFit = false;
                return @this;
            }

            if (typeof(TTyp) == typeof(string))
            {
                if ((string)value == "0")
                {
                    @this.Cell(cellRef).Value = string.Empty;
                }
                else
                {
                    @this.Cell(cellRef).Value = $"'{value}";
                }

                @this.Cell(cellRef).Style.Font.Bold = setBold;
                @this.Cell(cellRef).Style.Font.SetFontColor(fontColor);
                @this.Cell(cellRef).Style.Alignment.WrapText = true;
                @this.Cell(cellRef).Style.Fill.BackgroundColor = backgroundColor;
                @this.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                @this.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                @this.Cell(cellRef).Style.Alignment.ShrinkToFit = false;
            }
            else if (typeof(TTyp) == typeof(DateTime))
            {
                if (((DateTime)value).IsDateEmpty() == true)
                {
                    @this.Cell(cellRef).Value = string.Empty;
                }
                else
                {
                    @this.Cell(cellRef).Value = (DateTime)value;
                }

                @this.Cell(cellRef).Style.Font.Bold = setBold;
                @this.Cell(cellRef).Style.Font.SetFontColor(fontColor);
                @this.Cell(cellRef).Style.Alignment.WrapText = true;
                @this.Cell(cellRef).Style.Fill.BackgroundColor = backgroundColor;
                @this.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                @this.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                @this.Cell(cellRef).Style.Alignment.ShrinkToFit = false;
            }
            else if (typeof(TTyp) == typeof(double))
            {
                if ((double)value == 0)
                {
                    @this.Cell(cellRef).Value = string.Empty;
                }
                else
                {
                    @this.Cell(cellRef).Value = (double)value;
                }

                @this.Cell(cellRef).Style.Font.Bold = setBold;
                @this.Cell(cellRef).Style.Font.SetFontColor(fontColor);
                @this.Cell(cellRef).Style.Alignment.WrapText = true;
                @this.Cell(cellRef).Style.Fill.BackgroundColor = backgroundColor;
                @this.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                @this.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                @this.Cell(cellRef).Style.Alignment.ShrinkToFit = false;
                @this.Cell(cellRef).Style.NumberFormat.Format = format;
            }
            else if (typeof(TTyp) == typeof(decimal))
            {
                if ((decimal)value == 0)
                {
                    @this.Cell(cellRef).Value = string.Empty;
                }
                else
                {
                    @this.Cell(cellRef).Value = (decimal)value;
                }

                @this.Cell(cellRef).Style.Font.Bold = setBold;
                @this.Cell(cellRef).Style.Font.SetFontColor(fontColor);
                @this.Cell(cellRef).Style.Alignment.WrapText = true;
                @this.Cell(cellRef).Style.Fill.BackgroundColor = backgroundColor;
                @this.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                @this.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                @this.Cell(cellRef).Style.Alignment.ShrinkToFit = false;
                @this.Cell(cellRef).Style.NumberFormat.Format = format;
            }
            else if (typeof(TTyp) == typeof(int))
            {
                if ((int)value == 0)
                {
                    @this.Cell(cellRef).Value = string.Empty;
                }
                else
                {
                    @this.Cell(cellRef).Value = (int)value;
                }

                @this.Cell(cellRef).Style.Font.Bold = setBold;
                @this.Cell(cellRef).Style.Font.SetFontColor(fontColor);
                @this.Cell(cellRef).Style.Alignment.WrapText = true;
                @this.Cell(cellRef).Style.Fill.BackgroundColor = backgroundColor;
                @this.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                @this.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                @this.Cell(cellRef).Style.Alignment.ShrinkToFit = false;
            }
            else if (typeof(TTyp) == typeof(bool))
            {
                if ((bool)value == true)
                {
                    @this.Cell(cellRef).Value = "Ja";
                }
                else
                {
                    @this.Cell(cellRef).Value = "nein";
                }

                @this.Cell(cellRef).Style.Font.Bold = setBold;
                @this.Cell(cellRef).Style.Font.SetFontColor(fontColor);
                @this.Cell(cellRef).Style.Alignment.WrapText = true;
                @this.Cell(cellRef).Style.Fill.BackgroundColor = backgroundColor;
                @this.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                @this.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                @this.Cell(cellRef).Style.Alignment.ShrinkToFit = false;
            }

            return @this;
        }

        public static IXLWorksheet SetValue<TTyp>(this IXLWorksheet @this, string cellRef, object value)
        {
            return SetValue<TTyp>(@this, cellRef, value, XLColor.Transparent, XLColor.Black,false, setBorder: false);
        }

        public static IXLWorksheet SetValue<TTyp>(this IXLWorksheet @this, string cellRef, object value, bool setBold)
        {
            return SetValue<TTyp>(@this, cellRef, value, XLColor.Transparent, XLColor.Black, setBold, setBorder: false);
        }

        public static IXLWorksheet SetValue<TTyp>(this IXLWorksheet @this, string cellRef, object value, string format)
        {
            return SetValue<TTyp>(@this, cellRef, value, XLColor.Transparent, XLColor.Black,false, format, setBorder: false);
        }

        public static IXLWorksheet SetValue<TTyp>(this IXLWorksheet @this, string cellRef, object value, string format, bool setBold)
        {
            return SetValue<TTyp>(@this, cellRef, value, XLColor.Transparent, XLColor.Black, setBold, format, setBorder: false);
        }

        public static IXLWorksheet SetValue<TTyp>(this IXLWorksheet @this, string cellRef, object value, XLColor fontColor, string format = "")
        {
            return SetValue<TTyp>(@this, cellRef, value, XLColor.Transparent, fontColor, false, format, setBorder: false);
        }

        public static IXLWorksheet SetValue<TTyp>(this IXLWorksheet @this, string cellRef, object value, XLColor fontColor, bool setBold,string format = "")
        {
            return SetValue<TTyp>(@this, cellRef, value, XLColor.Transparent, fontColor, setBold, format, setBorder: false);
        }

        public static T GetCellValue<T>(this IXLCell @this)
        {
            object value = null;
            object getAs = null;

            try
            {
                value = @this.Value;
                /*
                if (@this.CachedValue == null)
                {
                    value = @this.Value;
                }
                else
                {
                    value = @this.CachedValue;
                }
                */

                if (value.ToString() == "#DIV/0!")
                {
                    return default(T);
                }
                else if (value.ToString() == "#N/A")
                {
                    return default(T);
                }
                else
                {
                    if (string.IsNullOrEmpty(value.ToString()) == false)
                    {
                        if (typeof(T) == typeof(decimal))
                        {
                            if (value.ToString().ToUpper().Contains("E") == true)
                            {
                                value = double.Parse(value.ToString(), CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(value.ToString()) == false)
                                {
                                    value = value.ToString().Replace(".", ",");
                                }
                            }
                        }

                        if (typeof(T) == typeof(double))
                        {
                            if (value.ToString().ToUpper().Contains("E") == true)
                            {
                                value = double.Parse(value.ToString(), CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(value.ToString()) == false)
                                {
                                    value = value.ToString().Replace(".", ",");
                                }
                            }
                        }

                        Type t = typeof(T);
                        t = Nullable.GetUnderlyingType(t) ?? t;
                        getAs = (value == null || DBNull.Value.Equals(value)) ? default(T) : (T)Convert.ChangeType(value, t);
                    }
                    else
                    {
                        if (typeof(T) == typeof(string))
                        {
                            Type t = typeof(T);
                            t = Nullable.GetUnderlyingType(t) ?? t;
                            return (T)Convert.ChangeType(string.Empty, t);
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                }

                return (T)getAs;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static string GetExcelColumnName(this IXLWorksheet @this, int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;

            if (columnNumber == 0)
            {
                return "A";
            }

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        public static string GetCellReference(this IXLWorksheet @this, int columnNumber, int row)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            string cellRef = $"{columnName}{row}";

            return cellRef;
        }

        public static string NumberFormat(string format)
        {
            string result = string.Empty;
            string currencyText = string.Empty;

            string number = Regex.Match(format, @"\d+").Value;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Convert.ToInt32(number); i++)
            {
                sb.Append("0");
            }

            if (format.Substring(0, 1) == "C")
            {
                currencyText = CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
                result = $"#,###.{sb.ToString()} {currencyText}";
            }

            if (format.Substring(0, 1) == "F")
            {
                result = $"#,###.{sb.ToString()}";
            }

            if (format.Length ==2 && format.Substring(0, 2) == "F2")
            {
                result = $"#,##0.{sb.ToString()}";
            }

            if (format.Substring(0, 1) == "P")
            {
                result = $"0.{sb.ToString()} %";
            }

            return result;
        }

        public static void CellHeader(this IXLWorksheet ws, string cellRef, string value, double width, XLColor backColor = null)
        {
            if (backColor == null)
            {
                backColor = XLColor.LightGray;
            }

            string column = GetFirstLetters(cellRef);
            ws.Column(column).Width = width;
            ws.Cell(cellRef).Style.Font.FontName = "Minion";
            ws.Cell(cellRef).Value = value;
            ws.Cell(cellRef).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(cellRef).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell(cellRef).Style.CellHeaderStyle(backColor);
            ws.Cell(cellRef).Style.Alignment.ShrinkToFit = false;

            /*
            string number = Regex.Match(cellRef, @"\d+").Value;
            int col = number.ToInt();
            ws.Column(col).AdjustToContents();
            */
        }

        public static IXLStyle CellHeaderStyle(this IXLStyle style, XLColor backColor = null)
        {
            if (backColor == null)
            {
                backColor = XLColor.LightGray;
            }

            style.Font.Bold = true;
            style.Fill.SetBackgroundColor(backColor);
            style.Border.BottomBorder = XLBorderStyleValues.Thin;
            style.Border.BottomBorderColor = XLColor.Black;
            return style;
        }

        public static void SetCellValue(this IXLWorksheet @this, string cellRef, object value)
        {
            if (value == DBNull.Value)
            {
                @this.Cell(cellRef).Value = string.Empty;
            }
            else
            {
                if (@this.Cell(cellRef).DataType == XLDataType.DateTime)
                {
                    if (value.ToString().Contains(".1900") == true)
                    {
                        @this.Cell(cellRef).Value = string.Empty;
                    }
                    else if (value.ToString().Contains(".0001") == true)
                    {
                        @this.Cell(cellRef).Value = string.Empty;
                    }
                    else
                    {
                        @this.Cell(cellRef).Value = value.ToString();
                    }
                }
                else if (@this.Cell(cellRef).DataType == XLDataType.Boolean)
                {
                    if ((bool)value == true)
                    {
                        @this.Cell(cellRef).Value = "Ja";
                    }
                    else
                    {
                        @this.Cell(cellRef).Value = "Nein";
                    }
                }
                else
                {
                    @this.Cell(cellRef).Value = value.ToString();
                }
            }
        }

        private static string GetFirstLetters(string s, char separator = ':')
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s.AsParallel())
            {
                if (c == separator)
                {
                    break;
                }
                else
                {
                    if (char.IsLetter(c) == true)
                    {
                        sb.Append(c);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
