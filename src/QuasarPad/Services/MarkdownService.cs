using Markdig;
using System.Text;

namespace QuasarPad.Services
{
    public class MarkdownService
    {
        private readonly MarkdownPipeline _pipeline;

        public MarkdownService()
        {
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePipeTables()
                .UseTaskLists()
                .UseEmphasisExtras()
                .UseAutoLinks()
                .Build();
        }

        public string ToFullHtml(string markdown, string title = "Document")
        {
            string body = Markdown.ToHtml(markdown ?? string.Empty, _pipeline);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine($"    <title>{System.Net.WebUtility.HtmlEncode(title)}</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, sans-serif; line-height: 1.6; max-width: 800px; margin: 40px auto; padding: 0 20px; color: #333; }");
            sb.AppendLine("        code { background: #f4f4f4; padding: 2px 6px; border-radius: 3px; font-family: Consolas, monospace; }");
            sb.AppendLine("        pre { background: #f4f4f4; padding: 16px; border-radius: 6px; overflow-x: auto; }");
            sb.AppendLine("        pre code { background: none; padding: 0; }");
            sb.AppendLine("        blockquote { border-left: 4px solid #ddd; margin: 0; padding-left: 16px; color: #666; }");
            sb.AppendLine("        table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("        th, td { border: 1px solid #ddd; padding: 8px 12px; text-align: left; }");
            sb.AppendLine("        th { background: #f8f8f8; }");
            sb.AppendLine("        img { max-width: 100%; height: auto; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine(body);
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public string ToBodyHtml(string markdown)
        {
            return Markdown.ToHtml(markdown ?? string.Empty, _pipeline);
        }
    }
}
