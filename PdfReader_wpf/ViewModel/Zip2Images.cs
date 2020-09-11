namespace PdfReader_wpf.ViewModel
{
    using System.Collections.Generic;
    using static PdfReader_wpf.Model.zip;

    class Zip2Images
    {
        private SortedList<int, byte[]> files;

        public Zip2Images(string path)
        {
            files = ZipCtrl(path);
        }

        public byte[] GetImageByte(int p)
        {
            return files[p];
        }

        public int GetCount()
        {
            return files.Count - 1;
        }

        private static SortedList<int, byte[]> ZipCtrl(string path)
        {
            var z = ZipOpen(path);

            var dict = new SortedList<int, byte[]>();
            foreach (var key in z.Keys)
                dict.Add(z.IndexOfKey(key), z[key]);

            return dict;
        }

    }
}
