/*
 * <copyright file="ModelBase.cs" company="Lifeprojects.de">
 *     Class: ModelBase
 *     Copyright © Lifeprojects.de 2022
 * </copyright>
 *
 * <author>Gerhard Ahrens - Lifeprojects.de</author>
 * <email>developer@lifeprojects.de</email>
 * <date>26.06.2017</date>
 * <Project>EasyPrototypingNET</Project>
 *
 * <summary>
 * Abstrakte Klasse zur Erstellung einer Model Klasse 
 * </summary>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by the Free Software Foundation, 
 * either version 3 of the License, or (at your option) any later version.
 * This program is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.You should have received a copy of the GNU General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>.
*/

namespace PertNET.Data.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;

    [DebuggerStepThrough]
    [Serializable]
    public abstract class ModelBase<TModel> : DefaultValueInitializer, IModel, IDisposable
    {
        private static List<string> searchFieldsFilter;
        private static IDictionary<string, Type> exportFieldsFilter;
        private static IEnumerable<Tuple<string, Type, string, int>> exportFieldsTranslate;
        private static DataTable clipboardFields;
        private bool classIsDisposed = false;
        private bool isModified;

        public ModelBase()
        {
            if (SearchFieldsFilter == null)
            {
                SearchFieldsFilter = this.InternSearchFields<TModel>();
            }

            if (ExportFieldsFilter == null)
            {
                ExportFieldsFilter = this.GetExportFields<TModel>(this);
            }

            if (ExportFieldsTranslate == null)
            {
                ExportFieldsTranslate = this.GetExportFieldsTranslate<TModel>(this);
            }

            if (ClipboardFields == null)
            {
                ClipboardFields = this.ClipboardFieldsContent<TModel>(this);
            }
        }

        public static DataTable ClipboardFields
        {
            get { return clipboardFields; }
            set
            {
                if (clipboardFields != value)
                {
                    clipboardFields = value;
                }
            }
        }

        public static List<string> SearchFieldsFilter
        {
            get { return searchFieldsFilter; }
            set
            {
                if (searchFieldsFilter != value)
                {
                    searchFieldsFilter = value;
                }
            }
        }

        public static IDictionary<string, Type> ExportFieldsFilter
        {
            get { return exportFieldsFilter; }
            set
            {
                if (exportFieldsFilter != value)
                {
                    exportFieldsFilter = value;
                }
            }
        }

        public static IEnumerable<Tuple<string, Type, string, int>> ExportFieldsTranslate
        {
            get { return exportFieldsTranslate; }
            set
            {
                if (exportFieldsTranslate != value)
                {
                    exportFieldsTranslate = value;
                }
            }
        }

        [IgnorTableColumn]
        public bool IsModified
        {
            get { return this.isModified; }
            set
            {
                if (this.isModified != value)
                {
                    this.isModified = value;
                }
            }
        }

        public static T ToClone<T>(T source)
        {
            var constructorInfo = typeof(T).GetConstructor(new Type[] { });
            if (constructorInfo != null)
            {
                var target = (T)constructorInfo.Invoke(new object[] { });

                const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;
                var sourceProperties = source.GetType().GetProperties(Flags);

                foreach (PropertyInfo pi in sourceProperties)
                {
                    if (pi.CanWrite == true)
                    {
                        var propInfoObj = target.GetType().GetProperty(pi.Name);
                        if (propInfoObj != null)
                        {
                            var propValue = pi.GetValue(source, null);
                            propInfoObj.SetValue(target, propValue, null);
                        }
                    }
                }

                return target;
            }

            return default(T);
        }

        public static T ToClone<T>(object source)
        {
            var constructorInfo = typeof(T).GetConstructor(new Type[] { });
            if (constructorInfo != null)
            {
                var target = (T)constructorInfo.Invoke(new object[] { });

                const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;
                var sourceProperties = source.GetType().GetProperties(Flags);

                foreach (PropertyInfo pi in sourceProperties)
                {
                    if (pi.CanWrite == true)
                    {
                        var propInfoObj = target.GetType().GetProperty(pi.Name);
                        if (propInfoObj != null)
                        {
                            var propValue = pi.GetValue(source, null);
                            propInfoObj.SetValue(target, propValue, null);
                        }
                    }
                }

                return target;
            }

            return default(T);
        }

        public IDictionary<string, Type> ToExportFields()
        {
            return exportFieldsFilter;
        }

        public IEnumerable<Tuple<string, Type, string, int>> ToExportFieldsTranslate()
        {
            return exportFieldsTranslate;
        }

        public DataTable ToClipboardFields()
        {
            return this.ClipboardFieldsContent<TModel>(this);
        }

        public override string ToString()
        {
            try
            {
                PropertyInfo[] propInfo = this.GetType().GetProperties();
                StringBuilder outText = new StringBuilder();
                outText.AppendFormat("{0} ", this.GetType().Name);
                foreach (PropertyInfo propItem in propInfo)
                {
                    outText.AppendFormat("{0}:{1};", propItem.Name, propItem.GetValue(this, null));
                }

                return outText.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Compare<T>(T object1, T object2)
        {
            Type type = typeof(T);

            if (object1 == null || object2 == null)
            {
                return false;
            }

            foreach (PropertyInfo property in type.GetProperties())
            {
                if (property.Name != "ExtensionData")
                {
                    string object1Value = string.Empty;
                    string object2Value = string.Empty;

                    if (type.GetProperty(property.Name).GetValue(object1, null) != null)
                    {
                        object1Value = type.GetProperty(property.Name).GetValue(object1, null).ToString();
                    }

                    if (type.GetProperty(property.Name).GetValue(object2, null) != null)
                    {
                        object2Value = type.GetProperty(property.Name).GetValue(object2, null).ToString();
                    }

                    if (object1Value.Trim() != object2Value.Trim())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public string ToSearchFilter(char separator = '|',bool isUpper = true)
        {
            string result = string.Empty;

            try
            {
                PropertyInfo[] pil = this.GetType().GetProperties();
                StringBuilder outText = new StringBuilder();
                foreach (PropertyInfo item in pil.AsParallel())
                {
                    if (SearchFieldsFilter.Contains(item.Name) == true)
                    {
                        outText.AppendFormat("{0}{1}", item.GetValue(this, null), separator.ToString());
                    }
                }

                if (outText.ToString().EndsWith(separator.ToString()) == true)
                {
                    result = outText.Remove(outText.ToString().Length - 1, 1).ToString();
                }

                return isUpper == true ? result.ToUpper() : result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ToSearchFilterLINQ(char separator = '|',bool isUpper = true)
        {
            string result = string.Empty;

            IEnumerable<object> propertyContent = from property in this.GetType().GetProperties()
                                                  where SearchFieldsFilter.Contains(property.Name) == true
                                                  select property.GetValue(this, null);
            if (propertyContent != null)
            {
                result = string.Join(separator.ToString(), propertyContent);
            }

            return result;
        }

        public IEnumerable<string> ToSearchFields()
        {
            return searchFieldsFilter;
        }

        public int CalculateHash(params Func<object>[] memberThunks)
        {
            /* Overflow is okay; just wrap around */
            unchecked
            {
                int hash = 5;
                foreach (var member in memberThunks)
                {
                    if (member() != null)
                    {
                        hash = hash * 29 + member().GetHashCode();
                    }
                }
                return hash;
            }
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
                if (classDisposing)
                {
                    searchFieldsFilter = null;
                    exportFieldsFilter = null;
                }
            }

            this.classIsDisposed = true;
        }

        #endregion Dispose

        private DataTable ClipboardFieldsContent<T>(object obj)
        {
            DataTable dic = new DataTable($"Clipboard({obj.GetType().Name})");
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties.AsParallel())
            {
                foreach (var attribute in property.GetCustomAttributes(false).AsParallel())
                {
                    if (attribute.GetType() == typeof(ClipboardFieldAttribute))
                    {
                        Type typ = property.PropertyType;
                        dic.Columns.Add(property.Name, typ);
                    }
                }
            }

            DataRow dr = dic.NewRow();
            foreach (PropertyInfo property in properties.AsParallel())
            {
                foreach (var attribute in property.GetCustomAttributes(false).AsParallel())
                {
                    if (attribute.GetType() == typeof(ClipboardFieldAttribute))
                    {
                        dr[property.Name] = property.GetValue(this, null);
                    }
                }
            }
            dic.Rows.Add(dr);
            dic.AcceptChanges();

            return dic;
        }

        /*
        private IList<string> SearchFields<T>(object obj)
        {
            IList<string> propertySearchFields = new List<string>();
            var properties = obj.GetType().GetProperties();
            foreach (var property in properties.AsParallel())
            {
                var attributes = property.GetCustomAttributes(false);
                foreach (var attribute in attributes.AsParallel())
                {
                    if (attribute.GetType() == typeof(SearchFilterAttribute))
                    {
                        propertySearchFields.Add(property.Name);
                    }
                }
            }

            return propertySearchFields;
        }
        */

        private List<string> InternSearchFields<T>()
        {
            List<string> propertyFields = typeof(T).GetProperties().SelectMany(p => p.GetCustomAttributes())
                .OfType<SearchFilterAttribute>()
                .AsParallel()
                .Select(p => p.FieldName)
                .ToList();

            return propertyFields;
        }

        private IDictionary<string, Type> GetExportFields<T>(object obj)
        {
            IDictionary<string, Type> propertyFields = (from prop in obj.GetType().GetProperties()
                                                        let attrs = prop.GetCustomAttributes(typeof(ExportFieldAttribute), false)
                                                        where attrs.Any()
                                                        select new { PropertyName = prop.Name, prop.PropertyType})
                                                        .AsParallel()
                                                        .ToDictionary(group => group.PropertyName, group => group.PropertyType);

            return propertyFields;
        }

        private IEnumerable<Tuple<string, Type, string, int>> GetExportFieldsTranslate<T>(object obj)
        {

            IEnumerable<Tuple<string, Type, string, int>> propertyFields = obj.GetType()
                .GetProperties()
                .SelectMany(p => p.GetCustomAttributes())
                .OfType<ExportFieldAttribute>()
                .AsParallel()
                .Select(p => ExportModelFieldsInfo<T>(p))
                .ToList();

            return propertyFields.OrderBy(o => o.Item4);
        }

        private Tuple<string, Type, string, int> ExportModelFieldsInfo<T>(ExportFieldAttribute p)
        {
            PropertyInfo pi = typeof(T).GetProperty(p.FieldName);

            return new Tuple<string, Type, string, int>(p.FieldName, pi == null ? null : pi.PropertyType, p.DisplayName, p.SortOrder);
        }


        private IEnumerable<Tuple<string, Type, RequiredTyp>> RequiredFieldsSource<T>(object obj)
        {
            IEnumerable<Tuple<string, Type, RequiredTyp>> propertyFields = obj.GetType().GetProperties().SelectMany(p => p.GetCustomAttributes())
                    .OfType<RequiredAttribute>()
                    .AsParallel()
                    .Select(p => RequiredFieldsInfo<T>(p))
                    .ToList();

            return propertyFields;
        }

        private Tuple<string, Type, RequiredTyp> RequiredFieldsInfo<T>(RequiredAttribute p)
        {
            PropertyInfo pi = typeof(T).GetProperty(p.FieldName);

            return new Tuple<string, Type, RequiredTyp>(p.FieldName, pi == null ? null : pi.PropertyType, p.RequiredTyp);
        }
    }
}