namespace ImageToPdfs.ViewModel
{
    using GongSolutions.Wpf.DragDrop;
    using ImageToPdfs.Model;
    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using static ImageToPdfs.Model.Common;
    using static ImageToPdfs.Model.zip;

    class Conv : ViewModelOrigin, IDropTarget
    {
        #region Members

        private CompositeDisposable Disposable { get; } 
            = new CompositeDisposable();

        private string LastInput = string.Empty;

        #endregion

        #region Properties

        public ObservableCollection<DataPiece> DataPieces { get; } 
            = new ObservableCollection<DataPiece>();

        public ReactiveProperty<bool> IsDrag { get; } 

        public AsyncReactiveCommand ButtonStart { get; } 

        public AsyncReactiveCommand ButtonRemoveAll { get; }

        private ReactiveProperty<bool> IsConvertingShare { get; } 

        #endregion

        public Conv()
        {
            IsDrag = new ReactiveProperty<bool>(true).AddTo(Disposable);

            IsConvertingShare = new ReactiveProperty<bool>(true).AddTo(Disposable);

            ButtonStart = new AsyncReactiveCommand().AddTo(Disposable);
            ButtonStart = IsConvertingShare.ToAsyncReactiveCommand();
            ButtonStart.Subscribe(async _ =>
            {
                await Task.Run(() =>
                {
                    using (var Pdf = new iTextSharp())
                    {
                        Pdf.Output = SaveFile(Path.GetFileName(LastInput), Path.GetDirectoryName(LastInput));

                        if (Pdf.Output == string.Empty)
                            return;

                        Pdf.Open();

                        foreach (var dp in DataPieces.OrderBy(x => x.page.Value))
                        {
                            switch (dp.fork)
                            {
                                case DataPiece.Fork.Bitmap:
                                    Pdf.AddToPdf(dp.path);
                                    break;
                                case DataPiece.Fork.Zip:
                                    Pdf.AddToPdf(dp.byteData);
                                    break;
                            }
                        }
                    }
                });

                MessageBox.Show("完了");
            });

            ButtonRemoveAll = new AsyncReactiveCommand().AddTo(Disposable);
            ButtonRemoveAll = IsConvertingShare.ToAsyncReactiveCommand();
            ButtonRemoveAll.Subscribe(async _ =>
            {
                DataPieces.Clear();
            });
        }

        ~Conv()
        {
            Disposable.Dispose();
        }

        #region methods

        private void ProcessDivergence(List<string> files)
        {
            files.ForEach(f =>
            {
                switch (Path.GetExtension(f))
                {
                    case ".jpg":
                        // Image追加
                        AddDataPieces(f);
                        break;
                    case ".zip":
                        // ZIP変換追加
                        LastInput = f;
                        AddDataPieces(zipOpen(f));
                        break;
                    case ".pdf":
                        // PDFimage追加(未実装)
                        LastInput = f;

                        break;
                }
            });
        }

        private BitmapImage Create(object a)
        {
            using (var stream = new MemoryStream(GetThumb(a)))
            {
                var src = new BitmapImage();
                src.BeginInit();
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.CreateOptions = BitmapCreateOptions.None;
                src.StreamSource = stream;
                src.EndInit();
                src.Freeze();

                return src;
            }
        }

        private byte[] GetThumb(object a)
        {
            Image orig = null;
            switch (a)
            {
                case string path:
                    orig = Image.FromFile(path);
                    break;
                case byte[] b:
                    var ic = new ImageConverter();
                    orig = (Image)ic.ConvertFrom(b);
                    break;
            }

            int ThumbSize = 160;
            var s = new Func<double, double, double>((w, h) =>
            {
                if (w > ThumbSize || h > ThumbSize)
                {
                    return ThumbSize / Math.Max(w, h);
                }

                return 1;
            })(orig.Width, orig.Height);

            using (var thumbnail = orig.GetThumbnailImage(
                (int)(orig.Width * s), (int)(orig.Height * s), () => false, IntPtr.Zero))
            {
                var imgconv = new ImageConverter();
                orig.Dispose();
                return (byte[])imgconv.ConvertTo(thumbnail, typeof(byte[]));
            }
        }

        private void AddDataPieces(string path)
        {
            DataPieces.Add(new DataPiece
            {
                image = Create(path),
                name = Path.GetFileName(path),
                fork = DataPiece.Fork.Bitmap,
                path = path,
                page = new ReactiveProperty<string>((DataPieces.Count() + 1).ToString()
                    .PadLeft(DataPieces.Count().ToString().Length, '0'))
            });
        }

        private void AddDataPieces(Dictionary<string, byte[]> dic)
        {
            foreach (var key in dic.Keys)
            {
                DataPieces.Add(new DataPiece
                {
                    image = Create(dic[key]),
                    name = Path.GetFileName(key),
                    fork = DataPiece.Fork.Zip,
                    byteData = dic[key],
                    page = new ReactiveProperty<string>((DataPieces.Count() + 1).ToString()
                        .PadLeft(DataPieces.Count().ToString().Length, '0'))
                });
            }
        }

        #region DragDrop

        public void DragOver(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();

            dropInfo.Effects = files.Any(fname => CheckExt(fname))
                ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>()
                .Where(fname => CheckExt(fname)).ToList();

            if (files.Count == 0) return;

            ProcessDivergence(files);

            // ページ数更新
            DataPieces[0].ReloadPage(DataPieces);
        }

        #endregion

        #endregion

#if DEBUG
        public void TestAddDataPieces(string path)
        {
            AddDataPieces(path);
        }
#endif

    }
}
