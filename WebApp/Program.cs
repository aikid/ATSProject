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

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();
var app = builder.Build();
app.UseSession();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}");
app.Run();