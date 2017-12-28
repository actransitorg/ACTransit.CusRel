using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ACTransit.Framework.Components
{
    /// <summary>
    /// An extension to the FileSystemWatcher which will trigger an event when a file has been fully created:  FileCreationCompleted.  The Created event is triggered as soon as the file is created on the drive, irrespective of how long it will take to be available.
    /// </summary>
    public class DelayedFileSystemWatcher : FileSystemWatcher
    {
        public event FileSystemEventHandler FileCreationCompleted;
        public event FileSystemEventHandler FileModificationCompleted;

        public DelayedFileSystemWatcher( string path ) : base( path ) { Initialize(); }

        public DelayedFileSystemWatcher( string path, string filter ) : base( path, filter ) { Initialize(); }

        private readonly ConcurrentDictionary<string, bool> _fileWasCreated = new ConcurrentDictionary<string, bool>();

        private void Initialize()
        {
            EnableRaisingEvents = true;
            Created += DelayedFileSystemWatcher_Created;
            Changed += DelayedFileSystemWatcher_Changed;
        }

        private void DelayedFileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //If the "Changed" event is fired with no prior events, then it is a modification, otherwise just exit, it's already being processed.
            if (_fileWasCreated.ContainsKey(e.FullPath))
                return;

            ProcessDelayedEvent(e, wasCreated: false);
        }

        void DelayedFileSystemWatcher_Created( object sender, FileSystemEventArgs e )
        {
            ProcessDelayedEvent(e, wasCreated: true);
        }

        private void ProcessDelayedEvent(FileSystemEventArgs e, bool wasCreated)
        {
            _fileWasCreated[e.FullPath] = wasCreated;

            var thread = new Thread(() => WaitForCompletion(e));
            thread.Start();
        }

        private void WaitForCompletion( FileSystemEventArgs e )
        {
            while( true )
            {
                try
                {
                    var file = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.None);
                    file.Close();
                    file.Dispose();
                }
                catch (IOException)
                {
                    //File is not available. Wait a half second before attempting again.
                    Thread.Sleep(250);
                    continue;
                }
                catch
                {
                    return;
                }

                var wasCreated = _fileWasCreated[e.FullPath];

                if (wasCreated)
                {
                    if (FileCreationCompleted != null)
                    {
                        FileCreationCompleted(this, e);
                    }
                }
                else
                {
                    if (FileModificationCompleted != null)
                    {
                        FileModificationCompleted(this, e);
                    }
                }

                bool successfullyRemovedKey;
                _fileWasCreated.TryRemove(e.FullPath, out successfullyRemovedKey);

                break;
            }
        }
    }
}