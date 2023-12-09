using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UnitySkins.Controls
{
    [TemplatePart(Name = OverflowButtonKey, Type = typeof(ContextMenuToggleButton))]
    [TemplatePart(Name = HeaderPanelKey, Type = typeof(TabPanel))]
    [TemplatePart(Name = OverflowScrollviewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = ScrollButtonLeft, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ScrollButtonRight, Type = typeof(ButtonBase))]
    [TemplatePart(Name = HeaderBorder, Type = typeof(Border))]
    [TemplatePart(Name = ItemsHolderPartName, Type = typeof(Panel))]
    public class TabControl : System.Windows.Controls.TabControl
    {
        private const string OverflowButtonKey = "PART_OverflowButton";

        private const string HeaderPanelKey = "PART_HeaderPanel";

        private const string OverflowScrollviewer = "PART_OverflowScrollviewer";

        private const string ScrollButtonLeft = "PART_ScrollButtonLeft";

        private const string ScrollButtonRight = "PART_ScrollButtonRight";

        private const string HeaderBorder = "PART_HeaderBorder";
        /// <summary>
        /// Template part.
        /// </summary>
        public const string ItemsHolderPartName = "PART_ItemsHolder";

        private ContextMenuToggleButton _buttonOverflow;

        internal TabPanel HeaderPanel { get; private set; }

        private ScrollViewer _scrollViewerOverflow;

        private ButtonBase _buttonScrollLeft;

        private ButtonBase _buttonScrollRight;

        private Border _headerBorder;

        private Panel _itemsHolder;

        /// <summary>
        /// 是否为内部操作
        /// </summary>
        internal bool IsInternalAction;

        /// <summary>
        /// 是否启用动画
        /// </summary>
        public static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.Register(
            "IsAnimationEnabled", typeof(bool), typeof(TabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        /// 是否启用动画
        /// </summary>
        public bool IsAnimationEnabled
        {
            get => (bool)GetValue(IsAnimationEnabledProperty);
            set => SetValue(IsAnimationEnabledProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        /// 是否可以拖动
        /// </summary>
        public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.Register(
            "IsDraggable", typeof(bool), typeof(TabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        /// 是否可以拖动
        /// </summary>
        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        /// 是否显示关闭按钮
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.RegisterAttached(
            "ShowCloseButton", typeof(bool), typeof(TabControl), new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetShowCloseButton(DependencyObject element, bool value)
            => element.SetValue(ShowCloseButtonProperty, ValueBoxes.BooleanBox(value));

        public static bool GetShowCloseButton(DependencyObject element)
            => (bool)element.GetValue(ShowCloseButtonProperty);

        /// <summary>
        /// 是否显示关闭按钮
        /// </summary>
        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        /// <summary>
        /// 是否显示上下文菜单
        /// </summary>
        public static readonly DependencyProperty ShowContextMenuProperty = DependencyProperty.RegisterAttached(
            "ShowContextMenu", typeof(bool), typeof(TabControl), new FrameworkPropertyMetadata(ValueBoxes.TrueBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetShowContextMenu(DependencyObject element, bool value)
            => element.SetValue(ShowContextMenuProperty, ValueBoxes.BooleanBox(value));

        public static bool GetShowContextMenu(DependencyObject element)
            => (bool)element.GetValue(ShowContextMenuProperty);

        /// <summary>
        /// 是否显示上下文菜单
        /// </summary>
        public bool ShowContextMenu
        {
            get => (bool)GetValue(ShowContextMenuProperty);
            set => SetValue(ShowContextMenuProperty, value);
        }

        /// <summary>
        /// 是否将标签填充
        /// </summary>
        public static readonly DependencyProperty IsTabFillEnabledProperty = DependencyProperty.Register(
            "IsTabFillEnabled", typeof(bool), typeof(TabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        /// 是否将标签填充
        /// </summary>
        public bool IsTabFillEnabled
        {
            get => (bool)GetValue(IsTabFillEnabledProperty);
            set => SetValue(IsTabFillEnabledProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        /// 标签宽度
        /// </summary>
        public static readonly DependencyProperty TabItemWidthProperty = DependencyProperty.Register(
            "TabItemWidth", typeof(double), typeof(TabControl), new PropertyMetadata(200.0));

        /// <summary>
        /// 标签宽度
        /// </summary>
        public double TabItemWidth
        {
            get => (double)GetValue(TabItemWidthProperty);
            set => SetValue(TabItemWidthProperty, value);
        }

        /// <summary>
        /// 标签高度
        /// </summary>
        public static readonly DependencyProperty TabItemHeightProperty = DependencyProperty.Register(
            "TabItemHeight", typeof(double), typeof(TabControl), new PropertyMetadata(30.0));

        /// <summary>
        /// 标签高度
        /// </summary>
        public double TabItemHeight
        {
            get => (double)GetValue(TabItemHeightProperty);
            set => SetValue(TabItemHeightProperty, value);
        }

        /// <summary>
        /// 是否可以滚动
        /// </summary>
        public static readonly DependencyProperty IsScrollableProperty = DependencyProperty.Register(
            "IsScrollable", typeof(bool), typeof(TabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        /// 是否可以滚动
        /// </summary>
        public bool IsScrollable
        {
            get => (bool)GetValue(IsScrollableProperty);
            set => SetValue(IsScrollableProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        /// 是否显示溢出按钮
        /// </summary>
        public static readonly DependencyProperty ShowOverflowButtonProperty = DependencyProperty.Register(
            "ShowOverflowButton", typeof(bool), typeof(TabControl), new PropertyMetadata(ValueBoxes.TrueBox));

        /// <summary>
        /// 是否显示溢出按钮
        /// </summary>
        public bool ShowOverflowButton
        {
            get => (bool)GetValue(ShowOverflowButtonProperty);
            set => SetValue(ShowOverflowButtonProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        /// 是否显示滚动按钮
        /// </summary>
        public static readonly DependencyProperty ShowScrollButtonProperty = DependencyProperty.Register(
            "ShowScrollButton", typeof(bool), typeof(TabControl), new PropertyMetadata(ValueBoxes.FalseBox));

        /// <summary>
        /// 是否显示滚动按钮
        /// </summary>
        public bool ShowScrollButton
        {
            get => (bool)GetValue(ShowScrollButtonProperty);
            set => SetValue(ShowScrollButtonProperty, ValueBoxes.BooleanBox(value));
        }

        /// <summary>
        /// Optionally allows a close item hook to be bound in.  If this propety is provided, the func must return true for the close to continue.
        /// </summary>
        public static readonly DependencyProperty ClosingItemCallbackProperty = DependencyProperty.Register(
            "ClosingItemCallback", typeof(ItemActionCallback), typeof(TabControl), new PropertyMetadata(default(ItemActionCallback)));

        /// <summary>
        /// Optionally allows a close item hook to be bound in.  If this propety is provided, the func must return true for the close to continue.
        /// </summary>
        public ItemActionCallback ClosingItemCallback
        {
            get { return (ItemActionCallback)GetValue(ClosingItemCallbackProperty); }
            set { SetValue(ClosingItemCallbackProperty, value); }
        }

        /// <summary>
        /// 可见的标签数量
        /// </summary>
        private int _itemShowCount;

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (HeaderPanel == null)
            {
                IsInternalAction = false;
                return;
            }

            if (_itemsHolder == null)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    var cp = FindChildContentPresenter(item);
                    if (cp != null)
                    {
                        _itemsHolder.Children.Remove(cp);
                    }
                }

                if (ClosingItemCallback != null)
                {
                    var callbackArgs = new ItemActionCallbackArgs<TabControl>(/*System.Windows.Window.GetWindow(this),*/ this, e.OldItems[0]);

                    ClosingItemCallback(callbackArgs);
                }
            }

            UpdateOverflowButton();

            if (IsInternalAction)
            {
                IsInternalAction = false;
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    _itemsHolder.Children.Clear();

                    if (Items.Count > 0)
                    {
                        SelectedItem = base.Items[0];
                        UpdateSelectedItem();
                    }

                    break;

                case NotifyCollectionChangedAction.Add:
                    UpdateSelectedItem();
                    //if (e.NewItems.Count == 1 && Items.Count > 1 && _dragablzItemsControl != null && _interTabTransfer == null)
                    //    _dragablzItemsControl.MoveItem(new MoveItemRequest(e.NewItems[0], SelectedItem, AddLocationHint));

                    break;

                case NotifyCollectionChangedAction.Remove:


                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace not implemented yet");
            }

            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    for (var i = 0; i < Items.Count; i++)
            //    {
            //        if (!(ItemContainerGenerator.ContainerFromIndex(i) is TabItem item)) return;
            //        item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //        item.TabPanel = HeaderPanel;
            //    }
            //}

            _headerBorder?.InvalidateMeasure();
            IsInternalAction = false;

        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            UpdateSelectedItem();

            if (e.AddedItems.Count != 0)
            {
                TabItem item = ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]) as TabItem;
                if (item != null)
                {
                    item.BringIntoView();
                    item.Focus();
                }
            }

        }

        public override void OnApplyTemplate()
        {
            if (_buttonOverflow != null)
            {
                if (_buttonOverflow.Menu != null)
                {
                    _buttonOverflow.Menu.Closed -= Menu_Closed;
                    _buttonOverflow.Menu = null;
                }

                _buttonOverflow.Click -= ButtonOverflow_Click;
            }

            if (_buttonScrollLeft != null)
            {
                _buttonScrollLeft.Click -= ButtonScrollLeft_Click;
            }

            if (_buttonScrollRight != null)
            {
                _buttonScrollRight.Click -= ButtonScrollRight_Click;
            }

            base.OnApplyTemplate();

            HeaderPanel = GetTemplateChild(HeaderPanelKey) as TabPanel;

            if (IsTabFillEnabled)
            {
                return;
            }

            _buttonOverflow = GetTemplateChild(OverflowButtonKey) as ContextMenuToggleButton;
            _scrollViewerOverflow = GetTemplateChild(OverflowScrollviewer) as ScrollViewer;
            _buttonScrollLeft = GetTemplateChild(ScrollButtonLeft) as ButtonBase;
            _buttonScrollRight = GetTemplateChild(ScrollButtonRight) as ButtonBase;
            _headerBorder = GetTemplateChild(HeaderBorder) as Border;
            _itemsHolder = GetTemplateChild(ItemsHolderPartName) as Panel;

            if (_buttonScrollLeft != null)
            {
                _buttonScrollLeft.Click += ButtonScrollLeft_Click;
            }

            if (_buttonScrollRight != null)
            {
                _buttonScrollRight.Click += ButtonScrollRight_Click;
            }

            if (_buttonOverflow != null)
            {
                var menu = new ContextMenu
                {
                    Placement = PlacementMode.Bottom,
                    PlacementTarget = _buttonOverflow,
                };
                menu.Closed += Menu_Closed;
                _buttonOverflow.Menu = menu;
                _buttonOverflow.Click += ButtonOverflow_Click;
            }

            UpdateSelectedItem();
        }

        /// <summary>
        /// generate a ContentPresenter for the selected item
        /// </summary>
        private void UpdateSelectedItem()
        {
            if (_itemsHolder == null)
            {
                return;
            }

            CreateChildContentPresenter(SelectedItem);

            // show the right child
            var selectedContent = GetContent(SelectedItem);
            foreach (ContentPresenter child in _itemsHolder.Children)
            {
                var isSelected = (child.Content == selectedContent);
                child.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
                child.IsEnabled = isSelected;
            }
        }

        /// <summary>
        /// create the child ContentPresenter for the given item (could be data or a TabItem)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return;
            }

            var cp = FindChildContentPresenter(item);
            if (cp != null)
            {
                return;
            }

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter
            {
                Content = GetContent(item),
                ContentTemplate = ContentTemplate,
                ContentTemplateSelector = ContentTemplateSelector,
                ContentStringFormat = ContentStringFormat,
                Visibility = Visibility.Collapsed,
            };
            _itemsHolder.Children.Add(cp);
        }
        private static object GetContent(object item)
        {
            return (item is TabItem) ? ((TabItem)item).Content : item;
        }

        /// <summary>
        /// Find the CP for the given object.  data could be a TabItem or a piece of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is TabItem)
            {
                data = ((TabItem)data).Content;
            }

            return data == null
                ? null
                : _itemsHolder?.Children.Cast<ContentPresenter>().FirstOrDefault(cp => cp.Content == data);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateOverflowButton();
        }

        private void UpdateOverflowButton()
        {
            if (!IsTabFillEnabled)
            {
                //_itemShowCount = (int)(ActualWidth / TabItemWidth);
                _buttonOverflow?.Show(ShowOverflowButton && Items.Count > 1 && Items.Count >= _itemShowCount);
            }
        }

        private void Menu_Closed(object sender, RoutedEventArgs e) => _buttonOverflow.IsChecked = false;

        private void ButtonScrollRight_Click(object sender, RoutedEventArgs e) =>
            _scrollViewerOverflow.ScrollToHorizontalOffsetWithAnimation(Math.Min(
                _scrollViewerOverflow.CurrentHorizontalOffset + TabItemWidth, _scrollViewerOverflow.ScrollableWidth));

        private void ButtonScrollLeft_Click(object sender, RoutedEventArgs e) =>
            _scrollViewerOverflow.ScrollToHorizontalOffsetWithAnimation(Math.Max(
                _scrollViewerOverflow.CurrentHorizontalOffset - TabItemWidth, 0));

        private void ButtonOverflow_Click(object sender, RoutedEventArgs e)
        {
            if (_buttonOverflow.IsChecked == true)
            {
                _buttonOverflow.Menu.Items.Clear();
                for (var i = 0; i < Items.Count; i++)
                {
                    if (!(ItemContainerGenerator.ContainerFromIndex(i) is TabItem item))
                    {
                        continue;
                    }

                    var menuItem = new MenuItem
                    {
                        HeaderStringFormat = ItemStringFormat,
                        HeaderTemplate = ItemTemplate,
                        HeaderTemplateSelector = ItemTemplateSelector,
                        Header = item.Header,
                        IsChecked = item.IsSelected,
                        IsCheckable = true,
                    };

                    menuItem.Click += delegate
                    {
                        _buttonOverflow.IsChecked = false;

                        var list = GetActualList();
                        if (list == null)
                        {
                            return;
                        }

                        var actualItem = ItemContainerGenerator.ItemFromContainer(item);
                        if (actualItem == null)
                        {
                            return;
                        }

                        var index = list.IndexOf(actualItem);
                        //if (index >= _itemShowCount)
                        //{
                        //    //list.Remove(actualItem);
                        //    //list.Insert(1, actualItem);
                        //    //if (IsAnimationEnabled)
                        //    //{
                        //    //    HeaderPanel.SetValue(TabPanel.FluidMoveDurationPropertyKey, new Duration(TimeSpan.FromMilliseconds(200)));
                        //    //}
                        //    //else
                        //    //{
                        //    //    HeaderPanel.SetValue(TabPanel.FluidMoveDurationPropertyKey, new Duration(TimeSpan.FromMilliseconds(0)));
                        //    //}
                        //    HeaderPanel.ForceUpdate = true;
                        //    HeaderPanel.Measure(new Size(HeaderPanel.DesiredSize.Width, ActualHeight));
                        //    HeaderPanel.ForceUpdate = false;
                        //    SetCurrentValue(SelectedIndexProperty, ValueBoxes.Int0Box);

                        //}

                        item.BringIntoView();
                        item.IsSelected = true;
                        SelectedIndex = list.IndexOf(actualItem);

                    };
                    _buttonOverflow.Menu.Items.Add(menuItem);
                }
            }
        }

        internal double GetHorizontalOffset() => _scrollViewerOverflow?.CurrentHorizontalOffset ?? 0;

        internal void UpdateScroll() => _scrollViewerOverflow?.RaiseEvent(new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, 0)
        {
            RoutedEvent = MouseWheelEvent
        });

        public void CloseAllItems() => CloseOtherItems(null);

        internal void CloseOtherItems(TabItem currentItem)
        {
            var actualItem = currentItem != null ? ItemContainerGenerator.ItemFromContainer(currentItem) : null;

            var list = GetActualList();
            if (list == null)
            {
                return;
            }

            IsInternalAction = true;

            if (actualItem != null)
            {
                SelectedIndex = list.IndexOf(actualItem);
            }
            else
            {
                SetCurrentValue(SelectedIndexProperty, Items.Count == 0 ? -1 : 0);
            }

            for (var i = 0; i < Items.Count; i++)
            {
                var item = list[i];
                if (!(ItemContainerGenerator.ContainerFromItem(item) is TabItem tabItem))
                {
                    continue;
                }

                if (!Equals(item, actualItem) && item != null && tabItem.ShowCloseButton)
                {
                    var argsClosing = new CancelRoutedEventArgs(TabItem.ClosingEvent, item);

                    tabItem.RaiseEvent(argsClosing);
                    if (argsClosing.Cancel)
                    {
                        return;
                    }

                    tabItem.RaiseEvent(new RoutedEventArgs(TabItem.ClosedEvent, item));

                    list.Remove(item);

                    i--;
                }
            }
        }

        internal IList GetActualList()
        {
            IList list;
            if (ItemsSource != null)
            {
                list = ItemsSource as IList;
            }
            else
            {
                list = Items;
            }

            return list;
        }

        public void RemoveItem(object item)
        {

        }

        protected override bool IsItemItsOwnContainerOverride(object item) => item is TabItem;

        protected override DependencyObject GetContainerForItemOverride() => new TabItem();
    }
}
