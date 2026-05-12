using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dto;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class AuthController(AppDbContext _context) : Controller
    {
        // GET: AuthController
        public IActionResult Login()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            if (userDto == null || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Username)
            || string.IsNullOrEmpty(userDto.Password))
            {
                ViewBag.ErrorMessage = "All fields are required.";
                return View("Register");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "User with this email already exists.";
                return View("Register");
            } 
            else
            {
                var newUser = new User
                {
                        Email = userDto.Email,
                        Username = userDto.Username,
                        Password = userDto.Password
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registration successful. Please log in."; 
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> LoginUser(UserDto userDto)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                if (existingUser.Password == userDto.Password)
                {
                   return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ViewBag.ErrorMessage = "Incorrect password.";
                    return View("Login");
                }
            } else
            {
                ViewBag.ErrorMessage = "User with this email does not exist";
                return View("Login");
            }
        }

    }
}
