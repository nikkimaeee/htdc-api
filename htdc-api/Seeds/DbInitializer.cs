using Microsoft.EntityFrameworkCore;

namespace htdc_api.Seeds;

public class DbInitializer
{
    internal static void Initialize(ModelBuilder builder)
    {
        AdminProfileSeed.Initialize(builder);
        AppointmentTimeSeed.Initialize(builder);
    }
}