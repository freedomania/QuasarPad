using System.Windows;
using QuasarPad.Services;
using QuasarPad.Views;

namespace QuasarPad
{
    public partial class MainWindow
    {
        private readonly ConverterService _converter = new();

        /// <summary>
        /// Seed tool windows from editor selection or full text only.
        /// Do NOT auto-read clipboard (user pastes with Ctrl+V / right-click).
        /// </summary>
        private string GetToolSeedText()
        {
            if (!string.IsNullOrEmpty(MainEditor.SelectedText))
                return MainEditor.SelectedText;
            return EditorText ?? string.Empty;
        }

        private void ShowTool(Window win)
        {
            win.Owner = this;
            win.ShowInTaskbar = false;
            win.Closed += (_, _) =>
            {
                // Prevent main window from staying minimized / unfocused after tool closes
                if (WindowState == WindowState.Minimized)
                    WindowState = WindowState.Normal;
                Activate();
                Focus();
                MainEditor.Focus();
            };
            win.Show();
        }

        private void MarkdownToHtml_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new MarkdownConverterWindow(GetToolSeedText()));

        private void HtmlTest_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new HtmlTestWindow(GetToolSeedText()));

        private void TextTools_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new TextToolsWindow(GetToolSeedText()));

        private void RegexTester_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new RegexTesterWindow(GetToolSeedText()));

        private void Diff_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new DiffWindow(GetToolSeedText()));

        private void Timestamp_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new TimestampWindow());

        private void UnitConverter_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new UnitConverterWindow());

        private void NumberBase_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new NumberBaseWindow(GetToolSeedText()));

        private void ColorTools_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new ColorToolsWindow());

        private void Generators_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new GeneratorsWindow());

        private void SortLines_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _converter.SortLines(GetToolSeedText(), false);
            _isModified = true;
            UpdateTitle();
        }

        private void UniqueLines_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _converter.UniqueLines(GetToolSeedText());
            _isModified = true;
            UpdateTitle();
        }

        private void SortUnique_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _converter.SortUnique(GetToolSeedText(), false);
            _isModified = true;
            UpdateTitle();
        }
    }
}
