using Microsoft.EntityFrameworkCore;
using SharedModels;
public class RoleRepository
{
    private readonly LibraryContext _context;

    public RoleRepository(LibraryContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Get a role by ID
    public Role? GetRoleById(int roleId)
    {
        return _context.Roles.FirstOrDefault(r => r.Id == roleId);
    }

    // Get a role by name
    public Role? GetRoleByName(string roleName)
    {
        return _context.Roles.FirstOrDefault(r => r.Name == roleName);
    }

    // Get all roles
    public IEnumerable<Role> GetAllRoles()
    {
        return _context.Roles.ToList();
    }

    // Add a new role
    public void AddRole(Role role)
    {
        _context.Roles.Add(role);
        _context.SaveChanges();
    }

    // Update an existing role
    public void UpdateRole(Role role)
    {
        _context.Roles.Update(role);
        _context.SaveChanges();
    }

    // Delete a role
    public void DeleteRole(int roleId)
    {
        var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
        if (role != null)
        {
            _context.Roles.Remove(role);
            _context.SaveChanges();
        }
    }
}