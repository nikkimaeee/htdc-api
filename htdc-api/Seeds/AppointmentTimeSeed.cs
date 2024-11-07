using htdc_api.Models;
using Microsoft.EntityFrameworkCore;

namespace htdc_api.Seeds;

public class AppointmentTimeSeed
{
    internal static void Initialize(ModelBuilder builder)
    {
        var data = new List<AppointmentTime>();
        var id = 1;
        for (var i = 9; i < 19; i++)
        {
            var timeFrom = i > 12 ? i - 12 : i;
            var timeTo = i + 1 > 12 ? (i + 1) - 12 : i + 1;
            var labelFrom = i < 12 ? "am" : "pm";
            var labelTo = (i + 1) < 12 ? "am" : "pm";
            data.Add(new AppointmentTime
            {
                Id = id,
                Name =
                    $"{timeFrom.ToString().PadLeft(2, '0')}:00{labelFrom} - {timeTo.ToString().PadLeft(2, '0')}:00{labelTo}",
                From = $"{i.ToString().PadLeft(2, '0')}:00:00",
                To = $"{(i + 1).ToString().PadLeft(2, '0')}:00:00",
                MilitaryTime = i,
                IsActive = true,
                DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
            });
            id++;
        }

        builder.Entity<AppointmentTime>().HasData(data);
    }
}
