
using HandyControl.Data;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows;
using System;

namespace PhoenixControl.Prism
{
    public class BaseViewModel : BindableBase
    {
        /// <summary>
        /// 唯一标识，用于多开页面
        /// </summary>
        public string UUID { private set; get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// table Control的标题
        /// </summary>
        public string Header { get; set; }

        public IRegionManager RegionManager;
        public IEventAggregator EventAggregator;
        public IModuleManager moduleManager;

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 右侧滑出信息
        /// </summary>
        public bool IsRightDrawerOpen { set; get; }

        /// <summary>
        /// 功能类型
        /// </summary>
        public int AuthorizationType { set; get; } = 3;

        #region 分页

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { set; get; } = 1;

        /// <summary>
        /// 最大页码数
        /// </summary>
        public int MaxPageCount { set; get; } = 1;
        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalQuantity { set; get; }

        /// <summary>
        /// 每页数据条数
        /// </summary>
        public int PageSize { set; get; } = 50;

        /// <summary>
        /// 分页大小选项列表
        /// </summary>
        public ObservableCollection<ComboBoxDataBing> PageSizeOptions { get; set; } = new ObservableCollection<ComboBoxDataBing>();

        /// <summary>
        /// 页码改变命令
        /// </summary>
        public DelegateCommand<FunctionEventArgs<int>> PageUpdatedCmd =>
            new Lazy<DelegateCommand<FunctionEventArgs<int>>>(() =>
                new DelegateCommand<FunctionEventArgs<int>>(PageUpdated)).Value;

        /// <summary>
        ///     页码改变
        /// </summary>
        protected virtual void PageUpdated(FunctionEventArgs<int> info) { }

        #endregion

        public DelegateCommand<string> FunctionCommand => new Lazy<DelegateCommand<string>>(() =>
         new DelegateCommand<string>(FunctionCommandFunc)).Value;

        protected virtual void FunctionCommandFunc(string command) { }

        /// <summary>
        /// 关闭弹窗(临时使用)
        /// </summary>
        public Action<string> CloseAction;

        /// <summary>
        /// 最大数据显示高度
        /// </summary>
        public double MaxDataHeight { get { return SystemParameters.PrimaryScreenHeight - 300; } }

        public BaseViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IModuleManager moduleManager)
        {
            this.RegionManager = regionManager;
            this.EventAggregator = eventAggregator;
            this.moduleManager = moduleManager;

            InitPageSize();
        }

        public BaseViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.RegionManager = regionManager;
            this.EventAggregator = eventAggregator;

            InitPageSize();
        }

        public BaseViewModel(IEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;
            InitPageSize();
        }

        public BaseViewModel()
        {
            InitPageSize();
        }

        /// <summary>
        /// 设置分页大小选项列表
        /// </summary>
        private void InitPageSize()
        {
            PageSizeOptions.Clear();
            PageSizeOptions.Add(new ComboBoxDataBing() { Key = "10", Title = "10" });
            PageSizeOptions.Add(new ComboBoxDataBing() { Key = "20", Title = "20" });
            PageSizeOptions.Add(new ComboBoxDataBing() { Key = "30", Title = "30" });
            PageSizeOptions.Add(new ComboBoxDataBing() { Key = "50", Title = "50" });
            PageSizeOptions.Add(new ComboBoxDataBing() { Key = "100", Title = "100" });
        }
    }
}
