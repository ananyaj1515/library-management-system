using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Models;
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
                "SELECT Id, Title, Genre, PublicationDate, Rating, Author FROM Books"
            ).ToList();
            return View(books);
        }

        public IActionResult RedirectToBookForm()
        {
            return View("AddBook");
        }

        public async Task<IActionResult> CreateBook(BookDto bookDto)
        {
           if (bookDto == null || string.IsNullOrEmpty(bookDto.Title) || string.IsNullOrEmpty(bookDto.Author))
            {
                ViewBag.ErrorMessage = "Title and Author are required.";
                return View("AddBook");
            }

            var existingBook = _context.Database.SqlQueryRaw<BookDto>(
                "SELECT * FROM Books WHERE Title = {0} AND Author = {1}",
                new SqlParameter("@Title", bookDto.Title),
                new SqlParameter("@Author", bookDto.Author)
            ).ToList();
            
            if (existingBook.Count > 0)
            {
                ViewBag.ErrorMessage = "Book with this title and author already exists.";
                return View("AddBook");
            } 
            else
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Books (Title, Author, Genre, PublicationDate, Rating) VALUES({0}, {1}, {2}, {3}, {4})",
                    new SqlParameter("@Title", bookDto.Title),
                    new SqlParameter("@Author", bookDto.Author),
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
                "SELECT * FROM Books WHERE Id = {0}",
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
                "SELECT * FROM Books WHERE Id = {0}",
                new SqlParameter("@Id", bookDto.Id)
            ).ToList().FirstOrDefault();
    
            if (book == null)
            {
                ViewBag.ErrorMessage = "Book not found.";
                return View("Index");
            }
        
            return View( new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Rating = book.Rating,
                PublicationDate = book.PublicationDate,
                Genre = book.Genre
            });
        }

        public async Task<IActionResult> UpdateBookDetail(BookDto bookDto)
        {
            if (bookDto == null || string.IsNullOrEmpty(bookDto.Title) || string.IsNullOrEmpty(bookDto.Author))
            {
                ViewBag.ErrorMessage = "Title and Author are required";
                return View("UpdateBook", bookDto);
            }

            var book = _context.Database.SqlQueryRaw<BookDto>(
                "SELECT * FROM Books WHERE Id = {0}",
                new SqlParameter("@Id", bookDto.Id)
            ).ToList().FirstOrDefault();

            if (book == null)
            {
                ViewBag.ErrorMessage = "Book not found.";
                return View("UpdateBook", bookDto);
            }
    

            _context.Database.ExecuteSqlRaw(
                "UPDATE Books SET Title = {0}, Author = {1}, Genre = {2}, PublicationDate = {3}, Rating = {4} WHERE Id = {5}",
                new SqlParameter("@Title", bookDto.Title),
                new SqlParameter("@Author", bookDto.Author),
                new SqlParameter("@Genre", bookDto.Genre),
                new SqlParameter("@PublicationDate", bookDto.PublicationDate),
                new SqlParameter("@Rating", bookDto.Rating),
                new SqlParameter("@Id", bookDto.Id)
            );
            TempData["SuccessMessage"] = "Book updated successfully.";
            return RedirectToAction("Index");
        }  

    }
}
