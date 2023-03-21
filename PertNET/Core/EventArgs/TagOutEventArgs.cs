namespace PertNET.Core
{
    using System;

    using EasyPrototypingNET.Pattern;

    /// <summary>
    /// Argument beim Wechslem zwischen den UserControl-Dialogen
    /// </summary>
    /// <typeparam name="IViewModel"></typeparam>
    public class TagOutEventArgs<IViewModel> : EventArgs, IPayload
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
