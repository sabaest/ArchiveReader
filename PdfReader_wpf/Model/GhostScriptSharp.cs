namespace PdfReader_wpf.Model
{
    using Ghostscript.NET.Rasterizer;

    class GhostScriptSharpLine
    {
        /// <summary>
        /// tte
        /// </summary>
        /// <param name="file"></param>
        public void gsdotnet(string file)
        {
            PdfToPng(file, "test.jpg");
        }

        private static void PdfToPng(string inputFile, string outputFileName)
        {
            var xDpi = 100; //set the x DPI
            var yDpi = 100; //set the y DPI
            var pageNumber = 1; // the pages in a PDF document

            using (var rasterizer = new GhostscriptRasterizer()) //create an instance for GhostscriptRasterizer
            {
                rasterizer.Open(inputFile); //opens the PDF file for rasterizing

                var pdf2PNG = rasterizer.GetPage(xDpi, yDpi, pageNumber);

                //save the png's
                pdf2PNG.Save(outputFileName);
            }
        }
    }
}

