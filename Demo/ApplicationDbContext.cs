
using Demo.Entities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<NewAppUser, NewAppRole, Guid>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
}