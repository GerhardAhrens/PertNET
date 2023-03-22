namespace PertNET.View
{
    using PertNET.ViewModel;

    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für TabStatisticsInfo.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class TabStatisticsInfo : UserControl
    {
        private TabStatisticsInfoVM viewModel = null;

        public TabStatisticsInfo()
        {
            this.InitializeComponent();

            if (this.InDesignMode() == false )
            {
                if (viewModel == null)
                {
                    viewModel = new TabStatisticsInfoVM();
                }

                this.DataContext = viewModel;
            }
        }

        private bool InDesignMode()
        {
            return !(Application.Current is App);
        }
    }
}
