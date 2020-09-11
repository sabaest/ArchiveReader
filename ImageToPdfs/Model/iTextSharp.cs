namespace ImageToPdfs.Model
{
    using global::iTextSharp.text;
    using global::iTextSharp.text.pdf;
    using System;
    using System.IO;

    class iTextSharp : IDisposable
    {
        public string Output { get; set; } = string.Empty;

        private Document document;
        private FileStream stream;

        public iTextSharp()
        {
            document = new Document();
        }

        public void Dispose()
        {
            document.Close();
            stream.Close();
        }

        public void Open()
        {
            if (Output is null || Output == string.Empty)
                return;

            stream = new FileStream(Output, FileMode.Create, FileAccess.Write, FileShare.None);
            PdfWriter.GetInstance(document, stream);
            document.Open();
        }

        public void AddToPdf(string path)
        {
            using (var imageStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                document.Add(Image.GetInstance(imageStream));
            }
        }

        public void AddToPdf(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                document.Add(Image.GetInstance(ms));
            }
        }

    }
}
