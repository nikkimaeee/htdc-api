using htdc_api.Authentication;
using htdc_api.Interface;
using Microsoft.EntityFrameworkCore;

namespace htdc_api.Seeds;

public static class InitializeDatabase
{
    public static async void Initialize(this IApplicationBuilder builder)
    {
        var scope = builder.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var bgJob = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
        bgJob.SendReminder();

    }

}