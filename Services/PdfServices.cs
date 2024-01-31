using System.Diagnostics;

using docs_searcher_web.Models;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace docs_searcher_web.Services;

public static class PdfServices
{
    public static List<PdfResult> ReadFile(
        string filePath,
        string searchKeyword)
    {
        List<PdfResult> result = new();

        try
        {
            using PdfReader pdfReader = new(filePath);
            using PdfDocument pdfDocument = new(pdfReader);

            int numberOfPages = pdfDocument.GetNumberOfPages();

            for (int pageNum = 1; pageNum <= numberOfPages; pageNum++)
            {
                LocationTextExtractionStrategy strategy = new();

                PdfCanvasProcessor parser = new(strategy);
                parser.ProcessPageContent(pdfDocument.GetPage(pageNum));

                string text = strategy.GetResultantText();

                int keywordCountOnPage = CountOccurrences(text, searchKeyword, out int counter);

                if (keywordCountOnPage > 0)
                {
                    result.Add(new PdfResult()
                    {
                        KeywordCount = counter,
                        PageNumber = pageNum,
                        BookPath = filePath
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error => {ex.Message}");
            Console.ResetColor();
        }

        return result.OrderByDescending(x => x.KeywordCount).Take(5).ToList();
    }

    private static int CountOccurrences(string text, string searchKeyword, out int counter)
    {
        counter = 0;
        int index = text.IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase);
        while (index != -1)
        {
            counter++;
            index = text.IndexOf(searchKeyword, index + 1, StringComparison.OrdinalIgnoreCase);
        }
        return counter;
    }

    public static void OpenPdfAtPage(string filePath, int pageNumber)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true,
                Arguments = $"#{pageNumber}"
            });
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error opening PDF: {ex.Message}");
            Console.ResetColor();
        }
    }
}