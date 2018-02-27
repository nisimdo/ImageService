using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Modal;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Configuration;
using ImageService.Infrastructure;

namespace ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class ImageService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private ImageServer m_imageServer;          // The Image Server

        private Dictionary<MessageTypeEnum, EventLogEntryType> mapping = new Dictionary<MessageTypeEnum, EventLogEntryType>()
        {
            {MessageTypeEnum.INFO, EventLogEntryType.Information},
            {MessageTypeEnum.FAIL,EventLogEntryType.Error},
            {MessageTypeEnum.WARNING, EventLogEntryType.Warning }
        };

        public ImageService(string[] args)
        {
            InitializeComponent();
            //string eventSourceName = "ImageServiceSource";
            //string logName = "ImageServiceLog";
            //if (args.Count() > 0)
            //{
            //    eventSourceName = args[0];
            //}
            //if (args.Count() > 1)
            //{
            //    logName = args[1];
            //}

            //if (!System.Diagnostics.EventLog.SourceExists("ImageServiceSource"))
            //{
            //    System.Diagnostics.EventLog.CreateEventSource(
            //        "ImageServiceSource", "ImageServiceLog");
            //}

            eventLog1.Source = ConfigurationManager.AppSettings[ConfigurationInfrastructure.Node_SourceName];
            eventLog1.Log = ConfigurationManager.AppSettings[ConfigurationInfrastructure.Node_LogName];

            IImageServiceModal modal = new ImageServiceModal(ConfigurationManager.AppSettings[ConfigurationInfrastructure.Node_OutputDir], 
                int.Parse(ConfigurationManager.AppSettings[ConfigurationInfrastructure.Node_ThumbnailSize]));  // Creating the Service
            IImageController controller = new ImageController(modal);
            ILoggingService logging = new LoggingService();         // Creating The Logging Service
            logging.MessageRecieved += OnMessageRecieved;           // Adding the Upon Message Recieved

            m_imageServer = new ImageServer(controller, logging);   // Creating The Server

        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Starting the Image Service");

            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            string[] dirs = ConfigurationManager.AppSettings[ConfigurationInfrastructure.Node_Handler].Split(';');        // Getting the Node Handlers
            
            foreach(var dir in dirs)
            {
                m_imageServer.StartDirectoryHandler(dir);   // Starting a Hanlder on the File
            }

            // Code Here
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);


        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Stoping Image Service");
            m_imageServer.Stop();           // Stoping The Service
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);
        }

        #region Logging Event
        protected void OnMessageRecieved(object sender, Logging.Modal.MessageRecievedEventArgs e)
        {
            eventLog1.WriteEntry(e.Message, mapping[e.Status]);     // Adding the Log To the entry
        }
        #endregion
    }
}
