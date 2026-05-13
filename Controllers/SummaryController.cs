using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class SummaryController(AppDbContext _context) : Controller
    {
        // GET: SummaryController
        public ActionResult Index()
        {
            var summary = new Summary
            {
                TotalBooks = _context.Database.SqlQueryRaw<int>(
                    "SELECT COUNT(*) AS Value FROM Books"
                ).FirstOrDefault(),

                TotalAuthors = _context.Database.SqlQueryRaw<int>(
                    "SELECT COUNT(*) AS Value FROM Authors"
                ).FirstOrDefault(),

                TotalGenres = _context.Database.SqlQueryRaw<int>(
                    "SELECT COUNT(DISTINCT Genre) AS Value FROM Books"
                ).FirstOrDefault(),

                TopRatedAuthor = _context.Database.SqlQueryRaw<string>(
                    "SELECT TOP 1 Name AS Value FROM Authors ORDER BY Rating DESC"
                ).FirstOrDefault(),

                TopRatedBook = _context.Database.SqlQueryRaw<string>(
                    "SELECT TOP 1 Title AS Value FROM Books ORDER BY Rating DESC"
                ).FirstOrDefault(),

            };
            return View(summary);
        }

    }
}
