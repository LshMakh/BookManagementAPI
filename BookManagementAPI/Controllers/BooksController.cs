using BookManagementAPI.Data;
using BookManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly BookDbContext _context;
        public BooksController(BookDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooks()
        {
            var books = await _context.Books
                .Where(b => !b.IsDeleted)
                .Select(b => BookResponse.FromBook(b))
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookResponse>> GetBook(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found" });

            book.ViewCount++;
            book.LastAccessed = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(BookResponse.FromBook(book));
        }

        [HttpPost]
    public async Task<ActionResult<BookResponse>> CreateBook(BookAddDto request)
        {
            if (await _context.Books.AnyAsync(b => b.Title == request.Title && !b.IsDeleted))
                return BadRequest(new { message = "A book with this title already exists" });

            var book = new Book
            {
                Title = request.Title,
                PublicationYear = request.PublicationYear,
                AuthorName = request.AuthorName,
                IsDeleted = false,
                DeletedAt = null,
                ViewCount = 0
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, BookResponse.FromBook(book));
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<BookResponse>>> CreateBooks(List<BookAddDto> requests)
        {
            var titles = requests.Select(b => b.Title).ToList();
            if (await _context.Books.AnyAsync(b => titles.Contains(b.Title) && !b.IsDeleted))
                return BadRequest(new { message = "One or more books already exist" });

            var books = requests.Select(request => new Book
            {
                Title = request.Title,
                PublicationYear = request.PublicationYear,
                AuthorName = request.AuthorName,
                IsDeleted = false,
                DeletedAt = null,
                ViewCount = 0
            }).ToList();
            
            _context.Books.AddRange(books);
            await _context.SaveChangesAsync();

            var responses = books.Select(b => BookResponse.FromBook(b)).ToList();
            return CreatedAtAction(nameof(GetBooks), responses);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDto request)
        {
            var existingBook = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (existingBook == null)
                return NotFound(new { message = $"Book with ID {id} not found" });

            if (request.Title != null)
            {
                if (await _context.Books.AnyAsync(b => b.Title == request.Title && b.Id != id && !b.IsDeleted))
                    return BadRequest(new { message = "A book with this title already exists" });

                existingBook.Title = request.Title;
            }

            if (request.PublicationYear.HasValue)
            {
                existingBook.PublicationYear = request.PublicationYear.Value;
            }

            if (request.AuthorName != null)
            {
                existingBook.AuthorName = request.AuthorName;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Book updated successfully",
                    book = BookResponse.FromBook(existingBook)
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Books.AnyAsync(b => b.Id == id))
                    return NotFound(new { message = $"Book with ID {id} not found" });
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            if (book == null)
                return NotFound(new { message = $"Book with ID {id} not found" });

            book.IsDeleted = true;
            book.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Book '{book.Title}' has been deleted successfully" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBooks([FromBody] List<int> ids)
        {
            var books = await _context.Books
                .Where(b => ids.Contains(b.Id) && !b.IsDeleted)
                .ToListAsync();

            if (!books.Any())
                return NotFound();

            foreach (var book in books)
            {
                book.IsDeleted = true;
                book.DeletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("deleted")]
        public async Task<ActionResult<IEnumerable<Book>>> GetDeletedBooks()
        {
            return await _context.Books
                .Where(b => b.IsDeleted)
                .ToListAsync();
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreBook(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id && b.IsDeleted);

            if (book == null)
                return NotFound(new { message = $"Deleted book with ID {id} not found" });

            book.IsDeleted = false;
            book.DeletedAt = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Book '{book.Title}' has been restored successfully", book = BookResponse.FromBook(book) });
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<Book>>> GetPopularBooks()
        {
            return await _context.Books
                .OrderByDescending(b => b.ViewCount)
                .Take(10)
                .ToListAsync();
        }

    }
}
