using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using docs_searcher_web.Models;
using docs_searcher_web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace docs_searcher_web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private const string DirPath = @"C:\Users\eddar\Books";

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Search(IFormCollection formCollection)
    {
        string searchKeyword = formCollection["searchKeyword"]!;
        List<PdfResult> searchResults = [];

        try
        {
            string[] files = Directory.GetFiles(DirPath, "*.pdf");

            foreach (string filePath in files)
            {
                List<PdfResult> fileResults = PdfServices.ReadFile(filePath, searchKeyword);
                searchResults.AddRange(fileResults);
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error => {ex.Message}");
            Console.ResetColor();
        }

        return View("SearchResults", searchResults);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
