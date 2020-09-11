namespace ImageToPdfs.Model
{
    using ImageToPdfs.ViewModel;
    using Reactive.Bindings;
    using Reactive.Bindings.Extensions;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Windows;

    class DataPiece : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CompositeDisposable Disposable { get; }
            = new CompositeDisposable();

        public System.Windows.Media.ImageSource image { get; set; }
        public string name { get; set; }

        public Fork fork { get; set; }
        public enum Fork
        {
            Bitmap,
            Zip,
        }

        public string path { get; set; }
        public byte[] byteData { get; set; }

        public ReactiveProperty<string> page { get; set; }

        public ReactiveProperty<Visibility> visible { get; } 
        public ReactiveProperty<Visibility> InsertRight { get; set; } 
        public ReactiveProperty<Visibility> InsertLeft { get; set; } 

        public System.Windows.Input.ICommand RemoveCommand { get; private set; }

        public DataPiece()
        {
            visible = new ReactiveProperty<Visibility>(Visibility.Hidden).AddTo(Disposable);
            page = new ReactiveProperty<string>().AddTo(Disposable);
            InsertRight = new ReactiveProperty<Visibility>(Visibility.Hidden).AddTo(Disposable);
            InsertLeft = new ReactiveProperty<Visibility>(Visibility.Hidden).AddTo(Disposable);

            RemoveCommand = new DelegateCommand<MainWindow>(x =>
            {
                if (x.lview.SelectedItems.Count == 0)
                    return;

                var dp = ((Conv)x.DataContext).DataPieces;
                var delItems = new List<DataPiece>();

                foreach (DataPiece item in x.lview.SelectedItems)
                    delItems.Add(item);

                delItems.ForEach(d => dp.Remove(d));

                ReloadPage(dp);
            });
        }

        ~DataPiece()
        {
            Disposable.Dispose();
        }

        public void ReloadPage(ObservableCollection<DataPiece> dp)
        {
            var p = dp.Count.ToString().Length;
            foreach (var d in dp)
                d.page.Value = (dp.IndexOf(d) + 1).ToString().PadLeft(p, '0');
        }
    }
}
