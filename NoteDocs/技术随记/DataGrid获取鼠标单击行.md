## wpf DataGrid 实现行重复点击事件

由于行选中事件，选中后不会重复触发选中，只能取消选中，才能再次点击行，所以做了个鼠标松开事件，用于实现特定业务逻辑，具体实现方式如下，``DataGridRow.MouseUp``事件不会自动补全出来，但是可以使用：

```cs
<DataGrid AutoGenerateColumns="False"
        IsReadOnly="True"  
        ItemsSource="{Binding TestDataList, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
        DataGridRow.MouseUp="DataGrid_MouseUp">  
    <DataGrid.Columns>
        <DataGridTextColumn Header="标题"
                            Binding="{Binding Title}"
                            Width="*" />
        <DataGridTextColumn Header="描述"
                            Binding="{Binding Description}"
                            Width="*" />
    </DataGrid.Columns>
</DataGrid>

private void DataGrid_MouseUp(object sender, MouseButtonEventArgs e)
{
    DataGrid dataGrid = (DataGrid)sender;
    var pos = e.GetPosition(dataGrid);
    var result = VisualTreeHelper.HitTest(dataGrid, pos);
    if (result == null) { return; }

    var rowItem = VisualHelper.FindParent<DataGridRow>(result.VisualHit);
    if (rowItem == null) { return; }

    DataViewModel dataView = rowItem.DataContext as DataViewModel;

    if (dataView != null)
    {
        /* 处理业务逻辑 */
    }
}
```

## 通过扩展方法转换为Command方式

添加DataGrid的扩展方法：

```cs
public static class DataGridExtensions
{
    public static readonly DependencyProperty RowMouseUpCmdProperty =
        DependencyProperty.RegisterAttached(
            "RowMouseUpCmd",
            typeof(ICommand),
            typeof(DataGridExtensions),
            new FrameworkPropertyMetadata(default(ICommand), new PropertyChangedCallback(RowMouseUpCmd_ChangesCallback))
    );

    public static void SetRowMouseUpCmd(DependencyObject element, ICommand value)
        => element.SetValue(RowMouseUpCmdProperty, value);

    public static ICommand GetRowMouseUpCmd(DependencyObject element)
        => (ICommand)element.GetValue(RowMouseUpCmdProperty);

    private static void RowMouseUpCmd_ChangesCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DataGridRow dataGridRow = (DataGridRow)d;
        dataGridRow.MouseUp += DataGridRow_MouseUp;
    }

    private static void DataGridRow_MouseUp(object sender, MouseButtonEventArgs e)
    {
        DataGridRow row = (DataGridRow)sender;
        ICommand command = GetRowMouseUpCmd(row);
        if (command != null)
            command.Execute(row);
    }
}
```

DataGrid的row style中绑定命令：

```cs
<DataGrid AutoGenerateColumns="False"
            IsReadOnly="True"
            ItemsSource="{Binding TestDataList, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}">
    <DataGrid.RowStyle>
        <Style TargetType="{x:Type DataGridRow}" BasedOn="{StaticResource DataGridRowStyle}">
            <Setter Property="local:DataGridExtensions.RowMouseUpCmd"
                    Value="{Binding DataContext.DataGridRowMouseUpCmd, RelativeSource={RelativeSource AncestorType=Window}}" />
        </Style>
    </DataGrid.RowStyle>
    <DataGrid.Columns>
        <DataGridTextColumn Header="标题"
                            Binding="{Binding Title}"
                            Width="*" />
        <DataGridTextColumn Header="描述"
                            Binding="{Binding Description}"
                            Width="*" />
    </DataGrid.Columns>
</DataGrid>
```

其中``RelativeSource={RelativeSource AncestorType=Window}``表示获取的``DataContext``为``Windows``控件的``DataContext``。

``ViewModel``中绑定Command：

```cs
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
```
