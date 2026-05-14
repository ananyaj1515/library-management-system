using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class SummaryController(AppDbContext _context) : Controller
    {
        // GET: SummaryController
        public ActionResult Index(int? AuthorId)
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

                Authors = _context.Database.SqlQueryRaw<AuthorDto>(
                 "SELECT Id, Name, Rating, Bio, Email, NULL AS NumBooks FROM Authors" 
                ).ToList(),

                BookForAuthor = AuthorId.HasValue 
                                ? _context.Database.SqlQueryRaw<BookDto>(
                                    "SELECT Id, Title, Genre, PublicationDate, Rating, AuthorId, NULL AS AuthorName FROM Books WHERE AuthorId = {0}",
                                    new SqlParameter("@AuthorID", AuthorId)
                                ).ToList()
                                : new List<BookDto>()
            };

            
            return View(summary);
        }

    }
}
