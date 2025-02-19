namespace BookManagementAPI.Models
{
    public class BookResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public string AuthorName { get; set; }
        public int ViewCount { get; set; }
        public DateTime? LastAccessed { get; set; }
        public double PopularityScore { get; set; }

        public static BookResponse FromBook(Book book)
        {
            return new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                PublicationYear = book.PublicationYear,
                AuthorName = book.AuthorName,
                ViewCount = book.ViewCount,
                LastAccessed = book.LastAccessed,
                PopularityScore = book.PopularityScore
            };
        }
    }
}
