using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class BookController(AppDbContext _context) : Controller
    {
        // GET: BookController
        public ActionResult Index()
    
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
             var books = _context.Books.Select( b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                AuthorId = b.AuthorId,
                AuthorName = b.Author.Name,
                Rating = b.Rating,
                PublicationDate = b.PublicationDate,
                Genre = b.Genre
            }).ToList();
            return View(books);
        }

        public IActionResult RedirectToBookForm()
        {
            return View("AddBook");
        }

        public async Task<IActionResult> CreateBook(BookDto bookDto)
        {
           if (bookDto == null || string.IsNullOrEmpty(bookDto.Title) || bookDto.AuthorId == 0)
            {
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.ErrorMessage = "Title and Author are required.";
                return View("AddBook");
            }
var
             existingBook = _context.Books.FirstOrDefault(b => b.Title == bookDto.Title && b.AuthorId == bookDto.AuthorId);
            if (existingBook != null)
            {
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.ErrorMessage = "Book with this title and author already exists.";
                return View("AddBook");
            } else
            {
                var newBook = new Book
                {
                    Title = bookDto.Title,
                    AuthorId = bookDto.AuthorId,
                    Rating = bookDto.Rating,
                    PublicationDate = bookDto.PublicationDate,
                    Genre = bookDto.Genre
                };
                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Book added successfully.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Book deleted successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateBook(BookDto bookDto) {
           var book = _context.Books.FirstOrDefault(b => b.Id == bookDto.Id);
    
            if (book == null)
            {
                ViewBag.ErrorMessage = "Book not found.";
                return View("Index");
            }
            else
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
                return View("UpdateBook", bookDto);
            }

            var book = _context.Books.FirstOrDefault(b => b.Id == bookDto.Id);
            if (book == null)
            {
                ViewBag.ErrorMessage = "Book not found.";
                return View("UpdateBook", bookDto);
            }
           
            book.Title = bookDto.Title;
            book.AuthorId = bookDto.AuthorId;
            book.Rating = bookDto.Rating;
            book.PublicationDate = bookDto.PublicationDate;
            book.Genre = bookDto.Genre;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Book updated successfully.";
            return RedirectToAction("Index");
        }  

    }
}
