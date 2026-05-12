using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class AuthorController(AppDbContext _context) : Controller
    {
        // GET: AuthorController
        public IActionResult Index()
        {
            var authors = _context.Authors.Select( a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name,
                Rating = a.Rating,
                Bio = a.Bio,
                Email = a.Email
            }).ToList();
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View(authors);
        }

        public IActionResult RediectToAuthorForm()
        {
            return View("AddAuthor");
        }

        public async Task<IActionResult> CreateAuthor(AuthorDto authorDto)
        {
            if (authorDto == null || string.IsNullOrEmpty(authorDto.Name) || string.IsNullOrEmpty(authorDto.Email))
            {
                ViewBag.ErrorMessage = "Name and Email are required";
                return View("AddAuthor");
            }

            var existingAuthor = _context.Authors.FirstOrDefault(a => a.Email == authorDto.Email);
            if (existingAuthor != null)
            {
                ViewBag.ErrorMessage = "Author with this email already exists.";
                return View("AddAuthor");
            } 
            else
            {
                var newAuthor = new Author
                {
                    Name = authorDto.Name,
                    Rating = authorDto.Rating,
                    Bio = authorDto.Bio,
                    Email = authorDto.Email
                };
                _context.Authors.Add(newAuthor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Author added successfully.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
            TempData["SuccessMessage"] = "Author deleted successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateAuthor(int id)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Id == id);

            if (author == null)
            {
                TempData["ErrorMessage"] = "Author not found.";
                return RedirectToAction("Index");
            }

            return View(new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                Rating = author.Rating,
                Bio = author.Bio,
                Email = author.Email
            });
        }

        public async Task<IActionResult> UpdateAuthorDetail(AuthorDto authorDto)
        {
            if (authorDto == null || string.IsNullOrEmpty(authorDto.Name) || string.IsNullOrEmpty(authorDto.Email))
            {
                ViewBag.ErrorMessage = "Name and Email are required";
                return View("UpdateAuthor", authorDto);
            }

            var author = _context.Authors.FirstOrDefault(a => a.Id == authorDto.Id);

            if (author == null)
            {
                ViewBag.ErrorMessage = "Author not found.";
                return View("UpdateAuthor", authorDto);
            }

            author.Name = authorDto.Name;
            author.Rating = authorDto.Rating;
            author.Bio = authorDto.Bio;
            author.Email = authorDto.Email;

            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Author updated successfully.";
            return RedirectToAction("Index");

        }
        
    }
}
