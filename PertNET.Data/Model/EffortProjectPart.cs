//-----------------------------------------------------------------------
// <copyright file="EffortProject.cs" company="www.lifeprojects.de">
//     Class: EffortProject
//     Copyright © www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>30.06.2022 13:36:09</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Model
{
    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Core.Application;
    using EasyPrototypingNET.Interface;

    using PertNET.Data.Core;

    public partial class EffortProject : ModelBase<EffortProject>, IModel
    {
        public string FullName
        {
            get
            {
                return $"{this.ChapterA}.{this.ChapterB}.{this.ChapterC}-{this.Title}";
            }
        }

        [SearchFilter]
        [ExportField]
        public string Chapter
        {
            get
            {
                return $"{this.ChapterA}.{this.ChapterB}.{this.ChapterC}";
            }
        }

        public string ChapterSorter
        {
            get
            {
                return $"{this.ChapterA.ToString("000")}.{this.ChapterB.ToString("000")}.{this.ChapterC.ToString("000")}";
            }
        }

        public string Timestamp
        {
            get
            {
                string result = string.Empty;

                using (TimeStamp ts = new TimeStamp())
                {
                    result = ts.MaxEntry(this.CreatedOn, this.CreatedBy, this.ModifiedOn, this.ModifiedBy);
                }

                return result;
            }
        }
    }
}
