using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dto
{
    public class BookDto
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public DateTime PublicationDate { get; set; }
        public string Author { get; set; } = null!;
        public decimal Rating { get; set; } = 0.0m;
    }
}
