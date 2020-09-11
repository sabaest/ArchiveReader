namespace PdfReader_wpf.View.Behavior
{
    using PdfReader_wpf.Model;
    using PdfReader_wpf.ViewModel;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    class ListViewBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= SelectionChanged;
        }

        protected void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListView)sender;
            if (list.SelectedItems.Count > 0)
            {
                var dp = (DataPiece)list.SelectedItem;
                var iv = (ImageViewer)list.DataContext;
                iv.Page.Value = int.Parse(dp.page) - 1;
            }

            list.SelectedItems.Clear();
        }
    }
}
