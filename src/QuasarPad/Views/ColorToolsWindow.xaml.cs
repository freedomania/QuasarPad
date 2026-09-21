using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class ColorToolsWindow : Window
    {
        private readonly ConverterService _svc = new();

        public ColorToolsWindow()
        {
            InitializeComponent();
            BuildPalette();
            HexToRgb_Click(this, new RoutedEventArgs());
        }

        private void BuildPalette()
        {
            foreach (var (name, hex) in ConverterService.ColorPalette)
            {
                var (r, g, b, err) = _svc.HexToRgb(hex);
                if (err != null) continue;

                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(r, g, b)),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Margin = new Thickness(3),
                    Height = 36,
                    Cursor = Cursors.Hand,
                    ToolTip = $"{name}  {hex}"
                };
                border.MouseLeftButtonUp += (_, _) => SelectColor(name, hex, r, g, b);
                PaletteGrid.Children.Add(border);
            }
        }

        private void SelectColor(string name, string hex, byte r, byte g, byte b)
        {
            TxtHex.Text = hex;
            TxtR.Text = r.ToString();
            TxtG.Text = g.ToString();
            TxtB.Text = b.ToString();
            TxtName.Text = $"{name}  |  {hex}  |  rgb({r}, {g}, {b})";
            Preview.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void HexToRgb_Click(object sender, RoutedEventArgs e)
        {
            var (r, g, b, err) = _svc.HexToRgb(TxtHex.Text);
            if (err != null) { MessageBox.Show(err); return; }
            TxtR.Text = r.ToString();
            TxtG.Text = g.ToString();
            TxtB.Text = b.ToString();
            TxtName.Text = $"{TxtHex.Text}  |  rgb({r}, {g}, {b})";
            Preview.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void RgbToHex_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(TxtR.Text, out int r) || !int.TryParse(TxtG.Text, out int g) || !int.TryParse(TxtB.Text, out int b))
            {
                MessageBox.Show("Invalid RGB values.");
                return;
            }
            TxtHex.Text = _svc.RgbToHex(r, g, b);
            byte rb = (byte)Math.Clamp(r, 0, 255), gb = (byte)Math.Clamp(g, 0, 255), bb = (byte)Math.Clamp(b, 0, 255);
            TxtName.Text = $"{TxtHex.Text}  |  rgb({rb}, {gb}, {bb})";
            Preview.Background = new SolidColorBrush(Color.FromRgb(rb, gb, bb));
        }

        private void CopyHex_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtHex.Text)) Clipboard.SetText(TxtHex.Text);
        }

        private void CopyRgb_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"rgb({TxtR.Text}, {TxtG.Text}, {TxtB.Text})");
        }
    }
}
