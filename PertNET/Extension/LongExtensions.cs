//-----------------------------------------------------------------------
// <copyright file="LongExtensions.cs" company="Lifeprojects.de">
//     Class: LongExtensions
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>23.06.2022 12:44:15</date>
//
// <summary>
// Extension Class for 
// </summary>
//-----------------------------------------------------------------------

namespace System
{
    using System;

    public static class LongExtensions
    {

        /// <summary>
        /// Gibt die formatierte Zeit aus Ticks zurück
        /// </summary>
        /// <param name="this"></param>
        /// <returns>String, Formatierte Zeit (hh:mm:ss)</returns>
        public static string ToMillisecondsFormat(this long @this)
        {
            TimeSpan duration = new TimeSpan(@this);
            return $"{duration.Hours:00}:{duration.Minutes:00}:{duration.Seconds:00}";
        }
    }
}
