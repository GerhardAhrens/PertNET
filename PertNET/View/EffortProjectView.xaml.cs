
namespace PertNET.View
{
    using EasyPrototypingNET.Interface;

    using PERT.ViewModel;

    using PertNET.Core;
    using PertNET.Model;
    using PertNET.ViewModel;

    using System;
    using System.Collections.Generic;
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

    /// <summary>
    /// Interaktionslogik für EffortProjectView.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class EffortProjectView : Window
    {
        private EffortProjectVM rootVM = null;
        private bool setSubItem = false;

        public EffortProjectView()
        {
            this.InitializeComponent();

            if (rootVM == null)
            {
                this.rootVM = new EffortProjectVM(Guid.Empty,false);
            }

            App.EventAgg.Subscribe<TagInEventArgs<IViewModel>>(this.GetTextTag);
            App.EventAgg.Subscribe<TagOutEventArgs<IViewModel>>(this.SetTextTag);
        }

        public EffortProjectView(EffortProject currentItem)
        {
            this.InitializeComponent();
            this.CurrentId = currentItem.Id;
            this.setSubItem = false;

            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnWindowLoaded);

            App.EventAgg.Subscribe<TagInEventArgs<IViewModel>>(this.GetTextTag);
            App.EventAgg.Subscribe<TagOutEventArgs<IViewModel>>(this.SetTextTag);
        }

        public EffortProjectView(EffortProject currentItem, bool subItem = false)
        {
            this.InitializeComponent();
            this.CurrentId = currentItem.Id;
            this.setSubItem = subItem;

            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnWindowLoaded);

            App.EventAgg.Subscribe<TagInEventArgs<IViewModel>>(this.GetTextTag);
            App.EventAgg.Subscribe<TagOutEventArgs<IViewModel>>(this.SetTextTag);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (rootVM == null)
            {
                this.rootVM = new EffortProjectVM(this.CurrentId, this.setSubItem,false);
            }

            this.DataContext = this.rootVM;
        }

        private Guid CurrentId { get; set; }

        private void GetTextTag(TagInEventArgs<IViewModel> obj)
        {
            List<string> labels = new List<string>();

            foreach (var block in this.Tokenizer.Document.Blocks)
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
                string tags = string.Join(";", labels);
                if ((EffortProjectVM)this.DataContext != null)
                {
                    ((EffortProjectVM)this.DataContext).TagText = tags;
                }
            }
        }


        private void SetTextTag(TagOutEventArgs<IViewModel> obj)
        {
            if (obj != null)
            {
                this.Tokenizer.Document.Blocks.Clear();
                this.Tokenizer.CaretPosition = this.Tokenizer.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);

                string[] tags = obj.Text.Split(';');
                foreach (string tag in tags)
                {
                    this.Tokenizer.CaretPosition.InsertTextInRun(tag);
                    this.Tokenizer.CaretPosition.InsertTextInRun(";");
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
    }
}
