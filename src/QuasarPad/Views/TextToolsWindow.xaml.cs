using System.Windows;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class TextToolsWindow : Window
    {
        private readonly TextToolsService _tools = new();

        public TextToolsWindow(string? initialText = null)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(initialText))
                InputBox.Text = initialText;
        }

        private void Base64Encode_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.ToBase64(InputBox.Text);
        private void Base64Decode_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.FromBase64(InputBox.Text);
        private void UrlEncode_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.UrlEncode(InputBox.Text);
        private void UrlDecode_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.UrlDecode(InputBox.Text);
        private void Upper_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.ToUpper(InputBox.Text);
        private void Lower_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.ToLower(InputBox.Text);
        private void TitleCase_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.ToTitleCase(InputBox.Text);
        private void Reverse_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.Reverse(InputBox.Text);
        private void Md5_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.Md5(InputBox.Text);
        private void Sha256_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.Sha256(InputBox.Text);
        private void RemoveEmpty_Click(object sender, RoutedEventArgs e) => OutputBox.Text = _tools.RemoveEmptyLines(InputBox.Text);

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OutputBox.Text))
            {
                Clipboard.SetText(OutputBox.Text);
                MessageBox.Show("Result copied to clipboard.", "Text Tools", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
