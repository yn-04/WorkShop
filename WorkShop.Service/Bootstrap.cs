using Dashboard.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WorkShop.Core.Interfaces;
using WorkShop.Infrastructure.Data;
using WorkShop.Infrastructure.Repositories;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Services;

namespace myproject.Service;

public static class Bootstrap
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("WorkShop")
                   ?? throw new InvalidOperationException("ConnectionStrings:Default is missing");

        var connString = Aes256.DefaultInstance.Decrypt(conn, WorkShop.Core.Constants.SecurityKey.Aes256Key);

        services.AddDbContext<WorkShopDbContext>(opt => opt.UseSqlServer(connString));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
