using UnityControl.Prism;
using Prism.Regions;
using Prism.Regions.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using UnityMethods.Extend;

namespace UnityControl.Adapter
{
    public class TabControlRegionAdapter : RegionAdapterBase<UnitySkins.Controls.TabControl>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ItemsControlRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public TabControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, UnitySkins.Controls.TabControl regionTarget)
        {
        }

        /// <summary>
        /// Attach new behaviors.
        /// </summary>
        /// <param name="region">The region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        /// <remarks>
        /// This class attaches the base behaviors and also keeps the <see cref="UnitySkins.Controls.TabControl.SelectedItem"/> 
        /// and the <see cref="IRegion.ActiveViews"/> in sync.
        /// </remarks>
        protected override void AttachBehaviors(IRegion region, UnitySkins.Controls.TabControl regionTarget)
        {
            base.AttachBehaviors(region, regionTarget);
            region.Behaviors.Add(TabControlRegionSyncBehavior.BehaviorKey, new TabControlRegionSyncBehavior { HostControl = regionTarget });
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }

    }

    public class TabControlRegionSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public const string BehaviorKey = "TabControlRegionSyncBehavior";

        private static readonly DependencyProperty IsGeneratedProperty =
            DependencyProperty.RegisterAttached("IsGenerated", typeof(bool), typeof(TabControlRegionSyncBehavior), null);

        private UnitySkins.Controls.TabControl hostControl;

        public DependencyObject HostControl
        {
            get
            {
                return this.hostControl;
            }

            set
            {
                UnitySkins.Controls.TabControl newValue = value as UnitySkins.Controls.TabControl;
                if (IsAttached)
                {
                    throw new InvalidOperationException("HostControl Can not Be Set AfterAttach!");
                }

                this.hostControl = newValue ?? throw new InvalidOperationException("HostControl Must Be A UnitySkins.Controls.TabControl!");
            }
        }

        protected override void OnAttach()
        {
            if (this.hostControl == null)
            {
                throw new InvalidOperationException("HostControl Can not Be Null!");
            }

            this.SynchronizeItems();

            this.hostControl.SelectionChanged += this.OnSelectionChanged;
            this.Region.ActiveViews.CollectionChanged += this.OnActiveViewsChanged;
            this.Region.Views.CollectionChanged += this.OnViewsChanged;
            hostControl.ClosingItemCallback = ClosingItemCallbackFunc;
        }

        private void ClosingItemCallbackFunc(UnitySkins.Controls.ItemActionCallbackArgs<UnitySkins.Controls.TabControl> args)
        {
            BaseViewModel baseView = RegionUtility.GetInterfaceFromView<BaseViewModel>(args.TabItem);

            if (baseView != null
                && this.Region.Views.Contains(args.TabItem))
            {
                this.Region.Remove(args.TabItem);

                RegionUtility.RemoveRegionViewsHeader(baseView.UUID);

                RegionUtility.RemoveRegionViewsHeader(baseView.Header);

            }
        }

        private UserControl GetContainerForItem(object sourceItem)
        {
            BaseViewModel baseView = RegionUtility.GetInterfaceFromView<BaseViewModel>(sourceItem);

            foreach (UserControl item in hostControl.Items)
            {
                BaseViewModel itemView = item.DataContext as BaseViewModel;

                if (null != itemView)
                {
                    if (!RegionUtility.RegionName.IsEmpty())
                    {
                        if (RegionUtility.RegionName.StrEquals(itemView.Header))
                        {
                            RegionUtility.RegionName = "";

                            return item;
                        }
                    }
                    else
                    {
                        if (baseView.Header.StrEquals(itemView.Header)
                            || baseView.UUID.StrEquals(itemView.UUID))
                        {
                            return item;
                        }
                    }
                }
            }

            return null;
        }

        private void SynchronizeItems()
        {
            List<object> existingItems = new List<object>();
            if (this.hostControl.Items.Count > 0)
            {
                // Control must be empty before "Binding" to a region
                foreach (object childItem in this.hostControl.Items)
                {
                    existingItems.Add(childItem);
                }
            }

            foreach (object view in this.Region.Views)
            {
                this.hostControl.Items.Add(view);
            }

            foreach (object existingItem in existingItems)
            {
                this.Region.Add(existingItem);
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // e.OriginalSource == null, that's why we use sender.
            if (this.hostControl == sender && e.Source is UnitySkins.Controls.TabControl)
            {
                foreach (UserControl tabItem in e.RemovedItems)
                {
                    // check if the view is in both Views and ActiveViews collections (there may be out of sync)
                    if (this.Region.Views.Contains(tabItem) && this.Region.ActiveViews.Contains(tabItem))
                    {
                        this.Region.Deactivate(tabItem);
                    }
                }

                foreach (UserControl tabItem in e.AddedItems)
                {
                    if (this.Region.Views.Contains(tabItem) && !this.Region.ActiveViews.Contains(tabItem))
                    {
                        this.Region.Activate(tabItem);

                    }
                }

            }
        }

        private void OnActiveViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                /*UnitySkins.Controls.TabControl 标签列表不包含选择的视图，并且该视图已经关闭*/
                if (!this.hostControl.Items.Contains(e.NewItems[0])
                    && !((e.NewItems[0] as UserControl)?.IsLoaded ?? false))
                {
                    this.hostControl.Items.Add(e.NewItems[0]);

                    AddViewToList(e.NewItems[0]);
                }

                UserControl control = GetContainerForItem(e.NewItems[0]);
                if (control != null)
                {
                    this.hostControl.SelectedItem = control;
                }
            }
        }

        private void AddViewToList(object item)
        {
            //BaseViewModel baseView = RegionUtility.GetInterfaceFromView<BaseViewModel>(item);

            //if (baseView != null)
            //{
            //    RegionUtility.AddRegionViewsHeader(baseView.Header);
            //}
        }

        private void OnViewsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object newItem in e.NewItems)
                {
                    this.hostControl.Items.Add(newItem);

                    AddViewToList(newItem);
                }
            }

        }

    }
}
