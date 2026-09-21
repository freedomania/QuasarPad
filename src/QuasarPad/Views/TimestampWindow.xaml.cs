using System.Windows;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class TimestampWindow : Window
    {
        private readonly ExtraToolsService _tools = new();

        public TimestampWindow()
        {
            InitializeComponent();
        }

        private void UnixToLocal_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = _tools.UnixToLocal(UnixBox.Text);
            DateBox.Text = ResultBox.Text.StartsWith("[") ? "" : ResultBox.Text;
        }

        private void LocalToUnix_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = _tools.LocalToUnix(DateBox.Text);
            if (!ResultBox.Text.StartsWith("["))
                UnixBox.Text = ResultBox.Text;
        }

        private void NowUnix_Click(object sender, RoutedEventArgs e)
        {
            UnixBox.Text = _tools.NowUnix();
            ResultBox.Text = UnixBox.Text;
        }

        private void NowIso_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = _tools.NowIso();
            DateBox.Text = ResultBox.Text;
        }
    }
}
