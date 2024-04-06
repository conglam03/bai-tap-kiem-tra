using LTW_B4.Models;
using LTW_B4.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add services to the container. 
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddDefaultTokenProviders()
        .AddDefaultUI()
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.LogoutPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DenyAccessToDelete", policy =>
    {
        policy.RequireRole("Customer");
        policy.RequireAssertion(context =>
        {
            // Không cho phép thêm, s?a, xoá
            return false;
        });
    });
});

builder.Services.AddRazorPages();

builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DenyAccessToCustomerCompanyEmployee", policy =>
    {
        policy.RequireRole("Admin");
    });
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DenyAccessToDelete", policy =>
    {
        policy.RequireRole("Customer");
        policy.RequireAssertion(context =>
        {
            // Không cho phép thêm m?i và xóa
            return false;
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

// ??t tr??c UseRouting
app.UseSession();


app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute (
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "Employer",pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "Customer",pattern: "{area:exists}/{controller=ProductCustomerController}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=ProductCustomerController}/{action=Index}/{Id}");
});


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "Admin", pattern: "{area:exists}/{controller=Home}/{action=Index}/{Id}");
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{Id}");
});

app.Run();

