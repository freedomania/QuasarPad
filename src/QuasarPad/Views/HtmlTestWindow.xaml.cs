using System.Windows;

namespace QuasarPad.Views
{
    public partial class HtmlTestWindow : Window
    {
        private readonly string? _initialHtml;

        public HtmlTestWindow(string? initialHtml = null)
        {
            InitializeComponent();
            _initialHtml = initialHtml;

            if (!string.IsNullOrEmpty(_initialHtml))
            {
                HtmlEditor.Text = _initialHtml;
            }
            else
            {
                HtmlEditor.Text = "<!DOCTYPE html>\n<html>\n<head>\n<meta charset=\"UTF-8\">\n<title>Test</title>\n</head>\n<body>\n<h1>Hello QuasarPad</h1>\n<p>Edit HTML on the left to see live preview.</p>\n</body>\n</html>";
            }

            RefreshPreview();
        }

        private void HtmlEditor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RefreshPreview();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => RefreshPreview();

        private void RefreshPreview()
        {
            try
            {
                string html = HtmlEditor.Text;
                if (string.IsNullOrWhiteSpace(html))
                    html = "<html><body></body></html>";
                PreviewBrowser.NavigateToString(html);
            }
            catch { }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(HtmlEditor.Text))
            {
                Clipboard.SetText(HtmlEditor.Text);
                MessageBox.Show("HTML copied to clipboard.", "HTML Test", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadFromEditor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open this window from Tools menu with content from the main editor.", "Info");
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            HtmlEditor.Clear();
            PreviewBrowser.NavigateToString("<html><body></body></html>");
        }
    }
}
