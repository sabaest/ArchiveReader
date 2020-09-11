namespace PdfReader_wpf.ViewModel
{
    using System.Threading;
    using System.Windows.Threading;

    partial class ImageViewer
    {
        private Progress _p;

        private void StartProgress()
        {
            var t = new Thread(() =>
            {
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher)
                );

                _p = new Progress();

                _p.Closed += (o, args) =>
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                };

                _p.Show();
                Dispatcher.Run();
            });

            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;

            t.Start();
        }

        private void FinishProgress()
        {
            if (_p != null)
            {
                _p.Dispatcher.InvokeShutdown();
                _p = null;
            }
        }

    }
}
