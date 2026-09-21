using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using QuasarPad.Services;
using QuasarPad.Views;

namespace QuasarPad
{
    public partial class MainWindow : Window
    {
        private string? _currentFilePath;
        private bool _isModified;
        private bool _isPureMode = true;
        private bool _isDarkMode;
        private Encoding _currentEncoding = Encoding.UTF8;
        private readonly SettingsService _settingsService = new();
        private readonly TextFormatService _textFormatService = new();
        private readonly ExtraToolsService _extraTools = new();

        private static readonly SolidColorBrush DarkBg = BrushFrom("#1E1E1E");
        private static readonly SolidColorBrush DarkFg = BrushFrom("#D4D4D4");
        private static readonly SolidColorBrush DarkMenu = BrushFrom("#3C3C3C");
        private static readonly SolidColorBrush DarkMenuFg = BrushFrom("#FFFFFF");
        private static readonly SolidColorBrush DarkPopup = BrushFrom("#2D2D2D");
        private static readonly SolidColorBrush DarkHighlight = BrushFrom("#094771");
        private static readonly SolidColorBrush LightBg = BrushFrom("#FFFFFF");
        private static readonly SolidColorBrush LightFg = BrushFrom("#1E1E1E");
        private static readonly SolidColorBrush LightMenu = BrushFrom("#F0F0F0");
        private static readonly SolidColorBrush LightHighlight = BrushFrom("#CCE8FF");
        private static readonly SolidColorBrush StatusBlue = BrushFrom("#007ACC");

        private static SolidColorBrush BrushFrom(string hex)
        {
            var b = (SolidColorBrush)new BrushConverter().ConvertFrom(hex)!;
            b.Freeze();
            return b;
        }

        public MainWindow()
        {
            InitializeComponent();
            InitTabs();
            MainEditor.TextArea.Caret.PositionChanged += (s, e) => UpdateStatusBar();
            LoadSettings();
            UpdateTitle();
            UpdateStatusBar();
        }

        private string EditorText
        {
            get => MainEditor.Text;
            set
            {
                _suppressTextChanged = true;
                MainEditor.Text = value;
                _suppressTextChanged = false;
            }
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
            MenuLineNumbers.IsChecked = s.ShowLineNumbers;
            MainEditor.WordWrap = s.WordWrap;
            MainEditor.ShowLineNumbers = s.ShowLineNumbers;
            MainEditor.FontFamily = new FontFamily(s.FontFamily);
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
            s.ShowLineNumbers = MenuLineNumbers.IsChecked;
            s.FontFamily = MainEditor.FontFamily.Source;
            s.FontSize = MainEditor.FontSize;
            s.WindowWidth = Width;
            s.WindowHeight = Height;
            _settingsService.Save();
        }

        private void ApplyTheme()
        {
            if (_isDarkMode)
            {
                Background = DarkBg;
                MainEditor.Background = DarkBg;
                MainEditor.Foreground = DarkFg;
                MainEditor.LineNumbersForeground = DarkMenuFg;
                MainEditor.TextArea.Foreground = DarkFg;
                Application.Current.Resources[SystemColors.MenuBrushKey] = DarkPopup;
                Application.Current.Resources[SystemColors.MenuBarBrushKey] = DarkMenu;
                Application.Current.Resources[SystemColors.MenuTextBrushKey] = DarkMenuFg;
                Application.Current.Resources[SystemColors.HighlightBrushKey] = DarkHighlight;
                Application.Current.Resources[SystemColors.HighlightTextBrushKey] = DarkMenuFg;
                Application.Current.Resources[SystemColors.ControlBrushKey] = DarkPopup;
                Application.Current.Resources[SystemColors.ControlTextBrushKey] = DarkMenuFg;
                Application.Current.Resources[SystemColors.WindowBrushKey] = DarkBg;
                Application.Current.Resources[SystemColors.WindowTextBrushKey] = DarkFg;
                if (Content is DockPanel dp && dp.Children.Count > 0 && dp.Children[0] is Menu m)
                {
                    m.Background = DarkMenu;
                    m.Foreground = DarkMenuFg;
                    foreach (var item in m.Items)
                        if (item is MenuItem mi) { mi.Foreground = DarkMenuFg; mi.Background = Brushes.Transparent; }
                }
                StatusBar.Background = StatusBlue;
            }
            else
            {
                Background = LightBg;
                MainEditor.Background = LightBg;
                MainEditor.Foreground = LightFg;
                MainEditor.LineNumbersForeground = LightFg;
                MainEditor.TextArea.Foreground = LightFg;
                Application.Current.Resources[SystemColors.MenuBrushKey] = LightBg;
                Application.Current.Resources[SystemColors.MenuBarBrushKey] = LightMenu;
                Application.Current.Resources[SystemColors.MenuTextBrushKey] = LightFg;
                Application.Current.Resources[SystemColors.HighlightBrushKey] = LightHighlight;
                Application.Current.Resources[SystemColors.HighlightTextBrushKey] = LightFg;
                Application.Current.Resources[SystemColors.ControlBrushKey] = LightBg;
                Application.Current.Resources[SystemColors.ControlTextBrushKey] = LightFg;
                Application.Current.Resources[SystemColors.WindowBrushKey] = LightBg;
                Application.Current.Resources[SystemColors.WindowTextBrushKey] = LightFg;
                if (Content is DockPanel dp && dp.Children.Count > 0 && dp.Children[0] is Menu m)
                {
                    m.Background = LightMenu;
                    m.Foreground = LightFg;
                    foreach (var item in m.Items)
                        if (item is MenuItem mi) { mi.Foreground = LightFg; mi.Background = Brushes.Transparent; }
                }
                StatusBar.Background = StatusBlue;
            }
        }

        private void UpdateTitle()
        {
            string name = _currentFilePath != null ? Path.GetFileName(_currentFilePath) : (CurrentTab?.Title ?? "Untitled");
            Title = $"{(_isModified ? "*" : "")}{name} - QuasarPad";
        }

        private void UpdateStatusBar()
        {
            try
            {
                var loc = MainEditor.Document.GetLocation(MainEditor.CaretOffset);
                StatusLineCol.Text = $"Ln {loc.Line}, Col {loc.Column}";
            }
            catch { StatusLineCol.Text = "Ln 1, Col 1"; }
            StatusEncoding.Text = _currentEncoding.WebName.ToUpperInvariant();
            StatusMode.Text = _isPureMode ? "Pure Mode" : "Smart Mode";
            StatusCharCount.Text = $"{MainEditor.Text.Length:N0} characters";
        }

        private void MainEditor_TextChanged(object? sender, EventArgs e)
        {
            if (_suppressTextChanged) return;
            if (!_isModified) { _isModified = true; UpdateTitle(); }
            if (CurrentTab != null)
            {
                CurrentTab.IsModified = true;
                CurrentTab.Content = MainEditor.Text;
            }
            RefreshTabHeaders();
            UpdateStatusBar();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*|Text (*.txt)|*.txt|Markdown (*.md)|*.md|JSON (*.json)|*.json|HTML (*.html)|*.html",
                Title = "Open - QuasarPad"
            };
            if (dialog.ShowDialog() != true) return;
            try
            {
                string text = File.ReadAllText(dialog.FileName, Encoding.UTF8);
                // Open in a new tab
                SaveEditorToCurrentTab();
                string title = Path.GetFileName(dialog.FileName);
                AddTabInternal(title, dialog.FileName, text);
                RefreshTabHeaders();
                _suppressTabSwitch = true;
                DocTabs.SelectedIndex = _tabs.Count - 1;
                _suppressTabSwitch = false;
                LoadTabIntoEditor(DocTabs.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFilePath == null) SaveAs_Click(sender, e);
            else SaveToFile(_currentFilePath);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text (*.txt)|*.txt|Markdown (*.md)|*.md|JSON (*.json)|*.json|HTML (*.html)|*.html|All (*.*)|*.*",
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
                SyncTabAfterSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot save:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ConfirmSaveIfNeeded()
        {
            if (!_isModified) return true;
            var r = MessageBox.Show("Save changes?", "QuasarPad", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (r == MessageBoxResult.Cancel) return false;
            if (r == MessageBoxResult.Yes)
            {
                Save_Click(this, new RoutedEventArgs());
                return !_isModified;
            }
            return true;
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveEditorToCurrentTab();
            foreach (var tab in _tabs)
            {
                if (!tab.IsModified) continue;
                // switch and ask
            }
            if (!ConfirmSaveIfNeeded()) { e.Cancel = true; return; }
            SaveSettings();
            base.OnClosing(e);
        }

        private void Undo_Click(object sender, RoutedEventArgs e) => MainEditor.Undo();
        private void Redo_Click(object sender, RoutedEventArgs e) => MainEditor.Redo();
        private void Cut_Click(object sender, RoutedEventArgs e) { if (MainEditor.SelectionLength > 0) MainEditor.Cut(); }
        private void Copy_Click(object sender, RoutedEventArgs e) { if (MainEditor.SelectionLength > 0) MainEditor.Copy(); }
        private void Paste_Click(object sender, RoutedEventArgs e) => MainEditor.Paste();
        private void SelectAll_Click(object sender, RoutedEventArgs e) => MainEditor.SelectAll();

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window { Title = "Find", Width = 360, Height = 140, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, ResizeMode = ResizeMode.NoResize };
            var panel = new StackPanel { Margin = new Thickness(12) };
            var tb = new TextBox { Height = 28, Margin = new Thickness(0, 0, 0, 12) };
            var btnFind = new Button { Content = "Find Next", Width = 90, Margin = new Thickness(0, 0, 8, 0), IsDefault = true };
            var btnCancel = new Button { Content = "Cancel", Width = 80, IsCancel = true };
            var row = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            btnFind.Click += (_, _) =>
            {
                string q = tb.Text;
                if (string.IsNullOrEmpty(q)) return;
                int start = MainEditor.SelectionStart + MainEditor.SelectionLength;
                int idx = MainEditor.Text.IndexOf(q, start, StringComparison.OrdinalIgnoreCase);
                if (idx < 0) idx = MainEditor.Text.IndexOf(q, 0, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0) { MainEditor.Select(idx, q.Length); MainEditor.TextArea.Focus(); }
                else MessageBox.Show("Not found.", "Find");
            };
            btnCancel.Click += (_, _) => dialog.Close();
            row.Children.Add(btnFind); row.Children.Add(btnCancel);
            panel.Children.Add(new TextBlock { Text = "Find:", Margin = new Thickness(0, 0, 0, 6) });
            panel.Children.Add(tb); panel.Children.Add(row);
            dialog.Content = panel; dialog.ShowDialog();
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window { Title = "Replace", Width = 380, Height = 200, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, ResizeMode = ResizeMode.NoResize };
            var panel = new StackPanel { Margin = new Thickness(12) };
            var findBox = new TextBox { Height = 28, Margin = new Thickness(0, 0, 0, 10) };
            var repBox = new TextBox { Height = 28, Margin = new Thickness(0, 0, 0, 12) };
            var btnAll = new Button { Content = "Replace All", Width = 100, Margin = new Thickness(0, 0, 8, 0), IsDefault = true };
            var btnCancel = new Button { Content = "Cancel", Width = 80, IsCancel = true };
            var row = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            btnAll.Click += (_, _) =>
            {
                string q = findBox.Text;
                if (string.IsNullOrEmpty(q)) return;
                string r = repBox.Text ?? "";
                int count = 0, i = 0;
                var sb = new StringBuilder();
                string text = MainEditor.Text;
                while (true)
                {
                    int f = text.IndexOf(q, i, StringComparison.OrdinalIgnoreCase);
                    if (f < 0) { sb.Append(text[i..]); break; }
                    sb.Append(text[i..f]); sb.Append(r);
                    i = f + q.Length; count++;
                }
                EditorText = sb.ToString();
                _isModified = true; UpdateTitle(); dialog.Close();
                MessageBox.Show($"Replaced {count} time(s).");
            };
            btnCancel.Click += (_, _) => dialog.Close();
            row.Children.Add(btnAll); row.Children.Add(btnCancel);
            panel.Children.Add(new TextBlock { Text = "Find:", Margin = new Thickness(0, 0, 0, 4) });
            panel.Children.Add(findBox);
            panel.Children.Add(new TextBlock { Text = "Replace:", Margin = new Thickness(0, 0, 0, 4) });
            panel.Children.Add(repBox); panel.Children.Add(row);
            dialog.Content = panel; dialog.ShowDialog();
        }

        private void WordWrap_Click(object sender, RoutedEventArgs e) => MainEditor.WordWrap = MenuWordWrap.IsChecked;
        private void LineNumbers_Click(object sender, RoutedEventArgs e) => MainEditor.ShowLineNumbers = MenuLineNumbers.IsChecked;
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

        private void JsonFormat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(MainEditor.Text);
                EditorText = System.Text.Json.JsonSerializer.Serialize(doc, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                _isModified = true; UpdateTitle();
            }
            catch { MessageBox.Show("Invalid JSON.", "JSON Format", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void YamlFormat_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _extraTools.FormatYamlLike(MainEditor.Text);
            _isModified = true; UpdateTitle();
        }

        private void FormatText_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _textFormatService.FormatPlainText(MainEditor.Text);
            _isModified = true; UpdateTitle();
        }

        private void ReflowText_Click(object sender, RoutedEventArgs e)
        {
            EditorText = _textFormatService.ReflowParagraphs(MainEditor.Text, 80);
            _isModified = true; UpdateTitle();
        }

        private void Slug_Click(object sender, RoutedEventArgs e)
        {
            string source = !string.IsNullOrEmpty(MainEditor.SelectedText) ? MainEditor.SelectedText : MainEditor.Text;
            string slug = _extraTools.ToSlug(source);
            if (MainEditor.SelectionLength > 0)
                MainEditor.Document.Replace(MainEditor.SelectionStart, MainEditor.SelectionLength, slug);
            else
                EditorText = slug;
            _isModified = true; UpdateTitle();
            MessageBox.Show($"Slug:\n{slug}", "Make Slug");
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var win = new SettingsWindow(_settingsService) { Owner = this };
            if (win.ShowDialog() == true) { LoadSettings(); UpdateStatusBar(); }
        }

        private void About_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show(
                "QuasarPad v1.5\n\nClear + multi-tab\nOffline Text & Markup Toolkit\n\nMIT License\nhttps://github.com/freedomania/QuasarPad",
                "About QuasarPad");

        private void OpenGitHub_Click(object sender, RoutedEventArgs e)
        {
            try { Process.Start(new ProcessStartInfo { FileName = "https://github.com/freedomania/QuasarPad", UseShellExecute = true }); }
            catch { MessageBox.Show("https://github.com/freedomania/QuasarPad"); }
        }

        private void Support_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Star the project on GitHub?\n\nOpen repository?", "Support", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                OpenGitHub_Click(sender, e);
        }

        private void Supporter_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show("Free for everyone. Starring on GitHub helps.\n\nhttps://github.com/freedomania/QuasarPad", "Thank you");
    }
}
