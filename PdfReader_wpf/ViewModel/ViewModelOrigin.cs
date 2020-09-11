namespace PdfReader_wpf.ViewModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// ViewModelの親クラス
    /// プロパティが変更されたことを通知するため、INotifyPropertyChangedインターフェースを実装する
    /// プロパティ変更時NotifyPropertyChangedを呼び出す
    /// </summary>
    class ViewModelOrigin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
