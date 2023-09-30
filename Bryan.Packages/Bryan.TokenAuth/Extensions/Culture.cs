using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Bryan.TokenAuth.Extensions;

[ExcludeFromCodeCoverage]
internal static class Culture
{
    public static string GetLanguage(string location)
    {
        var countryCultureList = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Where(c => new RegionInfo(c.Name).TwoLetterISORegionName == location)
            .ToList();

        if (countryCultureList.Any(c => c.TwoLetterISOLanguageName.Equals("pt", StringComparison.InvariantCultureIgnoreCase)))
            return "pt";

        if (countryCultureList.Any(c => c.TwoLetterISOLanguageName.Equals("es", StringComparison.InvariantCultureIgnoreCase)))
            return "es";

        return "en";
    }
}