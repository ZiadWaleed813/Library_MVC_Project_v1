using Microsoft.EntityFrameworkCore;
using SharedModels;

public class UserRepository
{
    private readonly LibraryContext _context;

    public UserRepository(LibraryContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Get a user by ID
    public User? GetUserById(int userId)
    {
        return _context.Users.FirstOrDefault(u => u.Id == userId);
    }

    // Get all users
    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users.Include(u => u.Role).ToList(); // Include Role for easy access
    }

    // Add a new user
    public void AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    // Update an existing user
    public void UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    // Delete a user
    public void DeleteUser(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}