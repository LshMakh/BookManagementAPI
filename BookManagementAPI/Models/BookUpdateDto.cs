namespace BookManagementAPI.Models
{
    public class BookUpdateDto
    {
        public string? Title { get; set; } = null;
        public int? PublicationYear { get; set; } = null;
        public string? AuthorName { get; set; } = null;
    }
}
