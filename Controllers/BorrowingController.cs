using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;






public class BorrowingController : Controller
{
    private readonly BorrowingRepository _borrowingRepository;
    private readonly LibraryContext _context;

    public BorrowingController(BorrowingRepository borrowingRepository, LibraryContext context)
    {
        _borrowingRepository = borrowingRepository;
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Borrow(int bookId)
    {
        // Retrieve the book with related borrowing records
        var book = _context.Books.Include(b => b.Borrowings).FirstOrDefault(b => b.ID == bookId);

        if (!book.IsAvailable)
        {
            TempData["Message"] = "Failed to borrow the book. The book is not available.";
            return RedirectToAction("Index", "Books");
        }
        if (book == null)
        {
            TempData["Message"] = "Failed to borrow the book. The book is doesn't exist in the database.";
            return RedirectToAction("Index", "Books");
        }
        // Get the current user
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            return Unauthorized("You must be logged in to borrow a book.");
        }

        var borrowing = new Borrowing
        {
            UserId = user.Id,
            BookId = bookId,
            BorrowedDate = DateTime.Now
        };

        book.IsAvailable = false; // Update book availability
        _context.Borrowings.Add(borrowing); // Add a new borrowing record
        _context.SaveChanges();

        TempData["Message"] = $"You have borrowed \"{book.Name}\".";
        return RedirectToAction("Index", "Books");
    }

    [HttpPost]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Return(int bookId)
    {
        // Find the book and its active borrowing record
        var book = _context.Books.Include(b => b.Borrowings).FirstOrDefault(b => b.ID == bookId);

        if (book == null)
        {
            Console.WriteLine("Book not found in the database.");
            TempData["Message"] = "Failed to return the book. Book not found.";
            return RedirectToAction("Index", "Books");
        }

        // Find the active borrowing record for this book
        var borrowing = book.Borrowings?.FirstOrDefault(b => b.BookId == bookId && !book.IsAvailable);

        if (borrowing == null)
        {
            Console.WriteLine("Borrowing record not found or the book is already returned.");
            TempData["Message"] = "Failed to return the book. Borrowing record not found.";
            return RedirectToAction("Index", "Books");
        }

        // Update the book's availability
        book.IsAvailable = true;
        _context.SaveChanges();

        TempData["Message"] = $"The book \"{book.Name}\" has been successfully returned.";
        return RedirectToAction("Index", "Books");
    }
}