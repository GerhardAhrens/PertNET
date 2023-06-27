//-----------------------------------------------------------------------
// <copyright file="Attachment.cs" company="www.lifeprojects.de">
//     Class: Attachment
//     Copyright © www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>30.06.2022 13:53:09</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Model
{
    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core.Application;
    using EasyPrototypingNET.Interface;

    using PertNET.Data.Core;

    public sealed partial class Attachment : ModelBase<Note>, IModel
    {
        public string FullName
        {
            get
            {
                return $"{this.ObjectName}-{this.Filename}-{this.FileSize}";
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
