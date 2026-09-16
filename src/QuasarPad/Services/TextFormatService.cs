using System.Text;
using System.Text.RegularExpressions;

namespace QuasarPad.Services
{
    public class TextFormatService
    {
        /// <summary>
        /// Clean and pretty-format plain text:
        /// - Trim trailing spaces
        /// - Normalize line endings
        /// - Collapse multiple blank lines to max 2
        /// - Remove excessive spaces inside lines
        /// </summary>
        public string FormatPlainText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Normalize line endings to \n first
            string text = input.Replace("\r\n", "\n").Replace("\r", "\n");

            var lines = text.Split('\n');
            var sb = new StringBuilder();
            int blankCount = 0;

            foreach (var rawLine in lines)
            {
                // Trim trailing whitespace
                string line = rawLine.TrimEnd();

                // Collapse multiple spaces inside the line (keep single space)
                line = Regex.Replace(line, @"[ \t]{2,}", " ");

                if (string.IsNullOrWhiteSpace(line))
                {
                    blankCount++;
                    if (blankCount <= 2) // allow max 2 consecutive blank lines
                    {
                        sb.AppendLine();
                    }
                }
                else
                {
                    blankCount = 0;
                    sb.AppendLine(line);
                }
            }

            // Remove trailing blank lines at the end
            string result = sb.ToString().TrimEnd('\n', '\r') + "\n";
            return result;
        }

        /// <summary>
        /// Simple paragraph reflow: join lines that seem to be soft-wrapped
        /// and re-wrap at approximately maxLineLength characters.
        /// </summary>
        public string ReflowParagraphs(string input, int maxLineLength = 80)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            string text = input.Replace("\r\n", "\n").Replace("\r", "\n");
            var paragraphs = Regex.Split(text, @"\n\s*\n");
            var sb = new StringBuilder();

            foreach (var para in paragraphs)
            {
                // Join soft-wrapped lines inside paragraph
                string joined = Regex.Replace(para.Trim(), @"\n+", " ");
                joined = Regex.Replace(joined, @"[ \t]{2,}", " ").Trim();

                if (string.IsNullOrEmpty(joined))
                {
                    sb.AppendLine();
                    continue;
                }

                // Simple word wrap
                var words = joined.Split(' ');
                var currentLine = new StringBuilder();

                foreach (var word in words)
                {
                    if (currentLine.Length + word.Length + 1 > maxLineLength && currentLine.Length > 0)
                    {
                        sb.AppendLine(currentLine.ToString().TrimEnd());
                        currentLine.Clear();
                    }

                    if (currentLine.Length > 0)
                        currentLine.Append(' ');
                    currentLine.Append(word);
                }

                if (currentLine.Length > 0)
                    sb.AppendLine(currentLine.ToString());

                sb.AppendLine(); // blank line between paragraphs
            }

            return sb.ToString().TrimEnd() + "\n";
        }
    }
}
