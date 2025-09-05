using Microsoft.EntityFrameworkCore;
using MovilidadSostenible_YAMAHA.Data;
using MovilidadSostenible_YAMAHA.DBContext;
using MovilidadSostenible_YAMAHA.Models;
using MovilidadSostenible_YAMAHA.ProxyClient;
using MovilidadSostenible_YAMAHA.Services;
using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SecretString>(builder.Configuration.GetSection("AmazonSecret"));
builder.Services.AddSingleton<AmazonSecret>();
var amazonSecretService = builder.Services.BuildServiceProvider().GetRequiredService<AmazonSecret>();
var secret = await amazonSecretService.GetSecretAsync();
string connectionString = $"Server=\"{secret.servername}\";port=\"3306\";Database=\"{secret.database}\";UserID=\"{secret.username}\";Password=\"{secret.password}\";";
builder.Services.AddScoped<ValidateConnectionBD>();
builder.Services.AddHttpClient<ColombiaService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString)
);

CultureInfo customCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
customCulture.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = customCulture;
CultureInfo.DefaultThreadCurrentUICulture = customCulture;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<LugarService>(); // Agregar esta línea

var app = builder.Build();

// Detectar si corre en contenedor
var isInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

// Leer X-Forwarded-* del ALB antes de redirigir a HTTPS
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Redirige a HTTPS (el ALB termina TLS y envía X-Forwarded-Proto=https)
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "custom",
    pattern: "Encuesta",
    defaults: new { controller = "TomaDatos", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
