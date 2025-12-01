using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkShop.Core.Entities;

namespace WorkShop.Service.Interfaces
{
    public interface IUserService
    {
        Task<User?> LoginhAsync(string email, string password, CancellationToken ct = default);
        Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default);
    }
}