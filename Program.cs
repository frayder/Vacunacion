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
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
});

// Registrar servicios personalizados
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Highdmin.Services.IMenuService, Highdmin.Services.MenuService>();
builder.Services.AddScoped<Highdmin.Services.IEmpresaService, Highdmin.Services.EmpresaService>();

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
app.UseRouting();

// Middleware personalizado para manejar errores de base de datos
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Microsoft.Data.SqlClient.SqlException)
    {
        // Manejar errores específicos de SQL Server
        context.Response.Redirect("/Error/DatabaseError");
    }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
    {
        // Manejar errores de Entity Framework
        context.Response.Redirect("/Error/DatabaseError");
    }
    catch (Exception)
    {
        // Para otros errores, usar el manejador por defecto
        throw;
    }
});

// Configurar middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();


// Habilitar sesión (después del middleware principal)
app.UseSession();

app.MapStaticAssets();

// Configurar rutas - redirigir raíz a login si no está autenticado
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
