using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class UnitConverterWindow : Window
    {
        private readonly ConverterService _svc = new();

        public UnitConverterWindow()
        {
            InitializeComponent();
            LoadUnits();
        }

        private void Category_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (CmbFrom == null) return;
            LoadUnits();
        }

        private void LoadUnits()
        {
            CmbFrom.Items.Clear();
            CmbTo.Items.Clear();
            string cat = (CmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Temperature";
            string[] units = cat switch
            {
                "Length" => new[] { "mm", "cm", "m", "km", "in", "ft", "yd", "mi" },
                "Weight" => new[] { "mg", "g", "kg", "oz", "lb", "t" },
                "Data size" => new[] { "B", "KB", "MB", "GB", "TB" },
                _ => new[] { "C", "F", "K" }
            };
            foreach (var u in units)
            {
                CmbFrom.Items.Add(u);
                CmbTo.Items.Add(u);
            }
            CmbFrom.SelectedIndex = 0;
            CmbTo.SelectedIndex = Math.Min(1, units.Length - 1);
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(TxtValue.Text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
            {
                TxtResult.Text = "[Invalid number]";
                return;
            }
            string from = CmbFrom.SelectedItem?.ToString() ?? "";
            string to = CmbTo.SelectedItem?.ToString() ?? "";
            string cat = (CmbCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            double result = cat switch
            {
                "Length" => _svc.ConvertLength(val, from, to),
                "Weight" => _svc.ConvertWeight(val, from, to),
                "Data size" => _svc.ConvertData(val, from, to),
                _ => _svc.TempFromC(_svc.TempToC(val, from), to)
            };
            TxtResult.Text = result.ToString("G10", CultureInfo.InvariantCulture);
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtResult.Text))
                Clipboard.SetText(TxtResult.Text);
        }
    }
}
