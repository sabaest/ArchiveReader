namespace PdfReader_wpf.Model
{
    class DataPiece
    {
        public System.Windows.Media.ImageSource image { get; set; }
        public string name { get; set; }
        public string page { get; set; }

        public Fork fork { get; set; }
        public enum Fork
        {
            Bitmap,
            Zip,
            Pdf,
        }
    }
}
