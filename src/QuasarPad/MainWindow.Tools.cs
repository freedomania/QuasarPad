using System.Windows;
using QuasarPad.Services;
using QuasarPad.Views;

namespace QuasarPad
{
    public partial class MainWindow
    {
        private readonly ConverterService _converter = new();

        /// <summary>
        /// Prefer clipboard text (copied from anywhere),
        /// else selected text, else full editor content.
        /// </summary>
        private string GetToolSeedText()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clip = Clipboard.GetText();
                    if (!string.IsNullOrWhiteSpace(clip))
                        return clip;
                }
            }
            catch
            {
                // clipboard can fail if locked by another app
            }

            if (!string.IsNullOrEmpty(MainEditor.SelectedText))
                return MainEditor.SelectedText;

            return EditorText ?? string.Empty;
        }

        private void MarkdownToHtml_Click(object sender, RoutedEventArgs e) =>
            new MarkdownConverterWindow(GetToolSeedText()) { Owner = this }.Show();

        private void HtmlTest_Click(object sender, RoutedEventArgs e) =>
            new HtmlTestWindow(GetToolSeedText()) { Owner = this }.Show();

        private void TextTools_Click(object sender, RoutedEventArgs e) =>
            new TextToolsWindow(GetToolSeedText()) { Owner = this }.Show();

        private void RegexTester_Click(object sender, RoutedEventArgs e) =>
            new RegexTesterWindow(GetToolSeedText()) { Owner = this }.Show();

        private void Diff_Click(object sender, RoutedEventArgs e) =>
            new DiffWindow(GetToolSeedText()) { Owner = this }.Show();

        private void Timestamp_Click(object sender, RoutedEventArgs e) =>
            new TimestampWindow { Owner = this }.Show();

        private void UnitConverter_Click(object sender, RoutedEventArgs e) =>
            new UnitConverterWindow { Owner = this }.Show();

        private void NumberBase_Click(object sender, RoutedEventArgs e) =>
            new NumberBaseWindow(GetToolSeedText()) { Owner = this }.Show();

        private void ColorTools_Click(object sender, RoutedEventArgs e) =>
            new ColorToolsWindow { Owner = this }.Show();

        private void Generators_Click(object sender, RoutedEventArgs e) =>
            new GeneratorsWindow { Owner = this }.Show();

        private void SortLines_Click(object sender, RoutedEventArgs e)
        {
            string src = GetToolSeedText();
            EditorText = _converter.SortLines(src, false);
            _isModified = true;
            UpdateTitle();
        }

        private void UniqueLines_Click(object sender, RoutedEventArgs e)
        {
            string src = GetToolSeedText();
            EditorText = _converter.UniqueLines(src);
            _isModified = true;
            UpdateTitle();
        }

        private void SortUnique_Click(object sender, RoutedEventArgs e)
        {
            string src = GetToolSeedText();
            EditorText = _converter.SortUnique(src, false);
            _isModified = true;
            UpdateTitle();
        }
    }
}
