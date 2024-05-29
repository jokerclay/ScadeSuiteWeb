using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ScadeSuiteWeb.Server.Models;
using ScadeSuiteWeb.Server.Models.User;

namespace ScadeSuiteWeb.Server.Database;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<XUser, XRole, int>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Book>();
    }
    public DbSet<Book> Books => Set<Book>();
    public DbSet<XRole> XRoles => Set<XRole>();
    public DbSet<XUser> XUsers => Set<XUser>();
    public DbSet<XUserRole> XUserRoles => Set<XUserRole>();
}