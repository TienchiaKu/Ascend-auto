using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Services;
using DDD_2024.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ASCENDContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ASCENDContext") ?? throw new InvalidOperationException("Connection string 'ASCENDContext' not found.")));
builder.Services.AddDbContext<ATIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ATIContext") ?? throw new InvalidOperationException("Connection string 'ATIContext' not found.")));
builder.Services.AddDbContext<KIR1NContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KIR1NContext") ?? throw new InvalidOperationException("Connection string 'KIR1NContext' not found.")));
builder.Services.AddDbContext<INTERTEKContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("INTERTEKContext") ?? throw new InvalidOperationException("Connection string 'INTERTEKContext' not found.")));
builder.Services.AddDbContext<TESTBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TESTBContext") ?? throw new InvalidOperationException("Connection string 'TESTBContext' not found.")));
builder.Services.AddDbContext<BizAutoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BizAutoContext") ?? throw new InvalidOperationException("Connection string 'BizAutoContext' not found.")));
builder.Services.AddDbContext<CusVendorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CusVendorContext") ?? throw new InvalidOperationException("Connection string 'CusVendorContext' not found.")));
builder.Services.AddDbContext<DDD_DutyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DDD_DutyContext") ?? throw new InvalidOperationException("Connection string 'DDD_DutyContext' not found.")));
builder.Services.AddDbContext<DoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DoContext") ?? throw new InvalidOperationException("Connection string 'DoContext' not found.")));
builder.Services.AddDbContext<DDD_EmployeeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DDD_EmployeeContext") ?? throw new InvalidOperationException("Connection string 'DDD_EmployeeContext' not found.")));
builder.Services.AddDbContext<ProjectMContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectMContext") ?? throw new InvalidOperationException("Connection string 'ProjectMContext' not found.")));
builder.Services.AddDbContext<ProjectDContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectDContext") ?? throw new InvalidOperationException("Connection string 'ProjectDContext' not found.")));
builder.Services.AddDbContext<ProjectDOontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectDOontext") ?? throw new InvalidOperationException("Connection string 'ProjectDOontext' not found.")));
builder.Services.AddDbContext<Project_DIDWContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Project_DIDWContext") ?? throw new InvalidOperationException("Connection string 'Project_DIDWContext' not found.")));
builder.Services.AddDbContext<Project_EmpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Project_EmpContext") ?? throw new InvalidOperationException("Connection string 'Project_EmpContext' not found.")));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IDoService, DoService>();
builder.Services.AddTransient<IDinService, DinService>();
builder.Services.AddTransient<IDwinService, DwinService>();
builder.Services.AddTransient<IProjectEmpService, ProjectEmpService>();
builder.Services.AddTransient<IDutyService, DutyService>();
builder.Services.AddTransient<ICusVendoeService, CusVendorService>();
builder.Services.AddTransient<IBounsCalService, BonusCalService>();
//builder.Services.AddSession();

//取得組態中資料庫連線設定
//string connectionString =
//    builder.Configuration.GetConnectionString("MPMasterContext");

//註冊EF Core 的FriendContext
//builder.Services.AddDbContext<MPMasterContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(60);
});

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
app.UseSession();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
