using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QuasarPad.Services
{
    public class ConverterService
    {
        // --- Temperature: store as Celsius ---
        public double TempToC(double v, string from) => from switch
        {
            "C" => v,
            "F" => (v - 32) * 5.0 / 9.0,
            "K" => v - 273.15,
            _ => v
        };

        public double TempFromC(double c, string to) => to switch
        {
            "C" => c,
            "F" => c * 9.0 / 5.0 + 32,
            "K" => c + 273.15,
            _ => c
        };

        // --- Length: store as meters ---
        private static readonly Dictionary<string, double> LengthToM = new()
        {
            ["mm"] = 0.001, ["cm"] = 0.01, ["m"] = 1, ["km"] = 1000,
            ["in"] = 0.0254, ["ft"] = 0.3048, ["yd"] = 0.9144, ["mi"] = 1609.344
        };

        public double ConvertLength(double v, string from, string to)
        {
            if (!LengthToM.ContainsKey(from) || !LengthToM.ContainsKey(to)) return v;
            return v * LengthToM[from] / LengthToM[to];
        }

        // --- Weight: store as grams ---
        private static readonly Dictionary<string, double> WeightToG = new()
        {
            ["mg"] = 0.001, ["g"] = 1, ["kg"] = 1000,
            ["oz"] = 28.349523125, ["lb"] = 453.59237, ["t"] = 1_000_000
        };

        public double ConvertWeight(double v, string from, string to)
        {
            if (!WeightToG.ContainsKey(from) || !WeightToG.ContainsKey(to)) return v;
            return v * WeightToG[from] / WeightToG[to];
        }

        // --- Data size: store as bytes (binary 1024) ---
        private static readonly Dictionary<string, double> DataToB = new()
        {
            ["B"] = 1, ["KB"] = 1024, ["MB"] = 1024 * 1024,
            ["GB"] = Math.Pow(1024, 3), ["TB"] = Math.Pow(1024, 4)
        };

        public double ConvertData(double v, string from, string to)
        {
            if (!DataToB.ContainsKey(from) || !DataToB.ContainsKey(to)) return v;
            return v * DataToB[from] / DataToB[to];
        }

        // --- Number bases ---
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

        // --- Color ---
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
            r = Math.Clamp(r, 0, 255);
            g = Math.Clamp(g, 0, 255);
            b = Math.Clamp(b, 0, 255);
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        // --- UUID ---
        public string NewGuid() => Guid.NewGuid().ToString();
        public string NewGuidNoDash() => Guid.NewGuid().ToString("N");

        // --- Password ---
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
            // ensure at least one from each selected set
            foreach (var set in sets)
                sb.Append(set[RandomNumberGenerator.GetInt32(set.Length)]);
            while (sb.Length < length)
                sb.Append(all[RandomNumberGenerator.GetInt32(all.Length)]);

            // shuffle
            return new string(sb.ToString().OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
        }

        // --- Sort / Unique lines ---
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
            {
                if (seen.Add(line))
                    result.Add(line);
            }
            return string.Join(Environment.NewLine, result);
        }

        public string SortUnique(string text, bool descending)
        {
            return SortLines(UniqueLines(text), descending);
        }
    }
}
