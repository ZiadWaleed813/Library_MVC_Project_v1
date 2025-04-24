using Microsoft.AspNetCore.Authorization; // Handles role-based access control
using Microsoft.AspNetCore.Mvc; // Provides attributes like [HttpGet], [HttpPost] for routing
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SharedModels; // Assuming this namespace contains the SharedModels.Book class







public class BooksController : Controller
{
    private readonly LibraryContext _context;
    private readonly BookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly BorrowingRepository _borrowingRepository; // Added for borrowing functionality

    public BooksController(LibraryContext context, BookRepository bookRepository, BorrowingRepository borrowingRepository, IAuthorRepository authorRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _borrowingRepository = borrowingRepository ?? throw new ArgumentNullException(nameof(borrowingRepository));
        _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
    }

    public IActionResult Index()
    {
        var books = _bookRepository.GetAllBooks().ToList();

        return View(books);
    }

    public IActionResult Details(int id)
    {
        var book = _bookRepository.GetBookById(id);

        if (book == null)
        {
            return RedirectToAction("NotFound", "Books");
        }

        var borrowingRecord = _context.Borrowings
        .Include(b => b.Book)
        .FirstOrDefault(b => b.BookId == id && b.Book != null && !b.Book.IsAvailable);

        var viewModel = new BookDetailsViewModel
        {
            Book = book,
            IsBorrowed = borrowingRecord != null,
            BorrowingId = borrowingRecord?.Id
        };

        return View(viewModel);
    }


    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Create()//The method with [HttpGet] is responsible for fetching data required to render the form
    {
        // var authors = _context.Authors.ToList(); // Fetch all authors from the database
        var authors = _authorRepository.GetAllAuthors().ToList();
        ViewBag.Authors = new SelectList(authors, "ID", "Name"); // Create a dropdown list
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Create(Book book)
    {
        // Populate dropdown again in case of validation errors
        var authors = _authorRepository.GetAllAuthors().ToList();
        ViewBag.Authors = new SelectList(authors, "ID", "Name");

        if (ModelState.IsValid)
        {
            var existingAuthor = _bookRepository.GetBookByName(book.Name);
            if (existingAuthor != null)
            {
                // Add a custom error to the ModelState
                ModelState.AddModelError("Name", "A book with this name already exists.");
                return View(book); // Return the same view with the error message
            }

            if (_bookRepository.AddBook(book))
            {
                return RedirectToAction(nameof(Index)); // Redirect to the Index page
            }
            else
            {
                ModelState.AddModelError("", "Failed to add the book.");

            }
        }
        return View(book);
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Edit(int id)
    {
        var book = _bookRepository.GetBookById(id);
        if (book == null) return NotFound();

        ViewBag.Authors = new SelectList(_context.Authors, "ID", "Name");
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Edit(Book book)
    {
        // Check if a book with the same name exists
        var existingBook = _bookRepository.GetBookByName(book.Name);

        // If no book exists with the same name, proceed with the update
        if (existingBook == null)
        {
            if (ModelState.IsValid && _bookRepository.UpdateBook(book))
            {
                return RedirectToAction(nameof(Index));
            }

            // Populate authors in the ViewBag for the dropdown list
            ViewBag.Authors = new SelectList(_authorRepository.GetAllAuthors(), "ID", "Name");//_context.Authors
            return View(book);
        }
        // If the book exists but it’s the same book being edited, allow renaming to the same name
        else if (existingBook.ID == book.ID)
        {
            if (ModelState.IsValid) //&& _bookRepository.UpdateBook(book)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Authors = new SelectList(_authorRepository.GetAllAuthors(), "ID", "Name");
            return View(book);
        }
        else
        {
            // If a different book exists with the same name, display error
            ModelState.AddModelError("Name", "A book with this name already exists.");
            ViewBag.Authors = new SelectList(_authorRepository.GetAllAuthors(), "ID", "Name");
            return View(book); // Return the view with the error
        }
    }

    [Authorize(Roles = "User,Admin")]
    public IActionResult Delete(int id)
    {
        if (_bookRepository.DeleteBook(id))
        {
            return RedirectToAction(nameof(Index));
        }
        return NotFound();
    }

    private int? GetUserId()
    {
        // Assuming you're using authentication middleware and User.Claims
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;
    }

    public IActionResult NotFound()
    {
        return View();
    }
}