namespace PdfReader_wpf.ViewModel
{
    using PdfReader_wpf.Model;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using static PdfReader_wpf.Model.zip;

    partial class ImageViewer
    {
        public ObservableCollection<DataPiece> DataPieces { get; }
            = new ObservableCollection<DataPiece>();

        private void SetImageList(string files)
        {
            DataPieces.Clear();

            switch (Path.GetExtension(files).ToLower())
            {
                case ".jpg":
                    // Image追加
                    AddDataPieces(files);
                    break;
                case ".zip":
                    // ZIP変換追加
                    AddDataPieces(ZipOpen(files));
                    break;
                case ".pdf":
                    // PDFimage追加
                    var p = new iTextSharpWrapper();
                    AddDataPieces(p.Pdf2Mem(files));
                    break;
            }
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
                page = (DataPieces.Count() + 1).ToString()
                    .PadLeft(DataPieces.Count().ToString().Length, '0'),
            });
        }

        private void AddDataPieces(SortedList<string, byte[]> dic)
        {
            foreach (var key in dic.Keys)
            {
                DataPieces.Add(new DataPiece
                {
                    image = Create(dic[key]),
                    name = Path.GetFileName(key),
                    fork = DataPiece.Fork.Zip,
                    page = (DataPieces.Count() + 1).ToString()
                        .PadLeft(DataPieces.Count().ToString().Length, '0'),
                });
            }
        }

        private void AddDataPieces(SortedList<int, byte[]> dic)
        {
            foreach (var key in dic.Keys)
            {
                DataPieces.Add(new DataPiece
                {
                    image = Create(dic[key]),
                    name = string.Empty,
                    fork = DataPiece.Fork.Pdf,
                    page = (DataPieces.Count() + 1).ToString()
                        .PadLeft(DataPieces.Count().ToString().Length, '0'),
                });
            }
        }

    }
}
