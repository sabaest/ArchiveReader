namespace PdfReader_wpf.Model
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.IO;

    static class Common
    {
        public static string SaveFile(string file = "", string path = "")
        {
            // ダイアログのインスタンスを生成
            var dialog = new SaveFileDialog
            {
                Filter = "PDFファイル (*.pdf)|*.pdf;",
                FileName = file,
                InitialDirectory = path,
            };

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {   // キャンセル
                return string.Empty;
            }
        }

        public static string LoadFile(string path = "")
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "PDFファイル (*.pdf)|*.pdf|ZIPファイル (*.zip)|*.zip;",
                InitialDirectory = path,
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {   // キャンセル
                return string.Empty;
            }
        }

        public static bool CheckExt(string path, List<string> ignore = null)
        {
            if (!new Func<string, bool>(s =>
                {
                    switch (Path.GetExtension(s.ToLower()))
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                        case ".pdf":
                        case ".zip":
                            if (ignore != null && ignore.Contains(Path.GetExtension(s.ToLower())))
                                return false;
                            return true;
                        default:
                            return false;
                    }
                })(path))
            {
                Console.WriteLine("読み込めないファイル形式");
                return false;
            }
            return true;
        }
    }
}
