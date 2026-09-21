using System.Windows;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class GeneratorsWindow : Window
    {
        private readonly ConverterService _svc = new();

        public GeneratorsWindow()
        {
            InitializeComponent();
            NewUuid_Click(this, new RoutedEventArgs());
        }

        private void NewUuid_Click(object sender, RoutedEventArgs e) => TxtUuid.Text = _svc.NewGuid();
        private void NewUuidN_Click(object sender, RoutedEventArgs e) => TxtUuid.Text = _svc.NewGuidNoDash();

        private void CopyOut_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtUuid.Text)) Clipboard.SetText(TxtUuid.Text);
        }

        private void GenPass_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(TxtLen.Text, out int len);
            if (len < 4) len = 4;
            TxtPass.Text = _svc.GeneratePassword(len,
                ChkUpper.IsChecked == true,
                ChkLower.IsChecked == true,
                ChkDigit.IsChecked == true,
                ChkSymbol.IsChecked == true);
        }

        private void CopyPass_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtPass.Text)) Clipboard.SetText(TxtPass.Text);
        }
    }
}
