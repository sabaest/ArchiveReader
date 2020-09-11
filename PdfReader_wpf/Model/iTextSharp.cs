namespace PdfReader_wpf.Model
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using iTextSharp.text.pdf;
    using iTextSharp.text.pdf.parser;

    public class iTextSharpWrapper
    {
        public SortedList<int, byte[]> Pdf2Mem(string path)
        {
            using (var reader = new PdfReader(path))
            {
                var images = new List<PdfImageObject>();
                var parser = new PdfReaderContentParser(reader);
                for (int pageNumber = 1; pageNumber < reader.NumberOfPages + 1; pageNumber++)
                {
                    parser.ProcessContent(pageNumber, new ImageRenderListener(images));
                }

                var pages = new SortedList<int, byte[]>();
                Parallel.For(0, images.Count, page =>
                {
                    pages[page] = images[page].GetImageAsBytes();
                });

                return pages;
            }
        }
    }

    internal class ImageRenderListener : IRenderListener
    {
        private List<PdfImageObject> _list;

        public ImageRenderListener(List<PdfImageObject> list)
        {
            _list = list;
        }

        public void BeginTextBlock()
        {
            // 今回なにもする必要なし
        }

        public void EndTextBlock()
        {
            // 今回なにもする必要なし
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            var img = renderInfo.GetImage();
            _list.Add(img);
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            // 今回なにもする必要なし
        }

    }
}