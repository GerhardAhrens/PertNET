namespace PertNET.View
{
    using PertNET.ViewModel;

    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für TabMainInfo.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class TabMainInfo : UserControl
    {
        private TabMainInfoVM viewModel = null;

        public TabMainInfo()
        {
            this.InitializeComponent();

            if (this.InDesignMode() == false )
            {
                if (viewModel == null)
                {
                    viewModel = new TabMainInfoVM();
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
