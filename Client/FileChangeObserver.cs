﻿using System;
using System.Windows;
using System.Windows.Forms;
using ToDoLib;
using System.IO;

using System.Windows.Input;
using System.Threading;

namespace Client
{
    class FileChangeObserver
    {
        private string _filename = "";
        private FileSystemWatcher _watcher;

		public delegate void FileChanged();

		public event FileChanged OnFileChanged;

        public void ObserveFile(string filename)
        {
            if (User.Default.AutoRefresh == false)
            {
                if (_watcher != null)
                {
                    _watcher.EnableRaisingEvents = false;
                    _watcher.Dispose();
                    _watcher = null;
                    _filename = "";
                }
                return;
            }

            if (_filename != filename)
            {
                _filename = filename;
                if (_watcher != null)
                {
                    _watcher.Dispose();
                }
                _watcher = new FileSystemWatcher();
                _watcher.Path = System.IO.Path.GetDirectoryName(filename);
                _watcher.Filter = System.IO.Path.GetFileName(filename);
                _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;

                // Add event handlers.
                _watcher.Changed += FileChange;

                // Begin watching.
                _watcher.EnableRaisingEvents = true;
            }
            else if (User.Default.AutoRefresh == false && _watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
            }
        }

        private void FileChange(object source, FileSystemEventArgs e)
        {
			Thread.Sleep(1000); // give the writing app time to release its lock
		
			if (OnFileChanged != null)
				OnFileChanged();
        }
    }
}
