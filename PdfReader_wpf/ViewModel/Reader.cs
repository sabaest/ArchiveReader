namespace PdfReader_wpf.ViewModel
{
    using System;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    class Reader
    {
        private object _source = null;

        public int MaxPage { get; private set; } = 0;
        public int MaxSize { get; private set; } = 2000;

        public float Dpi { get; set; } = 96f;

        public Reader(object _obj)
        {
            _source = _obj;
        }

        ~Reader()
        {
            _source = null;
        }

        public BitmapSource GetImage(int _page)
        {
            byte[] img = null;
            switch (_source)
            {
                case Pdf2Images p2i:
                    img = p2i.GetImageByte(_page);
                    MaxPage = p2i.GetCount();

                    break;
                case Zip2Images z2i:
                    img = z2i.GetImageByte(_page);
                    MaxPage = z2i.GetCount();

                    break;
            }

            using (var stream = new MemoryStream(img))
            {
                var b = BitmapFrame.Create(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad
                    );

                var stride = b.PixelWidth * 4;
                var pixelData = new byte[b.PixelHeight * stride];
                b.CopyPixels(pixelData, stride, 0);

                var s = new Func<double, double, double>((w, h) =>
                {
                    if (w > MaxSize || h > MaxSize)
                    {
                        return MaxSize / Math.Max(w, h);
                    }

                    return 1;
                })(b.PixelWidth, b.PixelHeight);

                var bitmap = BitmapSource.Create(b.PixelWidth, b.PixelHeight,
                                Dpi, Dpi, b.Format, b.Palette, pixelData, stride);

                return TranceSelect(bitmap, new ScaleTransform(s, s));
            }
        }

        private BitmapSource TranceSelect(BitmapSource _b, Transform _t)
        {
            var tBit = new TransformedBitmap();

            return new Func<Transform, TransformedBitmap> (x =>
                {
                    tBit.BeginInit();
                    tBit.Source = _b;
                    tBit.Transform = x;
                    tBit.EndInit();
                    tBit.Freeze();
                    return tBit;
                })(_t);
        }
    }
}
