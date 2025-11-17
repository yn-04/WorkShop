using myproject.Core.Entities;

namespace myproject.Service;

public interface IUserService
{
    Task<IReadOnlyList<User>> SearchAsync(string? email = null, bool? isActive = null, CancellationToken ct = default);
    Task<User> CreateAsync(string email, string displayName, CancellationToken ct = default);
    Task<User?> UpdateAsync(Guid id, string displayName, bool isActive, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
