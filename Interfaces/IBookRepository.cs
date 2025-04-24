using SharedModels;
public interface IBookRepository
{
    IEnumerable<Book> GetAllBooks();
    Book? GetBookById(int id);
    Book? GetBookByName(string name);
    bool AddBook(Book book);
    bool UpdateBook(Book book);
    bool DeleteBook(int id);
}