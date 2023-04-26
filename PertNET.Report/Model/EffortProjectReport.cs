//-----------------------------------------------------------------------
// <copyright file="EffortProjectReport.cs" company="Lifeprojects.de">
//     Class: EffortProjectReport
//     Copyright © Lifeprojects.de 2023
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>26.04.2023 09:11:24</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Report.Model
{
    using System;

    public class EffortProjectReport
    {

        public Guid Id { get; set; }

        public int ChapterA { get; set; }

        public int ChapterB { get; set; }

        public int ChapterC { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool ShowDescription { get; set; }

        public double Min { get; set; }

        public double Mid { get; set; }

        public double Max { get; set; }

        public double Factor { get; set; }

        public string BackgroundColor { get; set; }

        public string Tag { get; set; }

        public bool IsSelected { get; set; }
    }
}
