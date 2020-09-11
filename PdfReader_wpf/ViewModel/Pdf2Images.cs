namespace PdfReader_wpf.ViewModel
{
    using PdfReader_wpf.Model;
    using System.Collections.Generic;

    class Pdf2Images
    {
        private SortedList<int, byte[]> files;

        public Pdf2Images(string file)
        {
            var itsc = new Model.iTextSharpWrapper();


            var g = new GhostScriptSharpLine();
            g.gsdotnet(file);

            //files = itsc.Pdf2Mem(file);

            //files = test(file);
        }

        public SortedList<int, byte[]> test(string file)
        {
            var ps = new Model.PdfSharp();

            var s = ps.ExtractJpegs(file);

            var list = new SortedList<int, byte[]>();

            var i = 0;
            foreach (var t in s)
            {
                list.Add(i, t);
                i++;
            }

            return list;
        }

        public byte[] GetImageByte(int p)
        {
            return files[p];
        }

        public int GetCount()
        {
            return files.Count - 1;
        }

    }
}
