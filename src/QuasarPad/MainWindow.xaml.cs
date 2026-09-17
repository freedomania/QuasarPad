using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using QuasarPad.Services;
using QuasarPad.Views;

namespace QuasarPad
{
    public partial class MainWindow : Window
    {
        private string? _currentFilePath = null;
        private bool _isModified = false;
        private bool _isPureMode = true;
        private bool _isDarkMode = false;
        private Encoding _currentEncoding = Encoding.UTF8;
        private readonly SettingsService _settingsService = new();
        private readonly TextFormatService _textFormatService = new();

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            UpdateTitle();
            UpdateStatusBar();
        }

        private void LoadSettings()
        {
            var s = _settingsService.Settings;
            _isPureMode = s.IsPureMode;
            _isDarkMode = s.IsDarkMode;
            MenuPureMode.IsChecked = _isPureMode;
            MenuDarkMode.IsChecked = _isDarkMode;
            MenuWordWrap.IsChecked = s.WordWrap;
            MenuStatusBar.IsChecked = s.ShowStatusBar;

            MainEditor.TextWrapping = s.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            MainEditor.FontFamily = new System.Windows.Media.FontFamily(s.FontFamily);
            MainEditor.FontSize = s.FontSize;

            if (s.WindowWidth > 0) Width = s.WindowWidth;
            if (s.WindowHeight > 0) Height = s.WindowHeight;

            ApplyTheme();
        }

        private void SaveSettings()
        {
            var s = _settingsService.Settings;
            s.IsPureMode = _isPureMode;
            s.IsDarkMode = _isDarkMode;
            s.WordWrap = MenuWordWrap.IsChecked;
            s.ShowStatusBar = MenuStatusBar.IsChecked;
            s.FontFamily = MainEditor.FontFamily.Source;
            s.FontSize = MainEditor.FontSize;
            s.WindowWidth = Width;
            s.WindowHeight = Height;
            _settingsService.Save();
        }

        private void ApplyTheme()
        {
            var dict = new ResourceDictionary();
            if (_isDarkMode)
                dict.Source = new Uri("Resources/Themes/Dark.xaml", UriKind.Relative);
            else
                dict.Source = new Uri("Resources/Themes/Light.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void UpdateTitle()
        {
            string fileName = _currentFilePath != null 
                ? Path.GetFileName(_currentFilePath) 
                : "Untitled";
            string modifiedMark = _isModified ? "*" : "";
            Title = $"{modifiedMark}{fileName} - QuasarPad";
        }

        private void UpdateStatusBar()
        {
            try
            {
                int line = MainEditor.GetLineIndexFromCharacterIndex(MainEditor.CaretIndex) + 1;
                int col = MainEditor.CaretIndex - MainEditor.GetCharacterIndexFromLineIndex(line - 1) + 1;
                StatusLineCol.Text = $"Ln {line}, Col {col}";
            }
            catch { StatusLineCol.Text = "Ln 1, Col 1"; }

            StatusEncoding.Text = _currentEncoding.WebName.ToUpperInvariant();
            StatusMode.Text = _isPureMode ? "Pure Mode" : "Smart Mode";
            StatusCharCount.Text = $"{MainEditor.Text.Length:N0} characters";
        }

        private void MainEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isModified) { _isModified = true; UpdateTitle(); }
            UpdateStatusBar();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveIfNeeded()) return;
            MainEditor.Clear();
            _currentFilePath = null;
            _isModified = false;
            UpdateTitle();
            UpdateStatusBar();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveIfNeeded()) return;
            var dialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*|Text Files (*.txt)|*.txt|Markdown (*.md)|*.md|JSON (*.json)|*.json|HTML (*.html)|*.html",
                Title = "Open File - QuasarPad"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    MainEditor.Text = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                    _currentFilePath = dialog.FileName;
                    _isModified = false;
                    UpdateTitle();
                    UpdateStatusBar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Cannot open file:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFilePath == null) { SaveAs_Click(sender, e); return; }
            SaveToFile(_currentFilePath);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|Markdown (*.md)|*.md|JSON (*.json)|*.json|HTML (*.html)|*.html|All Files (*.*)|*.*",
                Title = "Save As - QuasarPad",
                FileName = _currentFilePath != null ? Path.GetFileName(_currentFilePath) : "Untitled.txt"
            };
            if (dialog.ShowDialog() == true) SaveToFile(dialog.FileName);
        }

        private void SaveToFile(string path)
        {
            try
            {
                File.WriteAllText(path, MainEditor.Text, _currentEncoding);
                _currentFilePath = path;
                _isModified = false;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot save file:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ConfirmSaveIfNeeded()
        {
            if (!_isModified) return true;
            var result = MessageBox.Show("Do you want to save changes?", "QuasarPad", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel) return false;
            if (result == MessageBoxResult.Yes)
            {
                Save_Click(this, new RoutedEventArgs());
                return !_isModified;
            }
            return true;
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!ConfirmSaveIfNeeded()) { e.Cancel = true; return; }
            SaveSettings();
            base.OnClosing(e);
        }

        private void Undo_Click(object sender, RoutedEventArgs e) => MainEditor.Undo();
        private void Redo_Click(object sender, RoutedEventArgs e) => MainEditor.Redo();
        private void Cut_Click(object sender, RoutedEventArgs e) => MainEditor.Cut();
        private void Copy_Click(object sender, RoutedEventArgs e) => MainEditor.Copy();
        private void Paste_Click(object sender, RoutedEventArgs e) => MainEditor.Paste();
        private void SelectAll_Click(object sender, RoutedEventArgs e) => MainEditor.SelectAll();

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window { Title = "Find - QuasarPad", Width = 360, Height = 140, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, ResizeMode = ResizeMode.NoResize };
            var panel = new StackPanel { Margin = new Thickness(12) };
            var textBox = new TextBox { Height = 28, Margin = new Thickness(0, 0, 0, 12) };
            var btnFind = new Button { Content = "Find Next", Width = 90, Margin = new Thickness(0, 0, 8, 0), IsDefault = true };
            var btnCancel = new Button { Content = "Cancel", Width = 80, IsCancel = true };
            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            btnFind.Click += (s, args) =>
            {
                string search = textBox.Text;
                if (string.IsNullOrEmpty(search)) return;
                int start = MainEditor.SelectionStart + MainEditor.SelectionLength;
                int index = MainEditor.Text.IndexOf(search, start, StringComparison.OrdinalIgnoreCase);
                if (index < 0) index = MainEditor.Text.IndexOf(search, 0, StringComparison.OrdinalIgnoreCase);
                if (index >= 0) { MainEditor.Select(index, search.Length); MainEditor.Focus(); }
                else MessageBox.Show("Text not found.", "Find", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            btnCancel.Click += (s, args) => dialog.Close();
            btnPanel.Children.Add(btnFind); btnPanel.Children.Add(btnCancel);
            panel.Children.Add(new TextBlock { Text = "Find what:", Margin = new Thickness(0, 0, 0, 6) });
            panel.Children.Add(textBox); panel.Children.Add(btnPanel);
            dialog.Content = panel; dialog.ShowDialog();
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window { Title = "Replace - QuasarPad", Width = 380, Height = 200, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, ResizeMode = ResizeMode.NoResize };
            var panel = new StackPanel { Margin = new Thickness(12) };
            var findBox = new TextBox { Height = 28, Margin = new Thickness(0, 0, 0, 10) };
            var replaceBox = new TextBox { Height = 28, Margin = new Thickness(0, 0, 0, 12) };
            var btnReplaceAll = new Button { Content = "Replace All", Width = 100, Margin = new Thickness(0, 0, 8, 0), IsDefault = true };
            var btnCancel = new Button { Content = "Cancel", Width = 80, IsCancel = true };
            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            btnReplaceAll.Click += (s, args) =>
            {
                string search = findBox.Text;
                if (string.IsNullOrEmpty(search)) return;
                string replace = replaceBox.Text ?? "";
                int count = 0, idx = 0;
                string text = MainEditor.Text;
                var sb = new StringBuilder();
                while (true)
                {
                    int found = text.IndexOf(search, idx, StringComparison.OrdinalIgnoreCase);
                    if (found < 0) { sb.Append(text.Substring(idx)); break; }
                    sb.Append(text.Substring(idx, found - idx));
                    sb.Append(replace);
                    idx = found + search.Length;
                    count++;
                }
                MainEditor.Text = sb.ToString();
                _isModified = true; UpdateTitle(); dialog.Close();
                MessageBox.Show($"Replaced {count} occurrence(s).", "Replace", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            btnCancel.Click += (s, args) => dialog.Close();
            btnPanel.Children.Add(btnReplaceAll); btnPanel.Children.Add(btnCancel);
            panel.Children.Add(new TextBlock { Text = "Find what:", Margin = new Thickness(0, 0, 0, 4) });
            panel.Children.Add(findBox);
            panel.Children.Add(new TextBlock { Text = "Replace with:", Margin = new Thickness(0, 0, 0, 4) });
            panel.Children.Add(replaceBox); panel.Children.Add(btnPanel);
            dialog.Content = panel; dialog.ShowDialog();
        }

        private void WordWrap_Click(object sender, RoutedEventArgs e) =>
            MainEditor.TextWrapping = MenuWordWrap.IsChecked ? TextWrapping.Wrap : TextWrapping.NoWrap;

        private void LineNumbers_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Line numbers need AvalonEdit (planned). Current version uses standard TextBox.", "Line Numbers");

        private void StatusBar_Click(object sender, RoutedEventArgs e) =>
            StatusBar.Visibility = MenuStatusBar.IsChecked ? Visibility.Visible : Visibility.Collapsed;

        private void PureMode_Click(object sender, RoutedEventArgs e)
        {
            _isPureMode = MenuPureMode.IsChecked;
            UpdateStatusBar();
        }

        private void DarkMode_Click(object sender, RoutedEventArgs e)
        {
            _isDarkMode = MenuDarkMode.IsChecked;
            ApplyTheme();
        }

        private void MarkdownToHtml_Click(object sender, RoutedEventArgs e)
        {
            var window = new MarkdownConverterWindow(MainEditor.Text) { Owner = this };
            window.Show();
        }

        private void HtmlTest_Click(object sender, RoutedEventArgs e)
        {
            var window = new HtmlTestWindow(MainEditor.Text) { Owner = this };
            window.Show();
        }

        private void TextTools_Click(object sender, RoutedEventArgs e)
        {
            var window = new TextToolsWindow(MainEditor.Text) { Owner = this };
            window.Show();
        }

        private void JsonFormat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parsed = System.Text.Json.JsonDocument.Parse(MainEditor.Text);
                MainEditor.Text = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                _isModified = true; UpdateTitle();
            }
            catch { MessageBox.Show("Invalid JSON content.", "JSON Format", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void FormatText_Click(object sender, RoutedEventArgs e)
        {
            MainEditor.Text = _textFormatService.FormatPlainText(MainEditor.Text);
            _isModified = true; UpdateTitle();
            MessageBox.Show("Text formatted.", "Format Text");
        }

        private void ReflowText_Click(object sender, RoutedEventArgs e)
        {
            MainEditor.Text = _textFormatService.ReflowParagraphs(MainEditor.Text, 80);
            _isModified = true; UpdateTitle();
            MessageBox.Show("Paragraphs reflowed (~80 chars/line).", "Reflow Paragraphs");
        }

        private void Settings_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Settings auto-save on exit:\n• Theme\n• Pure Mode\n• Word Wrap\n• Window size\n\nFull Settings UI coming later.", "Settings");

        private void About_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("QuasarPad v1.1.0\n\nOffline Text & Markup Toolkit\n• Pure Mode Notepad\n• Markdown → HTML\n• HTML Test\n• Text Tools (Base64, Hash, URL...)\n\nNo telemetry. MIT License.", "About QuasarPad");

        private void Support_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Support QuasarPad (optional):\n\n• GitHub: https://github.com/freedomania/QuasarPad\n• Sponsors / Ko-fi / PromptPay — coming soon\n\nThank you!", "Support the Project");

        private void Supporter_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Supporter is optional.\nApp stays free for everyone.\n\nPlanned perks: extra themes, name in About, early features.", "Become a Supporter");
    }
}
