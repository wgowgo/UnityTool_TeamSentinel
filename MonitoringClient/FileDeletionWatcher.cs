using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FileDeletionWatcher
{
    private List<string> events = new();
    private FileSystemWatcher watcher;

    public void Start()
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        watcher = new FileSystemWatcher(desktop);
        watcher.Deleted += OnFileDeleted;
        watcher.EnableRaisingEvents = true;
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        events.Add($"{DateTime.Now:HH:mm:ss} - {e.Name} 삭제됨");
        if (events.Count > 50)
            events.RemoveAt(0);
    }

    public string[] GetEvents()
    {
        var result = events.ToArray();
        events.Clear();
        return result;
    }
}

