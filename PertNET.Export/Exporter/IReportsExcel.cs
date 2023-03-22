//-----------------------------------------------------------------------
// <copyright file="IReportsExcel.cs" company="Lifeprojects.de">
//     Class: IReportsExcel
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>14.07.2022</date>
//
// <summary>
// Interface Class for Excel Reporting
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Export.Exporter
{
    using System.Collections.Generic;
    using System.Data;

    public interface IReportsExcel
    {
        object SourceItems { get; set; }

        List<string> Columns { get; set; }

        string SheetName { get; set; }

        string Filename { get; set; }

        Dictionary<string, string> TranslateDictionary { get; set; }

        void Run();

        void CreateDocument(DataTable objects, string outPath, Dictionary<string, string> translateColumnText = null);

        bool IsExcelRunning(string fileName);
    }
}
