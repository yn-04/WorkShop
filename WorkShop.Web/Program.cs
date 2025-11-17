using Dashboard.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using myproject.Service;
using WorkShop.Core.Entities;
using WorkShop.Core.Interfaces;
using WorkShop.Infrastructure.Data;
using WorkShop.Infrastructure.Repositories;
using WorkShop.Service;
using WorkShop.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// --- VVVVVV เพิ่มการตั้งค่า Authentication นี้ VVVVVV ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";     // หน้าที่ให้ไป ถ้ายังไม่ล็อกอิน
        options.AccessDeniedPath = "/User/AccessDenied"; // หน้าที่ให้ไป ถ้าไม่มีสิทธิ์
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // คุกกี้หมดอายุใน 7 วัน
        options.SlidingExpiration = true; // ต่ออายุคุกกี้อัตโนมัติถ้าใช้งาน
    });
// --- ^^^^^^ จบส่วนที่เพิ่ม ^^^^^^ ---


builder.Services.AddApplication(builder.Configuration);
var mvcBuilder = builder.Services.AddControllersWithViews();
#if DEBUG
mvcBuilder.AddRazorRuntimeCompilation();
#endif


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// --- VVVVVV ต้องเรียก UseAuthentication() VVVVVV ---
// **สำคัญมาก:** ต้องอยู่ระหว่าง UseRouting() และ UseAuthorization()
app.UseAuthentication();
app.UseAuthorization();
// --- ^^^^^^ ต้องเรียก UseAuthentication() ^^^^^^ ---


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// (คุณเรียก UseAuthentication/UseAuthorization ซ้ำ 2 ครั้ง เอาอันล่างนี้ออกได้)
// app.UseHttpsRedirection();
// app.UseAuthentication(); 
// app.UseAuthorization();
app.MapControllers();

app.Run();