//-----------------------------------------------------------------------
// <copyright file="SelectFolderSettings.cs" company="lifeprojects.de">
//     Class: SelectFolderSettings
//     Copyright © lifeprojects.de GmbH 2021
// </copyright>
//
// <author>Gerhard Ahrens</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>07.10.2021</date>
//
// <summary>
//  Class with SelectFolderSettings Definition
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using EasyPrototyping.Core;

    using EasyPrototypingNET.IO;

    public class SelectFolderSettings
    {
        public Window Owner { get; set; }

        public string HeaderText { get; set; }

        public string InstructionText { get; set; }

        public List<string> Folders { get; set; }

        public FileFilter FileFilter { get; set; }

        public string InitialFile { get; set; }

        public string InitialFolder { get; set; }

        public FolderAction FolderAction { get; set; }
    }
}
