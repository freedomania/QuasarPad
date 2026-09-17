using System.Security.Cryptography;
using System.Text;

namespace QuasarPad.Services
{
    public class TextToolsService
    {
        public string ToBase64(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public string FromBase64(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            try
            {
                byte[] bytes = Convert.FromBase64String(input.Trim());
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return "[Invalid Base64]";
            }
        }

        public string UrlEncode(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Uri.EscapeDataString(input);
        }

        public string UrlDecode(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Uri.UnescapeDataString(input);
        }

        public string ToUpper(string input) => input?.ToUpperInvariant() ?? string.Empty;
        public string ToLower(string input) => input?.ToLowerInvariant() ?? string.Empty;

        public string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLowerInvariant());
        }

        public string Md5(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public string Sha256(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        public string Reverse(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            char[] chars = input.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        public string RemoveEmptyLines(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var lines = input.Replace("\r\n", "\n").Split('\n')
                .Where(l => !string.IsNullOrWhiteSpace(l));
            return string.Join(Environment.NewLine, lines) + Environment.NewLine;
        }
    }
}
