namespace PertNET.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using PertNET.Core;

    /// <summary>
    /// Interaktionslogik für ApplicationSettingsView.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class ApplicationSettingsView : Window
    {
        public ApplicationSettingsView()
        {
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnWindowLoaded);
            WeakEventManager<TabControl, RoutedEventArgs>.AddHandler(this.tabControlSettings, "Loaded", this.OnTabControlLoaded);
            WeakEventManager<TabControl, SelectionChangedEventArgs>.AddHandler(this.tabControlSettings, "SelectionChanged", this.OnSelectionChanged);

        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            //var userControls = FindVisualChildren<UserControl>(this);
            /*
            foreach (var userControl in userControls)
            {
                ImageCapturer.SaveToPng(userControl, "C:\\Images\\" + userControl.Name + ".png");
            }
            */
        }

        private void OnTabControlLoaded(object sender, RoutedEventArgs e)
        {
            var tabControl = (TabControl)sender;
            if (tabControl != null)
            {
                foreach (TabItem tabItem in tabControl.Items)
                {
                    tabItem.IsVisibleChanged += (mSender, ev) => this.OnItemIsVisibleChanged(mSender, ev, tabControl);
                }
            }
        }

        private void OnItemIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e, TabControl tabControl)
        {
            if ((bool)e.NewValue == false)
            {
                if (tabControl == null)
                {
                    return;
                }

                tabControl.SelectedIndex = 0;
                tabControl.SelectedItem = tabControl.Items[0];

                var items = tabControl.Items.Cast<TabItem>().Where( w => w.IsVisible == true);
                foreach (TabItem tabItem in items)
                {
                    tabControl.SelectedItem = tabItem;
                    tabControl.SelectedIndex = 0;
                    tabItem.IsSelected = true;
                    return;
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems?.Count == 0)
            {
                return;
            }

            TabItem tabItem = e.AddedItems[0] as TabItem;
            if (this.IsLoaded == true && tabItem != null && tabItem.IsSelected == true)
            {
                SettingsTabItem tabName = (SettingsTabItem)tabItem.Tag;
                if (tabName == SettingsTabItem.Allgemein)
                {
                }
                else if (tabName == SettingsTabItem.UserProjectInfo)
                {
                    TabWorkUserInfo content = new TabWorkUserInfo();
                    tabItem.Content = content;
                }
                else if (tabName == SettingsTabItem.Statistik)
                {
                }
                else if (tabName == SettingsTabItem.ApplicationInfo)
                {
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
