using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set;}
        public string Name { get; set; } = null!;
        public decimal Rating { get; set; } = 0.0m;
        public string Bio { get; set;} = null!;
        public string Email { get; set; } = null!;
    }
}
