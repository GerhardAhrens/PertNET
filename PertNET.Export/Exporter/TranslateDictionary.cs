//-----------------------------------------------------------------------
// <copyright file="TranslateDictionary.cs" company="Lifeprojects.de">
//     Class: TranslateDictionary
//     Copyright © Lifeprojects.de 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>04.12.2020</date>
//
// <summary>
// Die Klasse stellt eine Übersetzungsliste der Columns von DataTable
// zur Verfügung. Beim Translate wird ebenfalls geprüft, ob die 
// angegebene Spalte vorhanden ist. 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Export.Exporter
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class TranslateDictionary : Dictionary<string, string>, IDisposable
    {
        private readonly Dictionary<string, Type> columnDict = null;

        public TranslateDictionary(DataColumnCollection columnCollection)
        {
            if (columnCollection != null || columnCollection.Count > 0)
            {
                this.columnDict = new Dictionary<string, Type>();

                foreach (DataColumn column in columnCollection)
                {
                    this.columnDict.Add(column.ColumnName, column.DataType);
                }
            }
        }

        public TranslateDictionary(Dictionary<string, Type> columnCollection)
        {
            if (columnCollection != null || columnCollection.Count > 0)
            {
                this.columnDict = columnCollection;
            }
        }

        public void AddTranslateText(string column, string translateText)
        {
            if (this.ContainsKey(column) == false)
            {
                this.Add(column, translateText);
            }
        }

        public Dictionary<string, string> Translate()
        {
            foreach (string column in this.Keys.ToList())
            {
                if (this.columnDict.ContainsKey(column) == false)
                {
                    this[column] = $"Column '{column}' not found";
                }
            }

            return this;
        }

        public void Dispose()
        {
        }
    }
}
