
namespace PertNET.View
{
    using System.Collections.Generic;
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using EasyPrototypingNET.Interface;

    using PERT.ViewModel;

    using PertNET.Core;


    /// <summary>
    /// Interaktionslogik für EffortProjectView.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class EffortProjectView : Window
    {
        public EffortProjectView()
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            App.EventAgg.Subscribe<TagInEventArgs<IViewModel>>(this.GetTextTag);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                string tags = ((EffortProjectVM)this.DataContext).TagText;
                this.SetTextTag(tags);
            }
        }

        private void GetTextTag(TagInEventArgs<IViewModel> obj)
        {
            List<string> labels = new List<string>();

            foreach (var block in this.TagViewControl.Document.Blocks)
            {
                if (block is Paragraph)
                {
                    var paragraph = block as Paragraph;
                    foreach (var inline in paragraph.Inlines)
                    {
                        if (inline is InlineUIContainer)
                        {
                            var child = ((InlineUIContainer)inline).Child;
                            var text = ((ContentPresenter)child).Content;
                            labels.Add(text.ToString());
                        }
                    }

                }
            }

            if (labels.Count > 0)
            {
                string tags = string.Join(",", labels);
                if ((EffortProjectVM)this.DataContext != null)
                {
                    ((EffortProjectVM)this.DataContext).TagText = tags;
                }
            }
        }


        private void SetTextTag(string tags)
        {
            if (string.IsNullOrEmpty(tags) == false)
            {
                this.TagViewControl.Document.Blocks.Clear();
                this.TagViewControl.CaretPosition = this.TagViewControl.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);

                foreach (string tag in tags.Split(','))
                {
                    this.TagViewControl.CaretPosition.InsertTextInRun(tag);
                    this.TagViewControl.CaretPosition.InsertTextInRun(",");
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }
}
