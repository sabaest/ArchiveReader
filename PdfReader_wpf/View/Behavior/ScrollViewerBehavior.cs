namespace PdfReader_wpf.View.Behavior
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    class ScrollViewerBehavior : Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseWheel += PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseWheel -= PreviewMouseWheel;
        }

        protected void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            const double move = 20f;
            var wh = e.Delta;
            var sv = (ScrollViewer)sender;
            var v = sv.VerticalOffset;

            v += wh < 0 ? move : wh > 0 ? -move : 0;
            sv.ScrollToVerticalOffset(v);
        }
    }
}
