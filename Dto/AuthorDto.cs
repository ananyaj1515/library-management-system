using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dto
{
    public class AuthorDto
    {
        [Key]
        public int Id { get; set;}
        public string Name { get; set; } = null!;
        public decimal Rating { get; set; } = 0.00m;
        public string Bio { get; set;} = null!;
        public string Email { get; set; } = null!;
        public int? NumBooks { get; set; } 

    }
}
