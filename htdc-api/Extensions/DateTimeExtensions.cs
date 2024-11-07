using System.Globalization;

namespace htdc_api.Extensions;

public static class DateTimeExtensions
{
    static TimeZoneInfo infotime = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
    static CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-PH");

    public static DateTime ConvertTime(this DateTime input)
    {
        return TimeZoneInfo.ConvertTime(input, infotime);
    }

    public static DateTime ConvertTimeFromUtc(this DateTime input)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(input), infotime);
    }
}