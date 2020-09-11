namespace PdfReader_wpf.Model
{
    using Ionic.Zip;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using static PdfReader_wpf.Model.Common;

    static class zip
    {
        private const string enc_sjis = "shift_jis";

        public static SortedList<string, byte[]> ZipOpen(string path)
        {
            var dic = new SortedList<string, byte[]>();

            using (var ZipFiles = new ZipFile(path, Encoding.GetEncoding(enc_sjis)))
            {
                foreach (var entry in ZipFiles.Entries)
                {
                    if (!CheckExt(entry.FileName, new List<string>() { ".pdf" })) continue;

                    using (var s = entry.OpenReader())
                    {
                        using (var ms = new MemoryStream())
                        {
                            s.CopyTo(ms);
                            dic.Add(entry.FileName, ms.ToArray());
                        }
                    }
                }
            }

            return dic;
        }
    }
}
