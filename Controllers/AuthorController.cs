using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementSystem.Controllers
{
    public class AuthorController(AppDbContext _context) : Controller
    {
        // GET: AuthorController
        public IActionResult Index()

        {
            var authors = _context.Database.SqlQueryRaw<AuthorDto>(
                "SELECT Id, Name, Rating, Bio, Email FROM Authors"
            ).ToList();
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

            var existingAuthor = _context.Database.SqlQueryRaw<AuthorDto>(
                "SELECT * FROM Authors WHERE Email = {0}", 
                new SqlParameter("@Email", authorDto.Email)
            ).ToList();

            if (existingAuthor.Count > 0)
            {
                ViewBag.ErrorMessage = "Author with this email already exists.";
                return View("AddAuthor");
            } 
            else
            {

                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Authors (Name, Rating, Bio, Email) VALUES ({0}, {1}, {2}, {3})",
                    new SqlParameter("@Name", authorDto.Name),
                    new SqlParameter("@Rating", authorDto.Rating),
                    new SqlParameter("@Bio", authorDto.Bio),  
                    new SqlParameter("@Email", authorDto.Email)
                );
                TempData["SuccessMessage"] = "Author added successfully.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = _context.Database.SqlQueryRaw<AuthorDto>(
                "SELECT * FROM Authors WHERE Id = {0}",
                new SqlParameter("@Id", id)
            ).ToList();

            if (author.Count > 0)
            {

                await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM Authors WHERE Id = {0}",
                    new SqlParameter("@Id", id)
                );
            }
            TempData["SuccessMessage"] = "Author deleted successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateAuthor(int id)
        {
            var author = _context.Database.SqlQueryRaw<AuthorDto>(
                "SELECT * FROM Authors WHERE Id = {0}",
                new SqlParameter("@Id", id)
            ).ToList().FirstOrDefault();

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

            var author = _context.Database.SqlQueryRaw<AuthorDto>(
                "SELECT * FROM Authors WHERE Id = {0}",
                new SqlParameter("@Id", authorDto.Id)
            ).ToList().FirstOrDefault();

            if (author == null)
            {
                ViewBag.ErrorMessage = "Author not found.";
                return View("UpdateAuthor", authorDto);
            }

            author.Name = authorDto.Name;
            author.Rating = authorDto.Rating;
            author.Bio = authorDto.Bio;
            author.Email = authorDto.Email;

            await _context.Database.ExecuteSqlRawAsync(
               "UPDATE Authors SET Name = {0}, Rating = {1}, Bio = {2}, Email = {3} WHERE Id = {4}",
               new SqlParameter("@Name", author.Name),
               new SqlParameter("@Rating", author.Rating),
               new SqlParameter("@Bio", author.Bio),
               new SqlParameter("@Email", author.Email),
               new SqlParameter("@Id", author.Id)
            );
            
            TempData["SuccessMessage"] = "Author updated successfully.";
            return RedirectToAction("Index");

        }
        
    }
}
