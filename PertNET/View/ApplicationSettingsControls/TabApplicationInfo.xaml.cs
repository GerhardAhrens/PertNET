namespace PertNET.View
{
    using PertNET.ViewModel;

    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaktionslogik für TabApplicationInfo.xaml
    /// </summary>
    public partial class TabApplicationInfo : UserControl
    {
        private TabApplicationInfoVM viewModel = null;

        public TabApplicationInfo()
        {
            this.InitializeComponent();

            if (this.InDesignMode() == false)
            {
                if (viewModel == null)
                {
                    viewModel = new TabApplicationInfoVM();
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
