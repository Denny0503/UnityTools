using PhoenixControl.Prism;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfAppTest
{
    public class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<TestDataGrid> TestDataList { get; set; } = new ObservableCollection<TestDataGrid>();

        public MainWindowViewModel()
        {
            for (int i = 0; i < 10; i++)
            {
                TestDataGrid grid = new TestDataGrid() { Title = $"测试 ： {i:0.0}", Description = $"描述 ： {i:0.00}" };
                TestDataList.Add(grid);
            }
        }

        public DelegateCommand<DataGridRow> DataGridRowMouseUpCmd => new DelegateCommand<DataGridRow>((row) =>
        {

        });
    }

    public class TestDataGrid : BindableBase
    {
        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}
