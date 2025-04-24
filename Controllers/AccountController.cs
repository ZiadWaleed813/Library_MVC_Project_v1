using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;

public class AccountController : Controller
{
    private readonly LibraryContext _context;

    public AccountController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        // Save the return URL for post-login redirection

        var redirectUrl = Url.Action("OnLoginCallback", "Account", new { returnUrl });
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult OnLoginCallback(string returnUrl = "/")
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = User.Identity?.Name;

        if (!string.IsNullOrEmpty(email))
        {
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                var userRole = _context.Roles.FirstOrDefault(r => r.Name == "User");
                if (userRole == null)
                {
                    throw new InvalidOperationException("Default 'User' role not found.");
                }

                user = new User
                {
                    UserName = name,
                    Email = email,
                    PasswordHash = null,
                    RoleId = userRole.Id
                };

                _context.Users.Add(user);
                _context.SaveChanges();
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty), // Use a default value if UserName is null
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? string.Empty),
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity)).Wait();//uses Wait() to ensure it executes synchronously and properly sets the authentication cookie before redirecting.
        }

        return LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {

        return View(); // Return Access Denied view
    }

}