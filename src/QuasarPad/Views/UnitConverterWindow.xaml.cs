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
                "Length" => new[] { "pm", "nm", "um", "mm", "cm", "dm", "m", "km", "in", "ft", "yd", "mi", "nmi", "au", "ly" },
                "Weight" => new[] { "ug", "mg", "g", "kg", "t", "oz", "lb", "st", "ct", "gr", "slug" },
                "Data size" => new[] { "bit", "B", "KB", "MB", "GB", "TB", "PB", "KiB", "MiB", "GiB" },
                "Area" => new[] { "mm2", "cm2", "m2", "km2", "in2", "ft2", "yd2", "acre", "ha", "rai", "ngan", "wa2" },
                "Volume" => new[] { "ml", "cl", "dl", "L", "m3", "tsp", "tbsp", "cup", "pt", "qt", "gal", "fl_oz", "in3", "ft3" },
                "Speed" => new[] { "m/s", "km/h", "mph", "knot", "ft/s", "mach", "c" },
                "Angle" => new[] { "deg", "rad", "grad", "arcmin", "arcsec" },
                "Time" => new[] { "ns", "us", "ms", "s", "min", "h", "d", "wk", "mo", "yr" },
                "Pressure" => new[] { "Pa", "kPa", "MPa", "bar", "atm", "psi", "mmHg", "Torr" },
                "Energy" => new[] { "J", "kJ", "cal", "kcal", "Wh", "kWh", "eV", "BTU" },
                "Power" => new[] { "W", "kW", "MW", "hp", "BTU/h" },
                _ => new[] { "C", "F", "K", "R" }
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
                "Area" => _svc.ConvertArea(val, from, to),
                "Volume" => _svc.ConvertVolume(val, from, to),
                "Speed" => _svc.ConvertSpeed(val, from, to),
                "Angle" => _svc.ConvertAngle(val, from, to),
                "Time" => _svc.ConvertTime(val, from, to),
                "Pressure" => _svc.ConvertPressure(val, from, to),
                "Energy" => _svc.ConvertEnergy(val, from, to),
                "Power" => _svc.ConvertPower(val, from, to),
                _ => _svc.TempFromC(_svc.TempToC(val, from), to)
            };
            TxtResult.Text = result.ToString("G12", CultureInfo.InvariantCulture);
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtResult.Text))
                Clipboard.SetText(TxtResult.Text);
        }
    }
}
