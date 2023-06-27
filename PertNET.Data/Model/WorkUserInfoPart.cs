//-----------------------------------------------------------------------
// <copyright file="WorkUserInfoPart.cs" company="Lifeprojects.de">
//     Class: WorkUserInfo
//     Copyright © www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>04.07.2022 13:44:09</date>
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

    public sealed partial class WorkUserInfo : ModelBase<WorkUserInfo>, IModel
    {
        public string FullName
        {
            get
            {
                return $"UserId={this.UserId};Project={this.Project}";
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
