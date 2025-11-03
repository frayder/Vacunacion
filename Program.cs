using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Infrastructure;

// Configurar licencia EPPlus antes de usar ExcelPackage  
 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Configurar Entity Framework Core con SQL Server y manejo de errores
builder.Services.AddDbContext<Highdmin.Data.ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            // Habilitar reintentos (si quieres)
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
            // QuerySplittingBehavior está disponible en EF Core relacional
            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
});

// Registrar servicios personalizados
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Highdmin.Services.IMenuService, Highdmin.Services.MenuService>();
builder.Services.AddScoped<Highdmin.Services.IEmpresaService, Highdmin.Services.EmpresaService>();
builder.Services.AddScoped<Highdmin.Services.AuthorizationService>();
builder.Services.AddScoped<Highdmin.Services.IPasswordHashService, Highdmin.Services.PasswordHashService>();

// Registrar servicios genéricos de Import/Export
builder.Services.AddScoped<Highdmin.Services.IImportExportService, Highdmin.Services.ImportExportService>();
builder.Services.AddSingleton<Highdmin.Services.IEntityConfigurationService, Highdmin.Services.EntityConfigurationService>();
builder.Services.AddScoped<Highdmin.Services.IDataPersistenceService, Highdmin.Services.DataPersistenceService>();
builder.Services.AddScoped<Highdmin.Services.IControllerImportExportService, Highdmin.Services.ControllerImportExportService>();

// Habilitar sesiones
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware personalizado para manejar errores de base de datos
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Npgsql.PostgresException)
    {
        context.Response.Redirect("/Error/DatabaseError");
    }
    catch (Microsoft.Data.SqlClient.SqlException)
    {
        context.Response.Redirect("/Error/DatabaseError");
    }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
    {
        context.Response.Redirect("/Error/DatabaseError");
    }
    catch (Exception)
    {
        throw;
    }
});

// Configurar middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
