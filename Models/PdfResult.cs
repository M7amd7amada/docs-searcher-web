namespace docs_searcher_web.Models;

public class PdfResult
{
    public int PageNumber { get; set; }
    public int KeywordCount { get; set; }
    public string BookPath { get; set; } = string.Empty;

    public string BookName => Path.GetFileNameWithoutExtension(BookPath);
    public string PageLink => $"file:///{BookPath}#page={PageNumber}";
}