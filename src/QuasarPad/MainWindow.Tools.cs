using System.Windows;
using QuasarPad.Services;
using QuasarPad.Views;

namespace QuasarPad
{
    public partial class MainWindow
    {
        private readonly ConverterService _converter = new();

        private void UnitConverter_Click(object sender, RoutedEventArgs e) =>
            new UnitConverterWindow { Owner = this }.Show();

        private void NumberBase_Click(object sender, RoutedEventArgs e) =>
            new NumberBaseWindow(EditorText) { Owner = this }.Show();

        private void ColorTools_Click(object sender, RoutedEventArgs e) =>
            new ColorToolsWindow { Owner = this }.Show();

        private void Generators_Click(object sender, RoutedEventArgs e) =>
            new GeneratorsWindow { Owner = this }.Show();

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
