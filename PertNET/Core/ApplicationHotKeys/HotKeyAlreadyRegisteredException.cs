//-----------------------------------------------------------------------
// <copyright file="HotKeyAlreadyRegisteredException.cs" company="Lifeprojects.de">
//     Class: HotKeyAlreadyRegisteredException
//     Copyright � Lifeprojects.de 2022
// </copyright>
//
// <Framework>4.8</Framework>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>25.12.2022 12:52:19</date>
//
// <summary>
// Klasse f�r 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core.ApplicationHotKeys
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    /// <summary>
    /// Initializes a new instance of the <see cref="HotKeyAlreadyRegisteredException"/> class.
    /// </summary>
    public class HotKeyAlreadyRegisteredException : Exception
    {
        public HotKeyAlreadyRegisteredException(string message, HotKey hotKey) : base(message) { this.HotKey = hotKey; }

        public HotKeyAlreadyRegisteredException(string message, HotKey hotKey, Exception inner) : base(message, inner) { this.HotKey = hotKey; }

        protected HotKeyAlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public HotKey HotKey { get; private set; }
    }
}
