using System.IO;
using System.Windows;
using Microsoft.Win32;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class MarkdownConverterWindow : Window
    {
        private readonly MarkdownService _markdownService = new();
        private string _currentFullHtml = string.Empty;
        private string _currentBodyHtml = string.Empty;
        private readonly string? _initialMarkdown;

        public MarkdownConverterWindow(string? initialMarkdown = null)
        {
            InitializeComponent();
            _initialMarkdown = initialMarkdown;

            if (!string.IsNullOrEmpty(_initialMarkdown))
            {
                MarkdownEditor.Text = _initialMarkdown;
            }

            UpdatePreview();
        }

        private void MarkdownEditor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            try
            {
                string md = MarkdownEditor.Text;
                _currentBodyHtml = _markdownService.ToBodyHtml(md);
                _currentFullHtml = _markdownService.ToFullHtml(md);

                HtmlCodeEditor.Text = _currentFullHtml;
                HtmlPreview.NavigateToString(_currentFullHtml);
            }
            catch
            {
                // Ignore parse errors while typing
            }
        }

        private void CopyFullHtml_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentFullHtml))
            {
                Clipboard.SetText(_currentFullHtml);
                MessageBox.Show("Full HTML copied to clipboard.", "QuasarPad", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CopyBodyOnly_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentBodyHtml))
            {
                Clipboard.SetText(_currentBodyHtml);
                MessageBox.Show("Body HTML copied to clipboard.", "QuasarPad", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportHtml_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "HTML Files (*.html)|*.html|All Files (*.*)|*.*",
                Title = "Export HTML - QuasarPad",
                FileName = "document.html"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, _currentFullHtml);
                    MessageBox.Show("HTML exported successfully.", "QuasarPad", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Export failed:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            MarkdownEditor.Clear();
            HtmlCodeEditor.Clear();
            HtmlPreview.NavigateToString("<html><body></body></html>");
            _currentFullHtml = string.Empty;
            _currentBodyHtml = string.Empty;
        }

        private void LoadFromEditor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Use the button from the main window to load current content.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
