
//-----------------------------------------------------------------------
// <copyright file="SettingsContent.cs" company="Lifeprojects.de">
//     Class: SettingsContent
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>27.06.2022</date>
//
// <summary>
// Klasse zum speichern von Wertepaare für Einstellungen (in LocalSettings)
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("Key={this.Key};Type={this.Type};Value={this.Value.ToString()}")]
    public class SettingsContent
    {
        public string Key { get; set; }

        public string Type { get; set; }

        public object Value { get; set; }
    }
}