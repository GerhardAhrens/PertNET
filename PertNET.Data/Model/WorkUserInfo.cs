//-----------------------------------------------------------------------
// <copyright file="WorkUserInfo.cs" company="Lifeprojects.de">
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

    using System;

    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;

    using PertNET.Data.Core;

    public sealed partial class WorkUserInfo : ModelBase<WorkUserInfo>, IModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffortProject"/> class.
        /// </summary>
        public WorkUserInfo()
        {
            this.Id = Guid.NewGuid();
            this.CreatedBy = UserInfo.TS().CurrentUser;
            this.CreatedOn = UserInfo.TS().CurrentTime;
        }

        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Project { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public DateTime StartDate { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
