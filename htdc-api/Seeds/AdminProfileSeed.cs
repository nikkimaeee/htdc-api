using htdc_api.Models;
using Microsoft.EntityFrameworkCore;

namespace htdc_api.Seeds;

public class AdminProfileSeed
{
    internal static void Initialize(ModelBuilder builder)
    {
        builder.Entity<UserProfile>().HasData(
            new UserProfile
            {
                Id = 1,
                AspNetUserId = "b74ddd14-6340-4840-95c2-db12554843e5",
                FirstName = "Admin",
                LastName = "Admin",
                IsActive = true,
                DateCreated = new DateTime(2022, 9, 19, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                Avatar = "",
                PatientInformation = null
            });
    }
}