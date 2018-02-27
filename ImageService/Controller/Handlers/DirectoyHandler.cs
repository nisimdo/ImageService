using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        #region Constructor
        public DirectoyHandler(IImageController controller, ILoggingService logger)
        {
            m_controller = controller;                      // Setting the Controller
            m_logging = logger;                             // Logging the Messages
        }
        #endregion

        public void StartHandleDirectory(string dirPath)
        {
            // If The Directory Doesn't Exists
            if(!Directory.Exists(dirPath))
            {
                DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(dirPath, 
                    String.Format(MessageInfrastructure.ERROR_DirNotFound, dirPath)));
                return;             // Finishing the Task Of Listening to the file
            }

            m_path = dirPath;       // Storing the Path of Directory

            m_dirWatcher = new FileSystemWatcher();
            m_dirWatcher.Path = dirPath;
            /* Watch for changes in LastAccess and LastWrite times, and
                the renaming of files or directories. */
            m_dirWatcher.NotifyFilter = NotifyFilters.LastWrite
                | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            m_dirWatcher.EnableRaisingEvents = true;             // Setting that the Watcher is active for Events

            m_dirWatcher.Created += new FileSystemEventHandler(OnChanged);

            // m_dirWatcher.Filter = "*.bmp;*.jpg;*.gif;*.png";               // Setting the Filter

            m_logging.Log(String.Format(MessageInfrastructure.INFO_StartDirHandler, dirPath), MessageTypeEnum.INFO);
        }

        #region Events
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            Task task = new Task(() =>
            {
                bool success;           // Indication if the Command Was Successful
                // If The Command is directed to the given Path
                if (e.RequestDirPath.Equals("*") || e.RequestDirPath.Equals(m_path))
                {
                    // If The User Wishes to close the file
                    if(e.CommandID == (int)CommandEnum.CloseCommand)
                    {
                        if (m_dirWatcher != null)
                        {
                            m_dirWatcher.EnableRaisingEvents = false;      // 
                        }
                        DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path,
                            String.Format(MessageInfrastructure.INFO_CloseCommand, m_path)));
                        // Invoking the Server Close
                    }
                    // Logging the Command Being Recieved
                    m_logging.Log(String.Format(MessageInfrastructure.INFO_RecievedCommand, e.CommandID, 
                        String.Join(",", e.Args)), Logging.Modal.MessageTypeEnum.INFO);

                    // Execute command
                    string result = m_controller.ExecuteCommand(e.CommandID, e.Args, out success);

                    MessageTypeEnum type = (success) ? MessageTypeEnum.INFO : MessageTypeEnum.FAIL;

                    m_logging.Log(String.Format(MessageInfrastructure.INFO_SuccessfulCommand, e.CommandID, 
                        String.Join(",", e.Args), result), type);
                }
                
            });
            task.Start();
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            string ext = new FileInfo(e.FullPath).Extension;

            if (Regex.IsMatch(ext, @"\.jpg|\.png|\.gif|\.bmp", RegexOptions.IgnoreCase))
            {
                // Specify what is done when a file is changed, created, or deleted.
                OnCommandRecieved(this, new CommandRecievedEventArgs((int)CommandEnum.NewFileCommand,
                new string[] { e.FullPath }, m_path));
            }
        }
        #endregion
    }
}
