namespace PdfReader_wpf.ViewModel
{
    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;
    using System;
    using System.IO;
    using System.Reactive.Disposables;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using static PdfReader_wpf.Model.Common;

    partial class ImageViewer : ViewModelOrigin
    {
        #region properties

        public ReactiveProperty<string> Filename { get; }
        public ReactiveProperty<ImageSource> Source { get; }
        public ReactiveProperty<double> Scale { get; set; }
        public ReactiveProperty<int> Page { get; }

        public WindowState WindowState { get; set; } = WindowState.Normal;

        #endregion

        #region members

        private Reader reader;

        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        #endregion

        public ImageViewer()
        {
            Filename = new ReactiveProperty<string>(string.Empty).AddTo(Disposable);
            Source = new ReactiveProperty<ImageSource>().AddTo(Disposable);
            Scale = new ReactiveProperty<double>(0).AddTo(Disposable);

            Page = new ReactiveProperty<int>(0).AddTo(Disposable);
            Page.Subscribe(_ => 
            {
                if (reader != null)
                {
                    Page.Value = Page.Value > reader.MaxPage 
                        ? 0 : Page.Value < 0 
                            ? reader.MaxPage : Page.Value;

                    PictureShow();
                }
            });

            LoadSettings();
        }

        public ImageViewer(string[] args) : this()
        {
            if (args != null)
            {
                if (!CheckExt(args[0])) return;

                ProcessDivergence(args[0]);
            }
        }

        ~ImageViewer()
        {
            Disposable.Dispose();
        }

        #region methods

        private void ProcessDivergence(string file)
        {
            if (file == string.Empty)
                return;

            Mouse.OverrideCursor = Cursors.Wait;
            StartProgress();

            switch (Path.GetExtension(file).ToLower())
            {
                case ".zip":
                    LoadImage(new Zip2Images(file));
                    goto Complete;
                case ".pdf":
                    LoadImage(new Pdf2Images(file));
                    goto Complete;
                Complete:
                    Filename.Value = Path.GetFileName(file);
                    SetImageList(file);
                    break;
                default:
                    break;
            }

            FinishProgress();
            Mouse.OverrideCursor = null;
        }

        public void LoadImage(object o)
        {
            if (reader != null)
            {
                reader = null;
            }
            reader = new Reader(o);

            if (reader.MaxPage >= 0)
            {
                PictureShow();
            }
        }

        public void PictureShow()
        {
            var s = Scale.Value;
            Scale.Value = 1;

            Source.Value = reader.GetImage(Page.Value);
            Scale.Value = s;
        }

        private void SaveSettings()
        {
            var settings = Properties.Settings.Default;

            settings.WinState = WindowState == WindowState.Maximized;
            WindowState = WindowState.Normal;

            settings["LastScale"] = Scale.Value;

            settings.Save();
        }

        private void LoadSettings()
        {
            var settings = Properties.Settings.Default;

            WindowState = settings.WinState ? WindowState.Maximized : WindowState.Normal;

            Scale.Value = (double)settings["LastScale"];
        }

        private void ShiftPage(object sender, int p = 0)
        {
            Page.Value += p;
            ((MainWindow)sender).sviewer.ScrollToTop();
        }

        #endregion
    }
}
