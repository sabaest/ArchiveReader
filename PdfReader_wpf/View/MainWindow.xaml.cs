﻿namespace PdfReader_wpf
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.ImageViewer(App.CommandLineArgs);
        }
    }
}
