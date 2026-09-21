using System.Windows;
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
            HexToRgb_Click(this, new RoutedEventArgs());
        }

        private void HexToRgb_Click(object sender, RoutedEventArgs e)
        {
            var (r, g, b, err) = _svc.HexToRgb(TxtHex.Text);
            if (err != null) { MessageBox.Show(err); return; }
            TxtR.Text = r.ToString();
            TxtG.Text = g.ToString();
            TxtB.Text = b.ToString();
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
            Preview.Background = new SolidColorBrush(Color.FromRgb((byte)Math.Clamp(r, 0, 255), (byte)Math.Clamp(g, 0, 255), (byte)Math.Clamp(b, 0, 255)));
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtHex.Text))
                Clipboard.SetText(TxtHex.Text);
        }
    }
}
