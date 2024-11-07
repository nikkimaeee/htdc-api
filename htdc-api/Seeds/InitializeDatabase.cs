using htdc_api.Authentication;
using Microsoft.EntityFrameworkCore;

namespace htdc_api.Seeds;

public static class InitializeDatabase
{
    public static async void Initialize(this IApplicationBuilder builder)
    {
        var scope = builder.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

    }

}