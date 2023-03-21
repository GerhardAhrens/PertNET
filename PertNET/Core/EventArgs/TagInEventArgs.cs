//-----------------------------------------------------------------------
// <copyright file="SwitchDialogEventArgs.cs" company="Lifeprojects.de">
//     Class: SwitchDialogEventArgs
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>29.05.2021</date>
//
// <summary>
// Argument Class zum Darstellen des Wechseln zwischen zwei Dialogen
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System;

    using EasyPrototypingNET.Pattern;

    /// <summary>
    /// Argument beim Wechslem zwischen den UserControl-Dialogen
    /// </summary>
    /// <typeparam name="IViewModel"></typeparam>
    public class TagInEventArgs<IViewModel> : EventArgs, IPayload
    {
        /// <summary>
        /// UserControl-Dialog Content
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// UserControl-Dialog Typ
        /// </summary>
        public string Text { get; set; }
    }
}
