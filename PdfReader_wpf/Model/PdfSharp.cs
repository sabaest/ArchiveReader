namespace PdfReader_wpf.Model
{
    using global::PdfSharp.Pdf;
    using global::PdfSharp.Pdf.Advanced;
    using global::PdfSharp.Pdf.IO;
    using System.Collections.Generic;
    using System.Linq;

    class PdfSharp
    {
        public IEnumerable<byte[]> ExtractJpegs(string pdfPath)
        {
            using (var doc = PdfReader.Open(pdfPath, PdfDocumentOpenMode.ReadOnly))
            {
                var test = doc.Pages
                    .Cast<PdfPage>()
                    .Select(page => page.Elements.GetDictionary("/Resources"))
                    .Where(res => res != null)
                    .Select(res => res.Elements.GetDictionary("/XObject"))
                    .Where(xobj => xobj != null)
                    .SelectMany(xobj => xobj.Elements.Values)
                    .OfType<PdfReference>()
                    .Select(r => r.Value);

                foreach (var t in test)
                {
                    var sss = t;
                }


                return doc.Pages
                    .Cast<PdfPage>()
                    .Select(page => page.Elements.GetDictionary("/Resources"))
                    .Where(res => res != null)
                    .Select(res => res.Elements.GetDictionary("/XObject"))
                    .Where(xobj => xobj != null)
                    .SelectMany(xobj => xobj.Elements.Values)
                    .OfType<PdfReference>()
                    .Select(r => r.Value)
                    .OfType<PdfDictionary>()
                    .Where(xobj => xobj != null && xobj.Elements.GetString("/Subtype") == "/Image")
                    .Where(image => image.Elements.GetName("/Filter") == "/DCTDecode")
                    .Select(image => image.Stream.Value);
            }
        }
    }
}
