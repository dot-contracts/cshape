using System;
using System.IO;

namespace nexus.common
{
    public class FileWatcher
    {
        public enum ChangeTypes { created, changed, renamed, deleted, all };

        public event OnChangeEventHandler OnChange; public delegate void OnChangeEventHandler(ChangeTypes change, string oldName, string newName );

        public void Create(string PathToWatch, string Filter = "*.*")
        {

            // If a directory is not specified, exit program.  
            if (Directory.Exists(PathToWatch))
            {
                try
                {
                    // Create a new FileSystemWatcher and set its properties.  
                    FileSystemWatcher watcher = new FileSystemWatcher();
                    watcher.Path = PathToWatch;
                    // Watch both files and subdirectories.  
                    watcher.IncludeSubdirectories = true;
                    // Watch for all changes specified in the NotifyFilters  
                    //enumeration.  
                    watcher.NotifyFilter = NotifyFilters.Attributes |
                    NotifyFilters.CreationTime |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.FileName |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Security |
                    NotifyFilters.Size;
                    // Watch all files.  
                    watcher.Filter = Filter;
                    // Add event handlers.  

                    watcher.Changed += new FileSystemEventHandler(OnChanged);
                    watcher.Created += new FileSystemEventHandler(OnChanged);
                    watcher.Deleted += new FileSystemEventHandler(OnChanged);
                    watcher.Renamed += new RenamedEventHandler   (OnRenamed);
                    //Start monitoring.  
                    watcher.EnableRaisingEvents = true;
                }
                catch (IOException e)
                {
                    Console.WriteLine("A Exception Occurred :" + e);
                }
                catch (Exception oe)
                {
                    Console.WriteLine("An Exception Occurred :" + oe);
                }

            }

        }
        // Define the event handlers.  
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            ChangeTypes ctype = ChangeTypes.created;

            switch (e.ChangeType )      
            {
                case WatcherChangeTypes.Changed: ctype = ChangeTypes.changed; break;
                case WatcherChangeTypes.Created: ctype = ChangeTypes.created; break;
                case WatcherChangeTypes.Deleted: ctype = ChangeTypes.deleted; break;
                case WatcherChangeTypes.All:     ctype = ChangeTypes.all;     break;
            }

            OnChange?.Invoke(ctype, e.FullPath, e.FullPath);

        }
        public void OnRenamed(object source, RenamedEventArgs e)
        {
            OnChange?.Invoke(ChangeTypes.renamed, e.OldFullPath, e.FullPath);
        }

    }
}
