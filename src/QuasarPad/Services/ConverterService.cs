using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace QuasarPad.Services
{
    public class ConverterService
    {
        public double TempToC(double v, string from) => from switch
        {
            "C" => v, "F" => (v - 32) * 5.0 / 9.0, "K" => v - 273.15, "R" => (v - 491.67) * 5.0 / 9.0, _ => v
        };

        public double TempFromC(double c, string to) => to switch
        {
            "C" => c, "F" => c * 9.0 / 5.0 + 32, "K" => c + 273.15, "R" => c * 9.0 / 5.0 + 491.67, _ => c
        };

        private static readonly Dictionary<string, double> LengthToM = new()
        {
            ["pm"] = 1e-12, ["nm"] = 1e-9, ["um"] = 1e-6, ["mm"] = 0.001, ["cm"] = 0.01,
            ["dm"] = 0.1, ["m"] = 1, ["dam"] = 10, ["hm"] = 100, ["km"] = 1000,
            ["in"] = 0.0254, ["ft"] = 0.3048, ["yd"] = 0.9144, ["mi"] = 1609.344,
            ["nmi"] = 1852, ["au"] = 1.495978707e11, ["ly"] = 9.4607304725808e15
        };

        public double ConvertLength(double v, string from, string to) =>
            !LengthToM.ContainsKey(from) || !LengthToM.ContainsKey(to) ? v : v * LengthToM[from] / LengthToM[to];

        private static readonly Dictionary<string, double> WeightToG = new()
        {
            ["ug"] = 1e-6, ["mg"] = 0.001, ["g"] = 1, ["kg"] = 1000, ["t"] = 1e6,
            ["oz"] = 28.349523125, ["lb"] = 453.59237, ["st"] = 6350.29318,
            ["ct"] = 0.2, ["gr"] = 0.06479891, ["slug"] = 14593.903
        };

        public double ConvertWeight(double v, string from, string to) =>
            !WeightToG.ContainsKey(from) || !WeightToG.ContainsKey(to) ? v : v * WeightToG[from] / WeightToG[to];

        private static readonly Dictionary<string, double> DataToB = new()
        {
            ["bit"] = 0.125, ["B"] = 1, ["KB"] = 1024, ["MB"] = 1024d * 1024,
            ["GB"] = Math.Pow(1024, 3), ["TB"] = Math.Pow(1024, 4), ["PB"] = Math.Pow(1024, 5),
            ["KiB"] = 1024, ["MiB"] = 1024d * 1024, ["GiB"] = Math.Pow(1024, 3)
        };

        public double ConvertData(double v, string from, string to) =>
            !DataToB.ContainsKey(from) || !DataToB.ContainsKey(to) ? v : v * DataToB[from] / DataToB[to];

        private static readonly Dictionary<string, double> AreaToM2 = new()
        {
            ["mm2"] = 1e-6, ["cm2"] = 1e-4, ["m2"] = 1, ["km2"] = 1e6,
            ["in2"] = 0.00064516, ["ft2"] = 0.09290304, ["yd2"] = 0.83612736,
            ["acre"] = 4046.8564224, ["ha"] = 10000, ["rai"] = 1600, ["ngan"] = 400, ["wa2"] = 4
        };

        public double ConvertArea(double v, string from, string to) =>
            !AreaToM2.ContainsKey(from) || !AreaToM2.ContainsKey(to) ? v : v * AreaToM2[from] / AreaToM2[to];

        private static readonly Dictionary<string, double> VolToL = new()
        {
            ["ml"] = 0.001, ["cl"] = 0.01, ["dl"] = 0.1, ["L"] = 1, ["m3"] = 1000,
            ["tsp"] = 0.00492892, ["tbsp"] = 0.0147868, ["cup"] = 0.236588,
            ["pt"] = 0.473176, ["qt"] = 0.946353, ["gal"] = 3.78541,
            ["fl_oz"] = 0.0295735, ["in3"] = 0.0163871, ["ft3"] = 28.3168
        };

        public double ConvertVolume(double v, string from, string to) =>
            !VolToL.ContainsKey(from) || !VolToL.ContainsKey(to) ? v : v * VolToL[from] / VolToL[to];

        private static readonly Dictionary<string, double> SpeedToMs = new()
        {
            ["m/s"] = 1, ["km/h"] = 1000.0 / 3600.0, ["mph"] = 0.44704,
            ["knot"] = 0.514444, ["ft/s"] = 0.3048, ["mach"] = 340.29, ["c"] = 299792458
        };

        public double ConvertSpeed(double v, string from, string to) =>
            !SpeedToMs.ContainsKey(from) || !SpeedToMs.ContainsKey(to) ? v : v * SpeedToMs[from] / SpeedToMs[to];

        public double ConvertAngle(double v, string from, string to)
        {
            double deg = from switch { "rad" => v * 180.0 / Math.PI, "grad" => v * 0.9, "arcmin" => v / 60.0, "arcsec" => v / 3600.0, _ => v };
            return to switch { "rad" => deg * Math.PI / 180.0, "grad" => deg / 0.9, "arcmin" => deg * 60.0, "arcsec" => deg * 3600.0, _ => deg };
        }

        private static readonly Dictionary<string, double> TimeToS = new()
        {
            ["ns"] = 1e-9, ["us"] = 1e-6, ["ms"] = 0.001, ["s"] = 1, ["min"] = 60,
            ["h"] = 3600, ["d"] = 86400, ["wk"] = 604800, ["mo"] = 2629800, ["yr"] = 31557600
        };

        public double ConvertTime(double v, string from, string to) =>
            !TimeToS.ContainsKey(from) || !TimeToS.ContainsKey(to) ? v : v * TimeToS[from] / TimeToS[to];

        // Pressure -> Pascal
        private static readonly Dictionary<string, double> PressureToPa = new()
        {
            ["Pa"] = 1, ["kPa"] = 1000, ["MPa"] = 1e6, ["bar"] = 1e5,
            ["atm"] = 101325, ["psi"] = 6894.76, ["mmHg"] = 133.322, ["Torr"] = 133.322
        };

        public double ConvertPressure(double v, string from, string to) =>
            !PressureToPa.ContainsKey(from) || !PressureToPa.ContainsKey(to) ? v : v * PressureToPa[from] / PressureToPa[to];

        // Energy -> Joule
        private static readonly Dictionary<string, double> EnergyToJ = new()
        {
            ["J"] = 1, ["kJ"] = 1000, ["cal"] = 4.184, ["kcal"] = 4184,
            ["Wh"] = 3600, ["kWh"] = 3.6e6, ["eV"] = 1.602176634e-19, ["BTU"] = 1055.06
        };

        public double ConvertEnergy(double v, string from, string to) =>
            !EnergyToJ.ContainsKey(from) || !EnergyToJ.ContainsKey(to) ? v : v * EnergyToJ[from] / EnergyToJ[to];

        // Power -> Watt
        private static readonly Dictionary<string, double> PowerToW = new()
        {
            ["W"] = 1, ["kW"] = 1000, ["MW"] = 1e6, ["hp"] = 745.7, ["BTU/h"] = 0.293071
        };

        public double ConvertPower(double v, string from, string to) =>
            !PowerToW.ContainsKey(from) || !PowerToW.ContainsKey(to) ? v : v * PowerToW[from] / PowerToW[to];

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

        // ~120 offline named colors
        public static readonly (string name, string hex)[] ColorPalette =
        {
            ("Black", "#000000"), ("White", "#FFFFFF"), ("Red", "#FF0000"), ("Lime", "#00FF00"),
            ("Blue", "#0000FF"), ("Yellow", "#FFFF00"), ("Cyan", "#00FFFF"), ("Magenta", "#FF00FF"),
            ("Silver", "#C0C0C0"), ("Gray", "#808080"), ("Maroon", "#800000"), ("Olive", "#808000"),
            ("Green", "#008000"), ("Purple", "#800080"), ("Teal", "#008080"), ("Navy", "#000080"),
            ("Orange", "#FFA500"), ("Pink", "#FFC0CB"), ("Brown", "#A52A2A"), ("Gold", "#FFD700"),
            ("Coral", "#FF7F50"), ("Tomato", "#FF6347"), ("OrangeRed", "#FF4500"), ("HotPink", "#FF69B4"),
            ("DeepPink", "#FF1493"), ("LightPink", "#FFB6C1"), ("Salmon", "#FA8072"), ("Crimson", "#DC143C"),
            ("FireBrick", "#B22222"), ("DarkRed", "#8B0000"), ("IndianRed", "#CD5C5C"), ("LightCoral", "#F08080"),
            ("Violet", "#EE82EE"), ("Orchid", "#DA70D6"), ("Plum", "#DDA0DD"), ("Thistle", "#D8BFD8"),
            ("Magenta2", "#FF00FF"), ("MediumOrchid", "#BA55D3"), ("MediumPurple", "#9370DB"), ("BlueViolet", "#8A2BE2"),
            ("DarkViolet", "#9400D3"), ("DarkOrchid", "#9932CC"), ("DarkMagenta", "#8B008B"), ("Indigo", "#4B0082"),
            ("SlateBlue", "#6A5ACD"), ("DarkSlateBlue", "#483D8B"), ("MediumSlateBlue", "#7B68EE"), ("Lavender", "#E6E6FA"),
            ("DodgerBlue", "#1E90FF"), ("SkyBlue", "#87CEEB"), ("LightSkyBlue", "#87CEFA"), ("DeepSkyBlue", "#00BFFF"),
            ("LightBlue", "#ADD8E6"), ("PowderBlue", "#B0E0E6"), ("CadetBlue", "#5F9EA0"), ("SteelBlue", "#4682B4"),
            ("CornflowerBlue", "#6495ED"), ("RoyalBlue", "#4169E1"), ("MediumBlue", "#0000CD"), ("DarkBlue", "#00008B"),
            ("MidnightBlue", "#191970"), ("AliceBlue", "#F0F8FF"), ("Azure", "#F0FFFF"), ("Aqua", "#00FFFF"),
            ("Turquoise", "#40E0D0"), ("MediumTurquoise", "#48D1CC"), ("DarkTurquoise", "#00CED1"), ("Aquamarine", "#7FFFD4"),
            ("PaleTurquoise", "#AFEEEE"), ("LightSeaGreen", "#20B2AA"), ("DarkCyan", "#008B8B"), ("SpringGreen", "#00FF7F"),
            ("MediumSpringGreen", "#00FA9A"), ("LimeGreen", "#32CD32"), ("ForestGreen", "#228B22"), ("SeaGreen", "#2E8B57"),
            ("MediumSeaGreen", "#3CB371"), ("DarkSeaGreen", "#8FBC8F"), ("LightGreen", "#90EE90"), ("PaleGreen", "#98FB98"),
            ("YellowGreen", "#9ACD32"), ("GreenYellow", "#ADFF2F"), ("Chartreuse", "#7FFF00"), ("LawnGreen", "#7CFC00"),
            ("DarkOliveGreen", "#556B2F"), ("OliveDrab", "#6B8E23"), ("DarkGreen", "#006400"), ("Khaki", "#F0E68C"),
            ("DarkKhaki", "#BDB76B"), ("PaleGoldenrod", "#EEE8AA"), ("LightYellow", "#FFFFE0"), ("LemonChiffon", "#FFFACD"),
            ("PapayaWhip", "#FFEFD5"), ("Moccasin", "#FFE4B5"), ("PeachPuff", "#FFDAB9"), ("PaleGolden", "#EEE8AA"),
            ("Wheat", "#F5DEB3"), ("Burlywood", "#DEB887"), ("Tan", "#D2B48C"), ("SandyBrown", "#F4A460"),
            ("Goldenrod", "#DAA520"), ("DarkGoldenrod", "#B8860B"), ("Peru", "#CD853F"), ("Chocolate", "#D2691E"),
            ("SaddleBrown", "#8B4513"), ("Sienna", "#A0522D"), ("RosyBrown", "#BC8F8F"), ("DarkSalmon", "#E9967A"),
            ("LightSalmon", "#FFA07A"), ("NavajoWhite", "#FFDEAD"), ("Bisque", "#FFE4C4"), ("BlanchedAlmond", "#FFEBCD"),
            ("Cornsilk", "#FFF8DC"), ("FloralWhite", "#FFFAF0"), ("Ivory", "#FFFFF0"), ("Beige", "#F5F5DC"),
            ("Snow", "#FFFAFA"), ("Honeydew", "#F0FFF0"), ("MintCream", "#F5FFFA"), ("GhostWhite", "#F8F8FF"),
            ("WhiteSmoke", "#F5F5F5"), ("Gainsboro", "#DCDCDC"), ("LightGray", "#D3D3D3"), ("DarkGray", "#A9A9A9"),
            ("DimGray", "#696969"), ("LightSlateGray", "#778899"), ("SlateGray", "#708090"), ("DarkSlateGray", "#2F4F4F"),
            ("VS Blue", "#007ACC"), ("GitHub Dark", "#0D1117"), ("Soft Red", "#E74C3C"), ("Soft Green", "#2ECC71"),
            ("Soft Blue", "#3498DB"), ("Amethyst", "#9B59B6"), ("Carrot", "#E67E22"), ("Sun Flower", "#F1C40F"),
            ("Wet Asphalt", "#34495E"), ("Concrete", "#95A5A6"), ("Asbestos", "#7F8C8D"), ("Clouds", "#ECF0F1"),
            ("Emerald", "#27AE60"), ("Peter River", "#2980B9"), ("Wisteria", "#8E44AD"), ("Alizarin", "#E74C3C"),
            ("Turquoise Flat", "#1ABC9C"), ("Green Sea", "#16A085"), ("Nephritis", "#27AE60"), ("Belize Hole", "#2980B9"),
            ("Midnight", "#2C3E50"), ("Asbestos2", "#7F8C8D"), ("Silver Flat", "#BDC3C7"), ("Pomegranate", "#C0392B")
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
    }
}
