using Microsoft.EntityFrameworkCore;
using SharedModels;

public class BorrowingRepository
{
    private readonly LibraryContext _context;

    public BorrowingRepository(LibraryContext context)
    {
        _context = context;
    }

    public bool BorrowBook(int userId, int bookId)
    {
        // Check if the book exists and is available
        var book = _context.Books.FirstOrDefault(b => b.ID == bookId);
        if (book == null || !book.IsAvailable)
        {
            return false; // Book not found or already borrowed
        }

        // Create a new borrowing record
        var borrowing = new Borrowing
        {
            UserId = userId,
            BookId = bookId,
            BorrowedDate = DateTime.Now
        };

        // Update the book's status to 'borrowed'
        book.IsAvailable = false;
        _context.Borrowings.Add(borrowing);
        _context.SaveChanges();

        return true;
    }

    public bool ReturnBook(int borrowingId)
    {
        var borrowing = _context.Borrowings
            .Include(b => b.Book) // Include related Book
            .FirstOrDefault(b => b.Id == borrowingId);

        if (borrowing == null || borrowing.Book == null || borrowing.Book.IsAvailable)
        {
            return false; // Borrowing not found, book missing, or already returned
        }

        // Update book availability
        borrowing.Book.IsAvailable = true;
        _context.SaveChanges();

        return true;
    }
}