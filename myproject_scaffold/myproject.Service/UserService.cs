using myproject.Core.Entities;
using myproject.Core.Interfaces;

namespace myproject.Service;

public class UserService(IUserRepository users, IRepository<User> repo) : IUserService
{
    public async Task<User> CreateAsync(string email, string displayName, CancellationToken ct = default)
    {
        var existing = await users.GetByEmailAsync(email, ct);
        if (existing is not null) throw new InvalidOperationException($"Email already exists: {email}");

        var user = new User { Email = email, DisplayName = displayName, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        await repo.AddAsync(user, ct);
        await repo.SaveChangesAsync(ct);
        return user;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var found = await repo.GetByIdAsync(id, ct);
        if (found is null) return false;
        await repo.DeleteAsync(found, ct);
        await repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<User>> SearchAsync(string? email = null, bool? isActive = null, CancellationToken ct = default)
    {
        return await users.ListAsync(u =>
            (string.IsNullOrEmpty(email) || u.Email.Contains(email)) &&
            (isActive is null || u.IsActive == isActive.Value), ct);
    }

    public async Task<User?> UpdateAsync(Guid id, string displayName, bool isActive, CancellationToken ct = default)
    {
        var found = await repo.GetByIdAsync(id, ct);
        if (found is null) return null;
        found.DisplayName = displayName;
        found.IsActive = isActive;
        found.UpdatedAt = DateTime.UtcNow;
        await repo.UpdateAsync(found, ct);
        await repo.SaveChangesAsync(ct);
        return found;
    }
}
