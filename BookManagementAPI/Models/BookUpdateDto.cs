namespace BookManagementAPI.Models
{
    public class BookUpdateDto
    {
        public string? Title { get; set; }
        public int? PublicationYear { get; set; }
        public string? AuthorName { get; set; }
    }
}
