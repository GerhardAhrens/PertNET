//-----------------------------------------------------------------------
// <copyright file="SelectFolderForDialog.cs" company="Lifeprojects.de">
//     Class: SelectFolderForDialog
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>07.10.2021</date>
//
// <summary>
// Class to Path Selection for Create File
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System;
    using System.IO;
    using System.Runtime.Versioning;

    using EasyPrototypingNET.IO;
    using EasyPrototypingNET.Pattern;

    [SupportedOSPlatform("windows")]
    public class SelectFolderForDialog : DisposableBase
    {
        public SelectFolderForDialog(SelectFolderSettings settings, string fileType)
        {
            this.FileType = fileType;
            this.FolderSelect(settings);
        }

        public string SelectFolder { get; private set; }

        public string FileType { get; private set; }

        protected override void DisposeManagedResources()
        {
            /* Behandeln von Managed Resources bem verlassen der Klasse */
        }

        protected override void DisposeUnmanagedResources()
        {
            /* Behandeln von UnManaged Resources bem verlassen der Klasse */
        }

        private void FolderSelect(SelectFolderSettings settings)
        {
            SelectFolderResult result = null;

            if (settings.FileFilter == null)
            {
                FileFilter fileFilter = new FileFilter();
                if (this.FileType == "xlsx")
                {
                    fileFilter.AddFilter("Excel (ab 2007)", "xlsx", true);
                }
                else if (this.FileType == "docx")
                {
                    fileFilter.AddFilter("Word (ab 2007)", "docx", true);
                }
                else if (this.FileType == "png")
                {
                    fileFilter.AddFilter("Picture", "png", true);
                }

                settings.FileFilter = fileFilter;
            }

            try
            {
                result = FileTargetFolderView.Execute(settings);
                if (result != null)
                {
                    this.SelectFolder = result.SelectFolder;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("ExportFolder", Path.GetDirectoryName(this.SelectFolder));
                ex.Data.Add("ExportFile", Path.GetFileName(this.SelectFolder));
                throw;
            }
        }
    }
}
