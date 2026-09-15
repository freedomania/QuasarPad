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
            catch
            {
                StatusLineCol.Text = "Ln 1, Col 1";
            }

            StatusEncoding.Text = _currentEncoding.WebName.ToUpperInvariant();
            StatusMode.Text = _isPureMode ? "Pure Mode" : "Smart Mode";
            StatusCharCount.Text = $"{MainEditor.Text.Length:N0} characters";
        }

        private void MainEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isModified)
            {
                _isModified = true;
                UpdateTitle();
            }
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
                    string content = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                    MainEditor.Text = content;
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
            if (_currentFilePath == null)
            {
                SaveAs_Click(sender, e);
                return;
            }
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

            if (dialog.ShowDialog() == true)
            {
                SaveToFile(dialog.FileName);
            }
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

            var result = MessageBox.Show(
                "Do you want to save changes?",
                "QuasarPad",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

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
            if (!ConfirmSaveIfNeeded())
            {
                e.Cancel = true;
                return;
            }
            SaveSettings();
            base.OnClosing(e);
        }

        private void Undo_Click(object sender, RoutedEventArgs e) => MainEditor.Undo();
        private void Redo_Click(object sender, RoutedEventArgs e) => MainEditor.Redo();
        private void Cut_Click(object sender, RoutedEventArgs e) => MainEditor.Cut();
        private void Copy_Click(object sender, RoutedEventArgs e) => MainEditor.Copy();
        private void Paste_Click(object sender, RoutedEventArgs e) => MainEditor.Paste();
        private void SelectAll_Click(object sender, RoutedEventArgs e) => MainEditor.SelectAll();
        private void Find_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Find will be available in the next update.", "Coming Soon");
        private void Replace_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Replace will be available in the next update.", "Coming Soon");

        private void WordWrap_Click(object sender, RoutedEventArgs e)
        {
            MainEditor.TextWrapping = MenuWordWrap.IsChecked ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }

        private void LineNumbers_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Full line numbers will be available after AvalonEdit integration.", "Info");
        }

        private void StatusBar_Click(object sender, RoutedEventArgs e)
        {
            StatusBar.Visibility = MenuStatusBar.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

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
            var window = new MarkdownConverterWindow(MainEditor.Text);
            window.Owner = this;
            window.Show();
        }

        private void JsonFormat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parsed = System.Text.Json.JsonDocument.Parse(MainEditor.Text);
                MainEditor.Text = System.Text.Json.JsonSerializer.Serialize(parsed, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                MessageBox.Show("Invalid JSON content.", "JSON Format", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings window coming soon.\n\nCurrent settings are auto-saved.", "Settings");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "QuasarPad v1.0.0\n\n" +
                "A clean, lightweight Notepad alternative for Windows.\n" +
                "Pure Mode by default • Markdown to HTML • No telemetry\n\n" +
                "Free and open. Support the project if you find it useful.",
                "About QuasarPad",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Thank you for considering supporting QuasarPad!\n\n" +
                "Donation channels:\n" +
                "• GitHub Sponsors (coming soon)\n" +
                "• Ko-fi / Buy Me a Coffee (coming soon)\n" +
                "• PromptPay (Thailand)\n\n" +
                "Your support keeps the project free and independent.",
                "Support the Project",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Supporter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Become a Supporter (optional)\n\n" +
                "One-time or monthly support unlocks:\n" +
                "• Extra themes\n" +
                "• Name in About page\n" +
                "• Early access to updates\n\n" +
                "This feature is coming soon.",
                "Become a Supporter",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
