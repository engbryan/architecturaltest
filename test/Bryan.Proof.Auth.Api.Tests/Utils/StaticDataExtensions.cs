using System.Text;

namespace Bryan.Proof.Auth.Api.Tests.Utils;

internal static class StaticDataExtensions
{
    internal enum StaticDataFolderRoot
    {
        StaticData,
        BlackBoxes
    }

    public static StringContent ToStringContentJson<T>(this T obj)
        => new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    private static string _staticDataPath = "";

    public static string StaticDataBlackBoxes(this string relativePath)
        => StaticData(relativePath, StaticDataFolderRoot.BlackBoxes);

    public static T StaticDataBlackBoxes<T>(this string relativePath)
        => StaticData<T>(relativePath, StaticDataFolderRoot.BlackBoxes);

    public static string StaticData(this string relativePath, StaticDataFolderRoot folder = StaticDataFolderRoot.StaticData)
    {
        if (string.IsNullOrWhiteSpace(_staticDataPath))
        {
            var assemblyName = typeof(StaticDataExtensions).Assembly.FullName?.Replace(".dll", "");
            DirectoryInfo dirInfo = new(Directory.GetCurrentDirectory());
            while (dirInfo!.Name != assemblyName)
                dirInfo = dirInfo.Parent;

            _staticDataPath = Path.Combine(dirInfo.FullName, assemblyName, folder.ToString());
        }

        var fullPath = Path.Combine((_staticDataPath + "\\" + relativePath).Split('/', '\\').ToArray());
        using var sr = new StreamReader(fullPath, Encoding.UTF8);
        var content = sr.ReadToEnd();
        return content;
    }

    public static T StaticData<T>(this string relativePath, StaticDataFolderRoot folder = StaticDataFolderRoot.StaticData)
    {
        var content = StaticData(relativePath, folder);
        return JsonSerializer.Deserialize<T>(content)!;
    }
}