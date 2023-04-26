namespace PertNET
{
    using System;
    using System.ComponentModel;
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Threading;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.Core.SystemMetrics;

    using PertNET.Core;
    using PertNET.Core.ApplicationHotKeys;
    using PertNET.Core.Collection;
    using PertNET.ViewModel;

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : Window
    {
        private const string DateFormat = "dd.MM.yy HH:mm";
        private DispatcherTimer statusBarDate = null;
        private readonly MainWindowVM rootVM = null;

        public MainWindow()
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, CancelEventArgs>.AddHandler(this, "Closing", this.OnClosing);
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "SizeChanged", this.OnSizeChanged);

            try
            {
                this.InitTimer();

                using (LocalSettingsManager sm = new LocalSettingsManager())
                {
                    if (sm.ApplicationPosition == true)
                    {
                        using (UserPreferences userPrefs = new UserPreferences(this, ApplicationProperties.AssemplyPath))
                        {
                            userPrefs.Load();
                        }
                    }
                    else
                    {
                        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }
                }

                this.InfoDeviceType = SystemMetricsInfo.DetectingDeviceType();

                if (this.rootVM == null)
                {
                    this.rootVM = new MainWindowVM();
                }

                this.DataContext = this.rootVM;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (KeyBinding item in this.InputBindings)
            {
                Key key = ((KeyGesture)((KeyBinding)item).Gesture).Key;
                ModifierKeys modifierKeys = ((KeyGesture)((KeyBinding)item).Gesture).Modifiers;
                KeyListCollection.Add(nameof(MainWindow),$"{modifierKeys}-{key}");
            }

            var aa = KeyListCollection.ToList();

            HotKeyHost hotKeyHost = new HotKeyHost((HwndSource)HwndSource.FromVisual(App.Current.MainWindow));
            hotKeyHost.AddHotKey(new HotKeyToCalculatorView("ShowCalcPopup", Key.C, ModifierKeys.Alt, true));
        }

        private InfoDeviceType InfoDeviceType { get; set; }

        private void MaximizeRestoreClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            if (window.WindowState == System.Windows.WindowState.Normal)
            {
                window.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                window.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = System.Windows.WindowState.Minimized;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            using (UserPreferences userPrefs = new UserPreferences(this, ApplicationProperties.AssemplyPath))
            {
                userPrefs.Save();
            }

            using (LocalSettingsManager sm = new LocalSettingsManager())
            {
                sm.LastAccess = DateTime.Now;
                sm.LastUser = UserInfo.TS().CurrentDomainUser;
            }
        }

        private void OnSizeChanged(object sender, RoutedEventArgs e)
        {
            int countMonitors = SystemMetricsInfo.CountMonitors;

            string windowSize = $"{this.ActualWidth.ToInt()}x{this.ActualHeight.ToInt()}x{countMonitors}:{this.InfoDeviceType}";
            this.tbMonitorSize.Text = windowSize;
        }

        private void InitTimer()
        {
            this.statusBarDate = new DispatcherTimer();
            this.statusBarDate.Interval = new TimeSpan(0, 0, 1);
            this.statusBarDate.Start();
            this.statusBarDate.Tick += new EventHandler(
                delegate (object s, EventArgs a) {
                    this.dtStatusBarDate.Text = DateTime.Now.ToString(DateFormat);
                });
        }

        private void OnContextMenuClick(object sender, RoutedEventArgs e)
        {
            if (sender != null && sender is Button button)
            {
                button.ContextMenu.IsEnabled = true;
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}
