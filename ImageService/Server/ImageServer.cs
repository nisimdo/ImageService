using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        #region Constructor
        public ImageServer(IImageController controller, ILoggingService logger)
        {
            m_controller = controller;          // Setting the Image Controller
            m_logging = logger;                 // Storing the Logger               
        }
        #endregion

        #region Start Doc 
        /// <summary>
        /// The Function Starts the Directory Service Until Directed Otherwise
        /// </summary>
        #endregion
        public void StartDirectoryHandler(string path)
        {
            IDirectoryHandler directory = new DirectoyHandler(m_controller, m_logging);        // Creating the Directory Handler for the Dir
            directory.DirectoryClose += OnDirectoryClose;               // Adding the Event Handler for when the Directory is closed
            CommandRecieved += directory.OnCommandRecieved;             // Subsribing to the Command Recieve Event
            directory.StartHandleDirectory(path);                       // Start Handling the Folder
        }


        #region Stop Doc 
        /// <summary>
        /// The Function Closes the Services with all it's Directories
        /// </summary>
        #endregion
        public void Stop()
        {
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null ,"*"));        // Closing all the Directories
        }

        #region Event Handlers

        private void OnDirectoryClose(object sender, Modal.DirectoryCloseEventArgs e)
        {
            IDirectoryHandler handler = sender as IDirectoryHandler;            // Casting the Handler
            CommandRecieved -= handler.OnCommandRecieved;                       // Unsubscribing the Command

            // Notify To The Logger
            m_logging.Log(e.Message, Logging.Modal.MessageTypeEnum.INFO);              // Notifying about the Logger the reason for closing the directory     
        }
        #endregion
    }
}
