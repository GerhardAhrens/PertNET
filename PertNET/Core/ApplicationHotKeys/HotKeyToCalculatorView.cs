//-----------------------------------------------------------------------
// <copyright file="CustomHotKey.cs" company="Lifeprojects.de">
//     Class: CustomHotKey
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <Framework>4.8</Framework>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>25.12.2022 13:20:07</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core.ApplicationHotKeys
{
    using System;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Input;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.WPF;

    using PertNET.View;

    [Serializable]
    public class HotKeyToCalculatorView : HotKey
    {
        private Window mainWindow = null;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKeyToCalculatorView"/> class.
        /// </summary>
        public HotKeyToCalculatorView(string name, Key key, ModifierKeys modifiers, bool enabled) : base(key, modifiers, enabled)
        {
            this.Name = name;
        }

        protected HotKeyToCalculatorView(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Name = info.GetString("Name");
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected override void OnHotKeyPress()
        {
            mainWindow = Application.Current.Windows.LastActiveWindow();

            using (DialogService ws = new DialogService())
            {
                ws.Title = "Rechner";
                ws.ResizeMode = ResizeMode.NoResize;
                ws.WindowStyle = WindowStyle.ToolWindow;
                ws.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                bool? dlgResult = ws.ShowDialog<CalculatorView>(this.mainWindow, MonitorSelect.Primary);
            }

            base.OnHotKeyPress();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Name", this.Name);
        }
    }
}
