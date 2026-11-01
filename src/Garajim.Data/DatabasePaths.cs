namespace Garajim.Data;

public static class DatabasePaths
{
    /// <summary>%LocalAppData%\Garajim\garajim.db (klasör yoksa oluşturulur).</summary>
    public static string DefaultFilePath
    {
        get
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Garajim");
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "garajim.db");
        }
    }
}
