using Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<AtsDbContext>)
                         || d.ServiceType == typeof(AtsDbContext))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            services.AddDbContext<AtsDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AtsDbContext>();

            db.Database.EnsureCreated();

            if (!db.Users.Any())
            {
                db.Users.Add(new Domain.Model.User
                {
                    Email = "admin@ats.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    IsActive = true,
                    Role = "Admin"
                });

                db.SaveChanges();
            }
        });
    }
}
