namespace PdfReader_wpf.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using ClickPoint = System.Windows.Point;
    using static PdfReader_wpf.Model.Common;

    partial class ImageViewer
    {
        public void Closed(object sender, EventArgs e)
        {
            reader = null;

            SaveSettings();
        }

        private bool IsDrugging;
        private bool IsZooming;
        private ClickPoint startPos;
        private ClickPoint startScroll;

        public void MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Source.Value == null)
                return;

            if (!IsZooming)
            {
                ShiftPage(sender, 1);
            }
            IsZooming = false;
        }

        public void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Source.Value == null)
                return;

            var view = (MainWindow)sender;

            if (view.side.Visibility == Visibility.Visible)
            {   // サイドが開いている場合
                return;
            }

            startPos = e.GetPosition(null);
            startScroll = new ClickPoint(view.sviewer.HorizontalOffset, view.sviewer.VerticalOffset);
            IsDrugging = true;
        }

        public void PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Source.Value == null)
                return;

            IsDrugging = false;
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            if (Source.Value == null)
                return;

            if (IsDrugging)
            {
                ClickPoint mousePos = e.GetPosition(null);
                Vector diff = -(startPos - mousePos);

                if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance &&
                    Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
                    return;

                if (e.LeftButton == MouseButtonState.Pressed)
                {   // 移動
                    var view = (MainWindow)sender;

                    if (view.sviewer.ExtentHeight > view.sviewer.ViewportHeight)
                        view.sviewer.ScrollToVerticalOffset(startScroll.Y - diff.Y);

                    if (view.sviewer.ExtentWidth > view.sviewer.ViewportWidth)
                        view.sviewer.ScrollToHorizontalOffset(startScroll.X - diff.X);
                }

                if (e.RightButton == MouseButtonState.Pressed)
                {   // ズーム
                    IsZooming = true;
                    Scale.Value += diff.X < 0 ? -0.01 : diff.X > 0 ? 0.01 : 0;
                    startPos = e.GetPosition(null);
                }
            }
        }

        public void MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Source.Value == null)
            {
                var f = LoadFile();
                if (f != string.Empty) ProcessDivergence(f);
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                ShiftPage(sender, -1);
            }
        }

    }
}
