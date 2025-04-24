using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SharedModels;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _context;

    public BookRepository(LibraryContext context)
    {
        _context = context;
    }
    public IEnumerable<Book> GetAllBooks()
    {
        return _context.Books.ToList();
    }

    public Book? GetBookById(int id)
    {
        var book = _context.Books
        .Include(b => b.Author) // Load the Author entity
        .Include(b => b.Borrowings) // Load Borrowing details
        .FirstOrDefault(b => b.ID == id);

        if (book == null)
        {
            return null;
        }

        return book;
    }
    public Book? GetBookByName(string name)
    {
        return _context.Books
        .Include(b => b.Author) // Load the Author entity
        .FirstOrDefault(a => a.Name.ToLower() == name.ToLower()); // Compare Name with the input
    }
    public bool AddBook(Book book)
    {
        try
        {
            _context.Books.Add(book);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool UpdateBook(Book book)
    {
        try
        {
            _context.Books.Update(book);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteBook(int id)
    {
        try
        {
            var book = _context.Books.Find(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
