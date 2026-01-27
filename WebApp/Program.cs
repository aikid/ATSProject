using WebApp.Middlewares;
using WebApp.WebAppUtilities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    var baseUrl = builder.Configuration["WebAppUtil:apiATSPath"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("Config 'WebAppUtil:apiATSPath' não encontrada.");

    client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
});
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
Console.WriteLine(">>> Registrando AuthMiddleware");
app.UseMiddleware<AuthMiddleware>();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");
app.Run();