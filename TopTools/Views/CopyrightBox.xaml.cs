using HandyControl.Controls;

namespace TopTools.Views
{
    /// <summary>
    /// CopyrightBox.xaml 的交互逻辑
    /// </summary>
    public partial class CopyrightBox : GlowWindow
    {
        public CopyrightBox()
        {
            InitializeComponent();

            ProgramVersion.Text = System.Windows.Application.ResourceAssembly.GetName().Version.ToString(3);
        }
    }
}
