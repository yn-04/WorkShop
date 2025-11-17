using Microsoft.EntityFrameworkCore;
using myproject.Core;
using myproject.Core.Entities;
using myproject.Core.Interfaces;

namespace myproject.Infrastructure;

public class UserRepository(AppDbContext db) : EfRepository<User>(db), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);
}
