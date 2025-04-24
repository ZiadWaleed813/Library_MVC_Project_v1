using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels;











public class AuthorsController : Controller
{
    private readonly IAuthorRepository _authorRepository;
    private readonly LibraryContext _context;

    public AuthorsController(IAuthorRepository authorRepository, LibraryContext context)
    {
        _authorRepository = authorRepository;
        _context = context;
    }

    public IActionResult Index()
    {
        var authors = _authorRepository.GetAllAuthors();
        return View(authors);
    }

    public IActionResult Details(int id)
    {
        var author = _authorRepository.GetAuthorById(id);
        if (author == null) return NotFound();

        return View(author);
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User,Admin")]
    // public IActionResult Create(Author author)
    // {
    //     if (ModelState.IsValid && _authorRepository.AddAuthor(author))
    //     {
    //         return RedirectToAction(nameof(Index));
    //     }
    //     return View(author);
    // }

    public IActionResult Create(Author author)
    {
        // Check if the ModelState is valid
        if (ModelState.IsValid)
        {
            // Check if an author with the same name already exists
            var existingAuthor = _authorRepository.GetAuthorByName(author.Name);
            if (existingAuthor != null)
            {
                // Add a custom error to the ModelState
                ModelState.AddModelError("Name", "An author with this name already exists.");
                return View(author); // Return the same view with the error message
            }

            // Add the author to the repository if no duplicate is found
            if (_authorRepository.AddAuthor(author))
            {
                return RedirectToAction(nameof(Index)); // Redirect to the Index page
            }
        }

        // Return the view if ModelState is invalid
        return View(author);
    }

    [HttpGet]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Edit(int id)
    {
        var author = _authorRepository.GetAuthorById(id);
        if (author == null) return NotFound();

        return View(author);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "User,Admin")]
    public IActionResult Edit(Author author)
    {
        // Check if an author with the same name already exists
        var existingAuthor = _authorRepository.GetAuthorByName(author.Name);
        if (existingAuthor != null)
        {
            // Add a custom error to the ModelState
            ModelState.AddModelError("Name", "An author with this name already exists.");
            return View(author); // Return the same view with the error message
        }

        if (ModelState.IsValid && _authorRepository.UpdateAuthor(author))
        {
            return RedirectToAction(nameof(Index));
        }
        return View(author);
    }

    [Authorize(Roles = "User,Admin")]
    public IActionResult Delete(int id)
    {
        if (_authorRepository.DeleteAuthor(id))
        {
            return RedirectToAction(nameof(Index));
        }
        return NotFound();
    }
}
