using System.IO;

public static class FileUtils
{
    public static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public static long GetLogFileSize(string filePath)
    {
        if (File.Exists(filePath))
            return new FileInfo(filePath).Length;
        return 0;
    }
}

