using System.Windows;
using QuasarPad.Services;
using QuasarPad.Views;

namespace QuasarPad
{
    public partial class MainWindow
    {
        private readonly ConverterService _converter = new();

        private void ShowTool(Window win)
        {
            win.Owner = this;
            win.ShowInTaskbar = false;
            win.Closed += (_, _) =>
            {
                if (WindowState == WindowState.Minimized)
                    WindowState = WindowState.Normal;
                Activate();
                Focus();
                MainEditor.Focus();
            };
            win.Show();
        }

        // Open tools EMPTY — user pastes with Ctrl+V / right-click if needed
        private void MarkdownToHtml_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new MarkdownConverterWindow(string.Empty));

        private void HtmlTest_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new HtmlTestWindow(string.Empty));

        private void TextTools_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new TextToolsWindow(string.Empty));

        private void RegexTester_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new RegexTesterWindow(string.Empty));

        private void Diff_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new DiffWindow(string.Empty));

        private void Timestamp_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new TimestampWindow());

        private void UnitConverter_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new UnitConverterWindow());

        private void NumberBase_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new NumberBaseWindow(string.Empty));

        private void ColorTools_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new ColorToolsWindow());

        private void Generators_Click(object sender, RoutedEventArgs e) =>
            ShowTool(new GeneratorsWindow());

        // In-place editor tools still use current editor text
        private void SortLines_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _converter.SortLines(EditorText, false);
            _isModified = true;
            UpdateTitle();
        }

        private void UniqueLines_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _converter.UniqueLines(EditorText);
            _isModified = true;
            UpdateTitle();
        }

        private void SortUnique_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _converter.SortUnique(EditorText, false);
            _isModified = true;
            UpdateTitle();
        }
    }
}
