using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dto
{
    public class BookDto
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; } = null!;
        
        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        [Required]
        public string Genre { get; set; } = null!;
        public DateTime PublicationDate { get; set; }

        [Range(1, 5)]
        public decimal Rating { get; set; } = 0.0m;
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; } = null!;
    }
}
