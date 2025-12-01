using Dashboard.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using myproject.Service;
using System.Text.Json.Serialization;
using WorkShop.Core.Entities;
using WorkShop.Core.Interfaces;
using WorkShop.Infrastructure.Data;
using WorkShop.Infrastructure.Repositories;
using WorkShop.Service;
using WorkShop.Service.Interfaces;
using WorkShop.Service.Services;
using WorkShop.Web.Security; // ✅ เรียกใช้ Namespace ที่เก็บ ActivePermissionHandler

var builder = WebApplication.CreateBuilder(args);

// Database Connection
builder.Services.AddDbContext<WorkShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// Authentication (Cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// ✅ 1. ลงทะเบียน Handler ตัวใหม่ (ActivePermissionHandler)
// เปลี่ยนจาก ActiveRoleHandler เป็นตัวนี้ เพื่อเช็คละเอียดรายเมนู
builder.Services.AddScoped<IAuthorizationHandler, ActivePermissionHandler>();

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // ✅ 2. แก้ไข Policy ให้ใช้ Requirement แบบระบุ Code
    // ระบบจะวิ่งไปเช็คใน DB ว่า "User มี Role Active ที่ถือสิทธิ์นี้จริงหรือไม่"

    options.AddPolicy("CanViewHome", policy =>
    {
        // ส่งรหัสเมนู "SC001" เข้าไปตรวจสอบ
        policy.AddRequirements(new ActivePermissionRequirement("SC001"));
    });

    options.AddPolicy("CanManageUsers", policy =>
    {
        policy.AddRequirements(new ActivePermissionRequirement("AD001"));
    });

    options.AddPolicy("CanManageRoles", policy =>
    {
        policy.AddRequirements(new ActivePermissionRequirement("AD002"));
    });

    options.AddPolicy("CanManageProducts", policy =>
    {
        policy.AddRequirements(new ActivePermissionRequirement("PD001"));
    });

    options.AddPolicy("CanManageOrders", policy =>
    {
        policy.AddRequirements(new ActivePermissionRequirement("OR001"));
    });
});

// --- ลงทะเบียน Services (DI) ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserManageService, UserManageService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IRoleManageService, RoleManageService>();

builder.Services.AddApplication(builder.Configuration);

var mvcBuilder = builder.Services.AddControllersWithViews();
mvcBuilder.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

#if DEBUG
mvcBuilder.AddRazorRuntimeCompilation();
#endif

var app = builder.Build();

// Configure HTTP Request Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();

app.UseRouting(); // Routing

app.UseAuthentication(); //  AuthN (ตรวจสอบตัวตน)
app.UseAuthorization();  // AuthZ (ตรวจสอบสิทธิ์)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();

app.Run();