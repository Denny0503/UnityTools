using System;
using System.Windows;

namespace PhoenixSkins.Controls
{
    public delegate void ItemActionCallback(ItemActionCallbackArgs<TabControl> args);

    public class ItemActionCallbackArgs<TOwner> where TOwner : FrameworkElement
    {
        //private readonly Window _window;
        private readonly TOwner _owner;
        private readonly object _tabItem;

        public ItemActionCallbackArgs(/*Window window,*/ TOwner owner, object tabItem)
        {
            //if (window == null) throw new ArgumentNullException("window");
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            //_window = window;
            _owner = owner;
            _tabItem = tabItem;
        }

        //public Window Window
        //{
        //    get { return _window; }
        //}

        public TOwner Owner
        {
            get { return _owner; }
        }

        public object TabItem
        {
            get { return _tabItem; }
        }

        public bool IsCancelled { get; private set; }

        public void Cancel()
        {
            IsCancelled = true;
        }
    }
}
