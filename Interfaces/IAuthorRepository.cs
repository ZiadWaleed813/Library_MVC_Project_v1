using SharedModels;
public interface IAuthorRepository
{
    IEnumerable<Author> GetAllAuthors();
    Author? GetAuthorById(int id);
    Author? GetAuthorByName(string id);
    bool AddAuthor(Author author);
    bool UpdateAuthor(Author author);
    bool DeleteAuthor(int id);
}