using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TareasMVC;
using TareasMVC.Data;
using TareasMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Filtro global a toda la aplicación que define la Politica para usuarios autenticados con Identity
var policiesUsersAuthenticate = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

// Add services to the container.
// le pasamos las politicas de autenticacion 
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter(policiesUsersAuthenticate));
}).AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(ResourceShare));
}).AddJsonOptions(options =>
{
    // agregamos esto para poder deserializar json y poder usar cuando se hacen referencias en los modelos
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


// Configuramos la conexion a sqlServer
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"))
    );

// Agregamos servicio para autenticacion despues de instalar identity
builder.Services.AddAuthentication();

// Ahora activamos Identity en nuestra aplicacion
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// como quiero trabajar con mis propias pantallas de login y registro debo configurar lo siguiente:
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options => {
    options.LoginPath = "/users/login";
    options.AccessDeniedPath = "/users/login";
});

// Agragamos lo siguiente para la localizacion, y por enede poder usar el IStringLocalizer que es una interface que se usa para la traduccion
builder.Services.AddLocalization(options =>
{
    // aqui colocamos la carpeta donde se encuentran los archivos para la traduccion
    options.ResourcesPath = "Resources";
});

// para el servicio de encontrar id del usuario
builder.Services.AddTransient<IServiceUser, ServiceUsers>();

// configurando automapper para el proyecto
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();

// Aqui colocamos las culturas soportadas por el sistema, que en este caso vendria a ser el idioma o los idiomas soportados o que queremos agregar
var cultureUiSupported = new[] { "es", "en" };

app.UseRequestLocalization(options =>
{
    // Aqui le decimos que la cultura por defecto es espanol
    options.DefaultRequestCulture = new RequestCulture("es");

    // Aqui las culturas soportadas
    options.SupportedUICultures = cultureUiSupported.Select(culture => new CultureInfo(culture)).ToList();
});

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

// Al configurar Identity lo colocamos, q es para obtener la data del usuario autenticado
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

