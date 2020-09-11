namespace ImageToPdfs.View.Behaviors
{
    using ImageToPdfs.Model;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    class ListViewBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseDown += PreviewMouseDown;
            this.AssociatedObject.PreviewMouseUp += PreviewMouseUp;
            this.AssociatedObject.PreviewMouseMove += PreviewMouseMove;
            this.AssociatedObject.PreviewDragEnter += PreviewDragEnter;
            this.AssociatedObject.PreviewDragOver += PreviewDragOver;
            this.AssociatedObject.PreviewDragLeave += PreviewDragLeave;
            this.AssociatedObject.PreviewDrop += PreviewDrop;

            this.AssociatedObject.SelectionChanged += SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseDown -= PreviewMouseDown;
            this.AssociatedObject.PreviewMouseUp -= PreviewMouseUp;
            this.AssociatedObject.PreviewMouseMove -= PreviewMouseMove;
            this.AssociatedObject.PreviewDragEnter -= PreviewDragEnter;
            this.AssociatedObject.PreviewDragOver -= PreviewDragOver;
            this.AssociatedObject.PreviewDragLeave -= PreviewDragLeave;
            this.AssociatedObject.PreviewDrop -= PreviewDrop;

            this.AssociatedObject.SelectionChanged -= SelectionChanged;
        }

        private const string DRAG_DATA_FMT = "ItemsControlDragAndDropData";
        private Point? startPos = null;
        private int? dragItemIdx = null;
        private DataObject dragData = null;

        protected void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:

                    // アプリ内ドラッグ許可
                    ((ViewModel.Conv)((ListView)sender).DataContext).IsDrag.Value = false;

                    if (sender is ItemsControl itemsControl)
                    {
                        //==== ドラッグアイテムを取得 ====//
                        if (e.OriginalSource is FrameworkElement dragItem)
                        {
                            //-==- ドラッグアイテムあり -==-//

                            //==== ドラッグアイテムのデータ取得 ====//
                            if (GetItemData(itemsControl, dragItem) is object itemData)
                            {
                                //==== ドラッグデータを設定 ====//
                                dragData = new DataObject(DRAG_DATA_FMT, itemData);
                                dragItemIdx = GetItemIndex(itemsControl, itemData);

                                //==== 開始位置を設定 ====//
                                var pos = e.GetPosition(itemsControl);
                                startPos = itemsControl.PointToScreen(pos);
                            }
                        }
                    }

                    break;
                case MouseButton.Right:
                    Console.WriteLine("PreviewMouseDown - BehaiviorRight!");
                    break;
            }
        }

        protected void PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((ViewModel.Conv)((ListView)sender).DataContext).IsDrag.Value = true;

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    CleanUp();
                    break;

                case MouseButton.Right:
                    Console.WriteLine("PreviewMouseUp - BehaiviorRight!");
                    break;
            }
        }

        protected void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                if (startPos != null)
                {
                    //==== ドラッグ開始可否判定 ====//
                    Point curPos = itemsControl.PointToScreen(e.GetPosition(itemsControl));
                    Vector diff = curPos - (Point)startPos;
                    if (new Func<Vector, bool>((d) =>
                        {
                            return (SystemParameters.MinimumHorizontalDragDistance < Math.Abs(d.X)) ||
                                   (SystemParameters.MinimumVerticalDragDistance < Math.Abs(d.Y));
                        })(diff))
                    {
                        //==== ドラッグ＆ドロップ開始 ====//
                        DragDrop.DoDragDrop(itemsControl, dragData, DragDropEffects.Move);

                        CleanUp();
                    }
                }
            }
        }

        protected void PreviewDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DRAG_DATA_FMT) && (sender == e.Source))
            {   //==== 操作設定：移動可 ====//
                e.Effects = DragDropEffects.Move;
            }
            else
            {   //==== 受け入れ拒否 ====//
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        protected void PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DRAG_DATA_FMT) && (sender == e.Source))
            {   //==== 操作設定：移動可 ====//
                e.Effects = DragDropEffects.Move;
            }
            else
            {   //==== 受け入れ拒否 ====//
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;

            if (!((ViewModel.Conv)((ListView)sender).DataContext).IsDrag.Value)
                InsertEffect(sender, e);
        }

        protected void PreviewDragLeave(object sender, DragEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                if (GetItemData(itemsControl, (DependencyObject)e.OriginalSource) is object dropData)
                {   // InsertEffect解除
                    ((DataPiece)dropData).InsertRight.Value = Visibility.Hidden;
                    ((DataPiece)dropData).InsertLeft.Value = Visibility.Hidden;
                }
            }
        }

        protected void PreviewDrop(object sender, DragEventArgs e)
        {
            var c = (ViewModel.Conv)((ListView)sender).DataContext;

            // アプリ内ドラッグ許可判定
            if (c.IsDrag.Value)
                return;

            if (sender is ItemsControl itemsControl)
            {
                //==== ドラッグ位置のアイテムを削除 ====//
                var items = (IList)itemsControl.ItemsSource;
                var data = dragData.GetData(DRAG_DATA_FMT);
                var dragItemIdx = items.IndexOf(data);
                items.Remove(data);

                //==== ドロップ位置にアイテムを挿入 ====//
                var dropObj = (DependencyObject)e.OriginalSource;
                var dropData = GetItemData(itemsControl, dropObj);

                if (GetItemIndex(itemsControl, dropData) is int dropItemIdx)
                {   //-==- ドロップ位置にアイテムがある -==-//
                    items.Insert(dragItemIdx > dropItemIdx 
                        ? (int)dropItemIdx : (int)dropItemIdx + 1, data);
                }
                else
                {   //-==- ドロップ位置にアイテムが無い -==-//
                    items.Add(data);
                }

                if (dropData != null)
                {   // InsertEffect解除
                    ((DataPiece)dropData).InsertRight.Value = Visibility.Hidden;
                    ((DataPiece)dropData).InsertLeft.Value = Visibility.Hidden;
                }

                // ページ数更新
                ((DataPiece)dropData).ReloadPage(c.DataPieces);

                CleanUp();
            }

            // アプリ内ドラッグ不許可
            ((ViewModel.Conv)((ListView)sender).DataContext).IsDrag.Value = true;
        }

        protected void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems.Cast<DataPiece>())
            {
                item.visible.Value = Visibility.Visible;
            }
            foreach (var item in e.RemovedItems.Cast<DataPiece>())
            {
                item.visible.Value = Visibility.Hidden;
            }
        }

        private object GetItemData(ItemsControl itemsControl, DependencyObject item)
        {
            var container = new Func<ItemsControl, DependencyObject, FrameworkElement>((ic, i) => 
            {
                if ((ic == null) || (i == null)) return null;
                return (FrameworkElement)ic.ContainerFromElement(i);
            })(itemsControl, item);

            return container?.DataContext;
        }
        private int? GetItemIndex(ItemsControl itemsControl, object data)
        {
            var items = itemsControl.ItemsSource as IList;
            int idx = items.IndexOf(data);
            return (idx != -1) ? idx : (int?)null;
        }

        private void CleanUp()
        {
            startPos = null;
            dragItemIdx = null;
            dragData = null;
        }

        private void InsertEffect(object sender, DragEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                //==== ドラッグのアイテム ====//
                var items = (IList)itemsControl.ItemsSource;
                var data = dragData.GetData(DRAG_DATA_FMT);
                var dragItemIdx = items.IndexOf(data);

                //==== ドロップ先 ====//
                var dropObj = (DependencyObject)e.OriginalSource;
                var dropData = GetItemData(itemsControl, dropObj);

                if (GetItemIndex(itemsControl, dropData) is int dropItemIdx)
                {
                    if (dropItemIdx < dragItemIdx)
                    {   // 後頁 >> 前頁
                        ((DataPiece)dropData).InsertLeft.Value = Visibility.Visible;
                    }
                    else if (dropItemIdx > dragItemIdx)
                    {   // 前頁 >> 後頁
                        ((DataPiece)dropData).InsertRight.Value = Visibility.Visible;
                    }
                }
            }
        }

    }
}
