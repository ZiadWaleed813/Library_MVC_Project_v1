using Microsoft.EntityFrameworkCore;
using SharedModels;

public class LibraryContext : DbContext
{
    public LibraryContext() { } // Default constructor for EF Core tools
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Borrowing> Borrowings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the relationship between Book and Author
        modelBuilder.Entity<Book>()
            .HasOne(book => book.Author)         // A Book has one Author
            .WithMany(author => author.Books)    // An Author can have many Books
            .HasForeignKey(book => book.AuthorID); // Foreign Key in the Book table

        // Seed roles
        // modelBuilder.Entity<Role>().HasData(
        //     new Role { Id = 1, Name = "Admin" },
        //     new Role { Id = 2, Name = "Librarian" },
        //     new Role { Id = 3, Name = "User" }
        // );

        modelBuilder.Entity<Borrowing>()
            .HasOne(b => b.User)
            .WithMany() // Adjust if a user has multiple borrowings
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}

