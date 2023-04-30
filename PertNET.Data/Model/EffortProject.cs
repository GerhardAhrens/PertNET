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

    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using EasyPrototypingNET.BaseClass;
    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Interface;

    [DebuggerDisplay("Chapter={this.Chapter}:Title={this.Title}Min={this.Min};Mid={this.Mid};Max={this.Max};Factor={this.Factor}")]
    public partial class EffortProject : ModelBase<EffortProject>, IModel, INotifyPropertyChanged
    {
        private bool isSelected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="EffortProject"/> class.
        /// </summary>
        public  EffortProject()
        {
            this.CreatedBy = UserInfo.TS().CurrentDomainUser;
            this.CreatedOn = UserInfo.TS().CurrentTime;
        }

        public Guid Id { get; set; }

        public int ChapterA { get; set; }

        public int ChapterB { get; set; }

        public int ChapterC { get; set; }

        [ExportField]
        [SearchFilter]
        public string Title { get; set; }

        [ExportField]
        public string Description { get; set; }

        [ExportField]
        public bool ShowDescription { get; set; }

        [ExportField]
        public double Min { get; set; }

        [ExportField]
        public double Mid { get; set; }

        [ExportField]
        public double Max { get; set; }

        [ExportField]
        public double Factor { get; set; }

        [ExportField]
        public string BackgroundColor { get; set; }

        [SearchFilter]
        [ExportField]
        public string Tag { get; set; }

        public bool ChapterInsert { get; set; } = false;

        public bool ChapterDelete { get; set; } = false;

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }
    }
}
