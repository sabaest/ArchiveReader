namespace ImageToPdfs
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            var testpath = @"L:\comic\dou\set\014\[みら国]イケない杏はえっちがシたい\";

            var test = (ViewModel.Conv)this.DataContext;
            test.TestAddDataPieces(testpath + @"001.jpg");
            test.TestAddDataPieces(testpath + @"002.jpg");
            test.TestAddDataPieces(testpath + @"003.jpg");
#endif

        }
    }
}
