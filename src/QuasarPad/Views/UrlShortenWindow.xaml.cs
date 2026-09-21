using System.Windows;
using QuasarPad.Services;

namespace QuasarPad.Views
{
    public partial class UrlShortenWindow : Window
    {
        private readonly ConverterService _svc = new();

        public UrlShortenWindow(string? initial = null)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(initial))
                TxtUrl.Text = initial.Trim();
        }

        private async void Shorten_Click(object sender, RoutedEventArgs e)
        {
            BtnShorten.IsEnabled = false;
            TxtShort.Text = "Working...";
            try
            {
                TxtShort.Text = await _svc.ShortenUrlAsync(TxtUrl.Text);
            }
            finally
            {
                BtnShorten.IsEnabled = true;
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtShort.Text) && !TxtShort.Text.StartsWith("["))
                Clipboard.SetText(TxtShort.Text);
        }
    }
}
