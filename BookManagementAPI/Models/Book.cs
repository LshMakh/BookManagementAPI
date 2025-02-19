namespace BookManagementAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public string AuthorName { get; set; }
        public int ViewCount { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public double PopularityScore => CalculatePopularityScore();

        private double CalculatePopularityScore()
        {
            var yearsSincePublished = DateTime.Now.Year - PublicationYear;
            return (ViewCount * 0.5) + Math.Pow(yearsSincePublished, 2);
        }
    }
}
