namespace PertNET.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using EasyPrototypingNET.Core;
    using EasyPrototypingNET.WPF;

    /// <summary>
    /// Interaktionslogik für CalculatorView.xaml
    /// </summary>
    public partial class CalculatorView : Window
    {
        private List<CalculatorResult> calcResults = null;

        public CalculatorView()
        {
            this.InitializeComponent();
            this.calcResults = new List<CalculatorResult>();

            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, KeyEventArgs>.AddHandler(this, "KeyDown", this.OnKeyDown);
            WeakEventManager<TextBoxEx, KeyEventArgs>.AddHandler(this.txtCalcContent, "KeyDown", this.OnKeyDown);
        }

        public string DialogTitle { get; set; }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.DialogTitle) == true)
            {
                this.tbDialogDescription.Text = "Rechner";
            }
            else
            {
                this.tbDialogDescription.Text = this.DialogTitle;
            }
        }

        private void BtnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Tag = null;
            this.Close();
        }

        private void BtnResultMemory_Click(object sender, RoutedEventArgs e)
        {
            Window currentWindow = null;
            if (Application.Current.Windows.Cast<Window>().Where(w => w.Owner != null).Count() == 1)
            {
                currentWindow = Application.Current.Windows.FirstWindow();
            }
            else
            {
                currentWindow = Application.Current.Windows.Cast<Window>().Where(w => w.Owner != null).FirstOrDefault();
            }

            if (currentWindow != null)
            {
                IInputElement focusedControl = FocusManager.GetFocusedElement(currentWindow);
                if (focusedControl is TextBoxEx | focusedControl is TextBox)
                {
                    string currentValue = ((TextBoxEx)focusedControl).Text;
                    if (string.IsNullOrEmpty(currentValue) == true || currentValue.Trim() == "0")
                    {
                        ((TextBoxEx)focusedControl).Text = this.txtCalcResult.Text;
                    }
                    else
                    {
                        ((TextBoxEx)focusedControl).Text = $"{currentValue}{this.txtCalcResult.Text}";
                    }
                }
            }

            this.Tag = null;
            this.Close();
        }

        private void BtnResultMemoryClear_Click(object sender, RoutedEventArgs e)
        {
            this.calcResults.Clear();
            this.cbCalcResult.ItemsSource = null;
            this.cbCalcResult.ItemsSource = this.calcResults;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExpressionEvaluator engine = new ExpressionEvaluator();
                try
                {
                    decimal result = engine.Evaluate(this.txtCalcContent.Text.Replace(",", "."));
                    this.txtCalcResult.Text = result.ToString("#.##");

                    if (this.calcResults.Any(c => c.Content == this.txtCalcContent.Text && c.Total == Convert.ToDecimal(this.txtCalcResult.Text)) == false)
                    {
                        int maxId = 1;
                        if (this.calcResults.Any() == true)
                        {
                            maxId = this.calcResults.Max(id => id.Id) + 1;
                        }

                        CalculatorResult calcResult = new CalculatorResult(maxId, this.txtCalcContent.Text, Convert.ToDecimal(this.txtCalcResult.Text));

                        this.calcResults.Add(calcResult);
                        this.cbCalcResult.ItemsSource = null;
                        this.cbCalcResult.ItemsSource = this.calcResults.OrderByDescending(id => id.Id);
                    }
                }
                catch (Exception ex)
                {
                    this.txtCalcResult.Text = ex.Message;
                }
            }
            else if (e.Key == Key.Escape)
            {
                this.Close();
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}
