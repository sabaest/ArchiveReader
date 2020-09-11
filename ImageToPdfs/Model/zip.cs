namespace ImageToPdfs.Model
{
    using Ionic.Zip;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using static ImageToPdfs.Model.Common;

    static class zip
    {
        public static Dictionary<string, byte[]> zipOpen(string path)
        {
            var dic = new Dictionary<string, byte[]>();

            using (var ZipFiles = new ZipFile(path, Encoding.GetEncoding("shift_jis")))
            {
                foreach (var entry in ZipFiles.Entries)
                {
                    if (!CheckExt(entry.FileName)) continue;

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
