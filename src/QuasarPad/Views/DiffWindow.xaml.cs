using System.Windows;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class DiffWindow : Window
    {
        private readonly ExtraToolsService _tools = new();

        public DiffWindow(string? initialA = null)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(initialA))
                BoxA.Text = initialA;
        }

        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            ResultBox.Text = _tools.Diff(BoxA.Text, BoxB.Text);
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ResultBox.Text))
            {
                Clipboard.SetText(ResultBox.Text);
                MessageBox.Show("Copied.", "Diff");
            }
        }
    }
}
