//-----------------------------------------------------------------------
// <copyright file="Note.cs" company="www.lifeprojects.de">
//     Class: Note
//     Copyright © www.lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>30.06.2022 13:44:09</date>
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

    public sealed partial class Note : ModelBase<Note>, IModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffortProject"/> class.
        /// </summary>
        public  Note()
        {
            this.Id = Guid.NewGuid();
            this.CreatedBy = UserInfo.TS().CurrentUser;
            this.CreatedOn = UserInfo.TS().CurrentTime;
        }

        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public string ObjectName { get; set; }

        public string Content { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}
