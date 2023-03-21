﻿//-----------------------------------------------------------------------
// <copyright file="FolderAction.cs" company="Lifeprojects.de">
//     Class: FolderAction
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>06.10.2021</date>
//
// <summary>
// Class for Enum Typ 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System.ComponentModel;

    public enum FolderAction : int
    {
        [Description("Unbekannt")]
        None = 0,
        [Description("SelectFolderDialog")]
        SelectFolder = 1,
        [Description("Working with Used Folder")]
        UsedFolder = 2,
        [Description("OpenFile")]
        OpenFile = 3,
        [Description("BrowseFolder")]
        Folder = 4
    }
}
