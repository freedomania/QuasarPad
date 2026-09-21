using System.Windows;
using System.Windows.Controls;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class NumberBaseWindow : Window
    {
        private readonly ConverterService _svc = new();

        public NumberBaseWindow(string? initial = null)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(initial))
                TxtInput.Text = initial.Trim();
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            int from = int.Parse((CmbFrom.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "10");
            int to = int.Parse((CmbTo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "10");
            TxtResult.Text = _svc.ConvertBase(TxtInput.Text, from, to);
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtResult.Text))
                Clipboard.SetText(TxtResult.Text);
        }
    }
}
