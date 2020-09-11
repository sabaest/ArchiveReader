namespace ImageToPdfs.Model
{
    using Microsoft.Win32;
    using System;
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

        public static bool CheckExt(string path)
        {
            if (!new Func<string, bool>(s =>
                {
                    switch (Path.GetExtension(s))
                    {
                        case ".jpg": break;
                        case ".jpeg": break;
                        case ".png": break;
                        case ".bmp": break;
                        case ".pdf": break;
                        case ".zip": break;
                        default:
                            return false;
                    }
                    return true;
                })(path))
            {
                Console.WriteLine("読み込めないファイル形式");
                return false;
            }
            return true;
        }

    }
}
