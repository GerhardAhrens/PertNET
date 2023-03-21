//-----------------------------------------------------------------------
// <copyright file="FileLock.cs" company="Lifeprojects.de">
//     Class: FileLock
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>10.09.2021</date>
//
// <summary>
// Class for
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core.IO
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using EasyPrototypingNET.Pattern;

    public class FileLock : DisposableBase
    {
        private const int ERROR_SHARING_VIOLATION = 32;
        private const int ERROR_LOCK_VIOLATION = 33;

        public FileLock()
        {
        }

        public bool CanReadFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath) == true)
                {
                    using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                if (this.IsFileLocked(ex))
                {
                    return false;
                }
            }

            return true;
        }

        protected override void DisposeManagedResources()
        {
            /* Behandeln von Managed Resources bem verlassen der Klasse */
        }

        protected override void DisposeUnmanagedResources()
        {
            /* Behandeln von UnManaged Resources bem verlassen der Klasse */
        }

        private bool IsFileLocked(Exception exception)
        {
            int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
            return errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION;
        }
    }
}
