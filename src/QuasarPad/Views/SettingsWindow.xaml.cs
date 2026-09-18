using System.Windows;
using System.Windows.Controls;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsService _settingsService;
        public bool Saved { get; private set; }

        public SettingsWindow(SettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;
            LoadFromSettings();
        }

        private void LoadFromSettings()
        {
            var s = _settingsService.Settings;
            ChkDarkMode.IsChecked = s.IsDarkMode;
            ChkPureMode.IsChecked = s.IsPureMode;
            ChkWordWrap.IsChecked = s.WordWrap;
            ChkStatusBar.IsChecked = s.ShowStatusBar;
            SliderFontSize.Value = s.FontSize;
            TxtFontSize.Text = ((int)s.FontSize).ToString();

            foreach (ComboBoxItem item in CmbFont.Items)
            {
                if (item.Content?.ToString() == s.FontFamily)
                {
                    CmbFont.SelectedItem = item;
                    break;
                }
            }
            if (CmbFont.SelectedItem == null && CmbFont.Items.Count > 0)
                CmbFont.SelectedIndex = 0;
        }

        private void SliderFontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtFontSize != null)
                TxtFontSize.Text = ((int)SliderFontSize.Value).ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var s = _settingsService.Settings;
            s.IsDarkMode = ChkDarkMode.IsChecked == true;
            s.IsPureMode = ChkPureMode.IsChecked == true;
            s.WordWrap = ChkWordWrap.IsChecked == true;
            s.ShowStatusBar = ChkStatusBar.IsChecked == true;
            s.FontSize = SliderFontSize.Value;

            if (CmbFont.SelectedItem is ComboBoxItem fontItem)
                s.FontFamily = fontItem.Content?.ToString() ?? "Consolas";

            _settingsService.Save();
            Saved = true;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
