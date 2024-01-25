using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SystemUserContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SystemUserContext") ?? throw new InvalidOperationException("Connection string 'SystemUserContext' not found.")));
builder.Services.AddDbContext<DDD_DoMContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DDD_DoMContext") ?? throw new InvalidOperationException("Connection string 'DDD_DoMContext' not found.")));
builder.Services.AddDbContext<DDD_EmployeeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DDD_EmployeeContext") ?? throw new InvalidOperationException("Connection string 'DDD_EmployeeContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();

//取得組態中資料庫連線設定
//string connectionString =
//    builder.Configuration.GetConnectionString("MPMasterContext");

//註冊EF Core 的FriendContext
//builder.Services.AddDbContext<MPMasterContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
