namespace QuasarPad.Models
{
    public class EditorTab
    {
        public string Title { get; set; } = "Untitled";
        public string? FilePath { get; set; }
        public string Content { get; set; } = "";
        public bool IsModified { get; set; }

        public string DisplayTitle => IsModified ? Title + " *" : Title;
    }
}
