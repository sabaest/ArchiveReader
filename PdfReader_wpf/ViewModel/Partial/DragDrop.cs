namespace PdfReader_wpf.ViewModel
{
    using GongSolutions.Wpf.DragDrop;
    using System.Linq;
    using System.Windows;
    using static PdfReader_wpf.Model.Common;

    partial class ImageViewer : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();

            dropInfo.Effects = files.Any(fname => CheckExt(fname))
                ? DragDropEffects.Copy : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var files = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>()
                .Where(fname => CheckExt(fname)).ToList();

            if (files.Count == 0) return;

            files.ForEach(x => ProcessDivergence(x));
        }
    }
}
