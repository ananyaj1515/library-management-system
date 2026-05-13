namespace LibraryManagementSystem.Models
{
    public class Summary
    {
        public int TotalBooks { get; set; }
        public int TotalAuthors { get; set; }
        public int TotalGenres { get; set; }
        public string TopRatedBook { get; set;} = null!;
        public string TopRatedAuthor { get; set;} = null!;
    }
}
