using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SharedModels;

public class AuthorRepository : IAuthorRepository
{
    private readonly LibraryContext _context;

    public AuthorRepository(LibraryContext context)
    {
        _context = context;
    }

    public IEnumerable<Author> GetAllAuthors()
    {
        return _context.Authors
        .Include(a => a.Books)
        .ToList();
    }

    public Author? GetAuthorById(int id)
    {
        var author = _context.Authors.Include(a => a.Books).FirstOrDefault(a => a.ID == id);
        if (author == null)
            return null;

        return author;
    }
    public Author? GetAuthorByName(string name)
    {
        // Perform case-insensitive comparison by converting to lowercase
        return _context.Authors.FirstOrDefault(a => a.Name.ToLower() == name.ToLower());
    }

    public bool AddAuthor(Author author)
    {
        try
        {
            _context.Authors.Add(author);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool UpdateAuthor(Author author)
    {
        try
        {
            _context.Authors.Update(author);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteAuthor(int id)
    {
        try
        {
            var author = _context.Authors.Find(id);
            if (author == null) return false;

            _context.Authors.Remove(author);
            _context.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }
}