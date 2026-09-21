using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QuasarPad.Services
{
    public class ConverterService
    {
        public double TempToC(double v, string from) => from switch
        {
            "C" => v, "F" => (v - 32) * 5.0 / 9.0, "K" => v - 273.15, _ => v
        };

        public double TempFromC(double c, string to) => to switch
        {
            "C" => c, "F" => c * 9.0 / 5.0 + 32, "K" => c + 273.15, _ => c
        };

        private static readonly Dictionary<string, double> LengthToM = new()
        {
            ["mm"] = 0.001, ["cm"] = 0.01, ["m"] = 1, ["km"] = 1000,
            ["in"] = 0.0254, ["ft"] = 0.3048, ["yd"] = 0.9144, ["mi"] = 1609.344,
            ["nm"] = 1e-9, ["um"] = 1e-6, ["nmi"] = 1852
        };

        public double ConvertLength(double v, string from, string to)
        {
            if (!LengthToM.ContainsKey(from) || !LengthToM.ContainsKey(to)) return v;
            return v * LengthToM[from] / LengthToM[to];
        }

        private static readonly Dictionary<string, double> WeightToG = new()
        {
            ["mg"] = 0.001, ["g"] = 1, ["kg"] = 1000, ["oz"] = 28.349523125,
            ["lb"] = 453.59237, ["t"] = 1_000_000, ["st"] = 6350.29318, ["ct"] = 0.2
        };

        public double ConvertWeight(double v, string from, string to)
        {
            if (!WeightToG.ContainsKey(from) || !WeightToG.ContainsKey(to)) return v;
            return v * WeightToG[from] / WeightToG[to];
        }

        private static readonly Dictionary<string, double> DataToB = new()
        {
            ["B"] = 1, ["KB"] = 1024, ["MB"] = 1024d * 1024, ["GB"] = Math.Pow(1024, 3),
            ["TB"] = Math.Pow(1024, 4), ["PB"] = Math.Pow(1024, 5)
        };

        public double ConvertData(double v, string from, string to)
        {
            if (!DataToB.ContainsKey(from) || !DataToB.ContainsKey(to)) return v;
            return v * DataToB[from] / DataToB[to];
        }

        // Area -> m²
        private static readonly Dictionary<string, double> AreaToM2 = new()
        {
            ["mm2"] = 1e-6, ["cm2"] = 1e-4, ["m2"] = 1, ["km2"] = 1e6,
            ["in2"] = 0.00064516, ["ft2"] = 0.09290304, ["yd2"] = 0.83612736,
            ["acre"] = 4046.8564224, ["ha"] = 10000, ["rai"] = 1600
        };

        public double ConvertArea(double v, string from, string to)
        {
            if (!AreaToM2.ContainsKey(from) || !AreaToM2.ContainsKey(to)) return v;
            return v * AreaToM2[from] / AreaToM2[to];
        }

        // Volume -> liters
        private static readonly Dictionary<string, double> VolToL = new()
        {
            ["ml"] = 0.001, ["L"] = 1, ["m3"] = 1000,
            ["tsp"] = 0.00492892, ["tbsp"] = 0.0147868, ["cup"] = 0.236588,
            ["pt"] = 0.473176, ["qt"] = 0.946353, ["gal"] = 3.78541
        };

        public double ConvertVolume(double v, string from, string to)
        {
            if (!VolToL.ContainsKey(from) || !VolToL.ContainsKey(to)) return v;
            return v * VolToL[from] / VolToL[to];
        }

        // Speed -> m/s
        private static readonly Dictionary<string, double> SpeedToMs = new()
        {
            ["m/s"] = 1, ["km/h"] = 1000.0 / 3600.0, ["mph"] = 0.44704,
            ["knot"] = 0.514444, ["ft/s"] = 0.3048
        };

        public double ConvertSpeed(double v, string from, string to)
        {
            if (!SpeedToMs.ContainsKey(from) || !SpeedToMs.ContainsKey(to)) return v;
            return v * SpeedToMs[from] / SpeedToMs[to];
        }

        // Angle
        public double ConvertAngle(double v, string from, string to)
        {
            double deg = from switch { "rad" => v * 180.0 / Math.PI, "grad" => v * 0.9, _ => v };
            return to switch { "rad" => deg * Math.PI / 180.0, "grad" => deg / 0.9, _ => deg };
        }

        // Time -> seconds
        private static readonly Dictionary<string, double> TimeToS = new()
        {
            ["ms"] = 0.001, ["s"] = 1, ["min"] = 60, ["h"] = 3600,
            ["d"] = 86400, ["wk"] = 604800
        };

        public double ConvertTime(double v, string from, string to)
        {
            if (!TimeToS.ContainsKey(from) || !TimeToS.ContainsKey(to)) return v;
            return v * TimeToS[from] / TimeToS[to];
        }

        public string ConvertBase(string input, int fromBase, int toBase)
        {
            input = input.Trim().Replace(" ", "");
            if (string.IsNullOrEmpty(input)) return "";
            try
            {
                long value = Convert.ToInt64(input, fromBase);
                return Convert.ToString(value, toBase).ToUpperInvariant();
            }
            catch { return "[Invalid number]"; }
        }

        public (byte r, byte g, byte b, string? error) HexToRgb(string hex)
        {
            hex = hex.Trim().TrimStart('#');
            if (hex.Length == 3)
                hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
            if (hex.Length != 6 || !Regex.IsMatch(hex, "^[0-9A-Fa-f]{6}$"))
                return (0, 0, 0, "[Invalid HEX]");
            return (
                byte.Parse(hex[..2], NumberStyles.HexNumber),
                byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                null);
        }

        public string RgbToHex(int r, int g, int b)
        {
            r = Math.Clamp(r, 0, 255); g = Math.Clamp(g, 0, 255); b = Math.Clamp(b, 0, 255);
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        public static readonly (string name, string hex)[] ColorPalette =
        {
            ("Black", "#000000"), ("White", "#FFFFFF"), ("Red", "#FF0000"), ("Lime", "#00FF00"),
            ("Blue", "#0000FF"), ("Yellow", "#FFFF00"), ("Cyan", "#00FFFF"), ("Magenta", "#FF00FF"),
            ("Orange", "#FFA500"), ("Purple", "#800080"), ("Pink", "#FFC0CB"), ("Brown", "#8B4513"),
            ("Gray", "#808080"), ("Navy", "#000080"), ("Teal", "#008080"), ("Olive", "#808000"),
            ("Maroon", "#800000"), ("Silver", "#C0C0C0"), ("Gold", "#FFD700"), ("Coral", "#FF7F50"),
            ("Tomato", "#FF6347"), ("OrangeRed", "#FF4500"), ("HotPink", "#FF69B4"), ("DeepPink", "#FF1493"),
            ("Violet", "#EE82EE"), ("Indigo", "#4B0082"), ("DodgerBlue", "#1E90FF"), ("SkyBlue", "#87CEEB"),
            ("Turquoise", "#40E0D0"), ("SpringGreen", "#00FF7F"), ("ForestGreen", "#228B22"), ("Khaki", "#F0E68C"),
            ("Chocolate", "#D2691E"), ("Sienna", "#A0522D"), ("SlateGray", "#708090"), ("DimGray", "#696969"),
            ("VS Blue", "#007ACC"), ("GitHub Dark", "#0D1117"), ("Soft Red", "#E74C3C"), ("Soft Green", "#2ECC71"),
            ("Soft Blue", "#3498DB"), ("Amethyst", "#9B59B6"), ("Carrot", "#E67E22"), ("Sun", "#F1C40F"),
            ("Wet Asphalt", "#34495E"), ("Concrete", "#95A5A6"), ("Asbestos", "#7F8C8D"), ("Clouds", "#ECF0F1")
        };

        public string NewGuid() => Guid.NewGuid().ToString();
        public string NewGuidNoDash() => Guid.NewGuid().ToString("N");

        public string GeneratePassword(int length, bool upper, bool lower, bool digits, bool symbols)
        {
            if (length < 4) length = 4;
            if (length > 128) length = 128;
            var sets = new List<string>();
            if (upper) sets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            if (lower) sets.Add("abcdefghijklmnopqrstuvwxyz");
            if (digits) sets.Add("0123456789");
            if (symbols) sets.Add("!@#$%^&*()-_=+[]{}<>?");
            if (sets.Count == 0) sets.Add("abcdefghijklmnopqrstuvwxyz");
            var all = string.Concat(sets);
            var sb = new StringBuilder(length);
            foreach (var set in sets)
                sb.Append(set[RandomNumberGenerator.GetInt32(set.Length)]);
            while (sb.Length < length)
                sb.Append(all[RandomNumberGenerator.GetInt32(all.Length)]);
            return new string(sb.ToString().OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
        }

        public string SortLines(string text, bool descending)
        {
            var lines = text.Replace("\r\n", "\n").Split('\n').ToList();
            lines.Sort(StringComparer.Ordinal);
            if (descending) lines.Reverse();
            return string.Join(Environment.NewLine, lines);
        }

        public string UniqueLines(string text)
        {
            var lines = text.Replace("\r\n", "\n").Split('\n');
            var seen = new HashSet<string>();
            var result = new List<string>();
            foreach (var line in lines)
                if (seen.Add(line)) result.Add(line);
            return string.Join(Environment.NewLine, result);
        }

        public string SortUnique(string text, bool descending) => SortLines(UniqueLines(text), descending);

        /// <summary>Shorten URL via TinyURL public API (requires internet).</summary>
        public async Task<string> ShortenUrlAsync(string url)
        {
            url = url.Trim();
            if (string.IsNullOrEmpty(url)) return "[Empty URL]";
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(12) };
                string api = "https://tinyurl.com/api-create.php?url=" + Uri.EscapeDataString(url);
                string result = await client.GetStringAsync(api);
                if (string.IsNullOrWhiteSpace(result) || result.StartsWith("Error", StringComparison.OrdinalIgnoreCase))
                    return "[Shorten failed]";
                return result.Trim();
            }
            catch (Exception ex)
            {
                return $"[Need internet / error: {ex.Message}]";
            }
        }
    }
}
