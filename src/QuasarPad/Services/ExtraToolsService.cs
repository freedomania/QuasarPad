using System.Text;
using System.Text.RegularExpressions;

namespace QuasarPad.Services
{
    public class ExtraToolsService
    {
        public string ToSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            string s = input.Trim().ToLowerInvariant();
            s = Regex.Replace(s, @"[^\w\s-]", "");
            s = Regex.Replace(s, @"[\s_-]+", "-");
            s = s.Trim('-');
            return s;
        }

        public string FormatYamlLike(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            // Lightweight indentation cleanup for YAML-like text (not a full parser)
            var lines = input.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                string t = line.TrimEnd();
                if (string.IsNullOrWhiteSpace(t))
                {
                    sb.AppendLine();
                    continue;
                }
                // collapse multiple spaces after key:
                t = Regex.Replace(t, @":\s+", ": ");
                sb.AppendLine(t);
            }
            return sb.ToString().TrimEnd() + "\n";
        }

        public string UnixToLocal(string input)
        {
            if (!long.TryParse(input.Trim(), out long unix))
                return "[Invalid Unix timestamp]";
            try
            {
                // support seconds or milliseconds
                DateTimeOffset dto = unix > 9_999_999_999
                    ? DateTimeOffset.FromUnixTimeMilliseconds(unix)
                    : DateTimeOffset.FromUnixTimeSeconds(unix);
                return dto.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz");
            }
            catch { return "[Invalid Unix timestamp]"; }
        }

        public string LocalToUnix(string input)
        {
            if (DateTime.TryParse(input.Trim(), out DateTime dt))
            {
                var dto = new DateTimeOffset(dt);
                return dto.ToUnixTimeSeconds().ToString();
            }
            return "[Invalid date/time]";
        }

        public string NowUnix() => DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

        public string NowIso() => DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");

        public (string result, int matchCount) TestRegex(string pattern, string input, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(pattern))
                return ("[Empty pattern]", 0);
            try
            {
                var options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
                var matches = Regex.Matches(input ?? "", pattern, options);
                if (matches.Count == 0)
                    return ("No matches.", 0);

                var sb = new StringBuilder();
                sb.AppendLine($"Matches: {matches.Count}");
                sb.AppendLine();
                int i = 1;
                foreach (Match m in matches)
                {
                    sb.AppendLine($"[{i}] Index {m.Index}, Length {m.Length}");
                    sb.AppendLine($"    {m.Value}");
                    if (m.Groups.Count > 1)
                    {
                        for (int g = 1; g < m.Groups.Count; g++)
                            sb.AppendLine($"    Group {g}: {m.Groups[g].Value}");
                    }
                    sb.AppendLine();
                    i++;
                }
                return (sb.ToString(), matches.Count);
            }
            catch (Exception ex)
            {
                return ($"[Regex error] {ex.Message}", 0);
            }
        }

        public string Diff(string a, string b)
        {
            a ??= "";
            b ??= "";
            var linesA = a.Replace("\r\n", "\n").Split('\n');
            var linesB = b.Replace("\r\n", "\n").Split('\n');
            var sb = new StringBuilder();
            int max = Math.Max(linesA.Length, linesB.Length);
            int changes = 0;

            for (int i = 0; i < max; i++)
            {
                string la = i < linesA.Length ? linesA[i] : null;
                string lb = i < linesB.Length ? linesB[i] : null;

                if (la == lb)
                {
                    sb.AppendLine($"  {la}");
                }
                else
                {
                    changes++;
                    if (la != null) sb.AppendLine($"- {la}");
                    if (lb != null) sb.AppendLine($"+ {lb}");
                }
            }

            sb.Insert(0, $"Line-based diff  |  changed blocks: {changes}\n" + new string('-', 40) + "\n");
            return sb.ToString();
        }
    }
}
