using API.Data;
using API.Entities;
using API.Middleware;
using API.RequestHelpers;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// get access to CloudinarySettings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    //opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // drop db - chg connstr: here+appsettings - delete migrations - re-add migrations - start app
});
builder.Services.AddCors();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddScoped<PaymentsService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddIdentityApiEndpoints<User>(opt =>
    {
        opt.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StoreContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

// enable the return of the contents of wwwroot subfolder
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(opt =>
{
    opt
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials() // to allow cookies
    .WithOrigins("https://localhost:3000"); 
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<User>();

// configure fallback controller
app.MapFallbackToController("Index", "Fallback");

await DbInitializer.InitDb(app);

app.Run();
