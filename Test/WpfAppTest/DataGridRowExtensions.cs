using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WpfAppTest
{
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
}
