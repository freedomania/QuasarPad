using System.Windows;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class RegexTesterWindow : Window
    {
        private readonly ExtraToolsService _tools = new();

        public RegexTesterWindow(string? initial = null)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(initial))
                InputBox.Text = initial;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            var (result, _) = _tools.TestRegex(PatternBox.Text, InputBox.Text, ChkIgnoreCase.IsChecked == true);
            ResultBox.Text = result;
        }
    }
}
