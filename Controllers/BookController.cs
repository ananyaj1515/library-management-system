using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BookController(AppDbContext _context) : Controller
    {
        // GET: BookController
        public ActionResult Index()
    
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            var books = _context.Database.SqlQueryRaw<BookDto>(
                "SELECT b.Id, b.Title, b.Genre, b.PublicationDate, b.Rating, b.AuthorId, a.Name AS AuthorName " +
                "FROM Books b " + 
                "INNER JOIN Authors a ON a.Id = b.AuthorID"
            ).ToList();
            return View(books);
        }

        public IActionResult RedirectToBookForm()
        {
            populateAuthorsInViewBag();
            return View("AddBook");
        }

        public async Task<IActionResult> CreateBook(BookDto bookDto)
        {
           if (bookDto == null || string.IsNullOrEmpty(bookDto.Title) || bookDto.AuthorId == 0)
            {
                ViewBag.ErrorMessage = "Title and Author are required.";
                populateAuthorsInViewBag();
                return View("AddBook");
            }

            var existingBook = _context.Database.SqlQueryRaw<BookDto>(
                "SELECT Id, Title, Genre, PublicationDate, Rating, AuthorID, NULL AS AuthorName from Books " +
                "WHERE Title = {0} AND AuthorID = {1}",
                new SqlParameter("@Title", bookDto.Title),
                new SqlParameter("@AuthorID", bookDto.AuthorId)
            ).ToList();
            
            if (existingBook.Count > 0)
            {
                ViewBag.ErrorMessage = "Book with this title and author already exists.";
                populateAuthorsInViewBag();
                return View("AddBook");
            } 
            else
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Books (Title, AuthorId, Genre, PublicationDate, Rating) VALUES({0}, {1}, {2}, {3}, {4})",
                    new SqlParameter("@Title", bookDto.Title),
                    new SqlParameter("@AuthorID", bookDto.AuthorId),
                    new SqlParameter("@Genre", bookDto.Genre),
                    new SqlParameter("@PublicationDate", bookDto.PublicationDate),
                    new SqlParameter("@Rating", bookDto.Rating)
                );
               
                TempData["SuccessMessage"] = "Book added successfully.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = _context.Database.SqlQueryRaw<BookDto>(
                "SELECT Id, Title, Genre, PublicationDate, Rating, AuthorId, NULL AS AuthorName FROM Books WHERE Id = {0}",
                new SqlParameter("@Id", id)
            ).ToList().FirstOrDefault();
            
            if (book != null)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM Books WHERE Id = {0}",
                    new SqlParameter("@Id", id)
                );
            }
            TempData["SuccessMessage"] = "Book deleted successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateBook(BookDto bookDto) {

            var book = _context.Database.SqlQueryRaw<BookDto>(
                "SELECT Id, Title, Genre, PublicationDate, Rating, AuthorId, NULL AS AuthorName FROM Books WHERE Id = {0}",
                new SqlParameter("@Id", bookDto.Id)
            ).ToList().FirstOrDefault();
    
            if (book == null)
            {
                ViewBag.ErrorMessage = "Book not found.";
                populateAuthorsInViewBag();
                return View("Index");
            }
        
            populateAuthorsInViewBag();
            return View( new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                Rating = book.Rating,
                PublicationDate = book.PublicationDate,
                Genre = book.Genre
            });
        }

        public async Task<IActionResult> UpdateBookDetail(BookDto bookDto)
        {
            if (bookDto == null || string.IsNullOrEmpty(bookDto.Title) || bookDto.AuthorId == 0)
            {
                ViewBag.ErrorMessage = "Title and Author are required";
                populateAuthorsInViewBag();
                return View("UpdateBook", bookDto);
            }

            var book = _context.Database.SqlQueryRaw<BookDto>(
                "SELECT Id, Title, Genre, PublicationDate, Rating, AuthorId, NULL AS AuthorName FROM Books WHERE Id = {0}",
                new SqlParameter("@Id", bookDto.Id)
            ).ToList().FirstOrDefault();

            if (book == null)
            {
                ViewBag.ErrorMessage = "Book not found.";
                populateAuthorsInViewBag();
                return View("UpdateBook", bookDto);
            }

            _context.Database.ExecuteSqlRaw(
                "UPDATE Books SET Title = {0}, AuthorID = {1}, Genre = {2}, PublicationDate = {3}, Rating = {4} WHERE Id = {5}",
                new SqlParameter("@Title", bookDto.Title),
                new SqlParameter("@AuthorId", bookDto.AuthorId),
                new SqlParameter("@Genre", bookDto.Genre),
                new SqlParameter("@PublicationDate", bookDto.PublicationDate),
                new SqlParameter("@Rating", bookDto.Rating),
                new SqlParameter("@Id", bookDto.Id)
            );
            TempData["SuccessMessage"] = "Book updated successfully.";
            return RedirectToAction("Index");
        }  
        private void populateAuthorsInViewBag()
        {
            ViewBag.Authors = _context.Database.SqlQueryRaw<AuthorDto>(
                "SELECT * FROM Authors"
            ).ToList();
        }
    }
}
