namespace PertNET.View
{
    using PERT.ViewModel;

    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für TabWorkUserInfo.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class TabWorkUserInfo : UserControl
    {
        private TabWorkUserInfoVM viewModel = null;

        public TabWorkUserInfo()
        {
            this.InitializeComponent();

            if (this.InDesignMode() == false)
            {
                if (viewModel == null)
                {
                    viewModel = new TabWorkUserInfoVM();
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
