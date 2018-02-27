using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure
{
    public static class MessageInfrastructure
    {
        public static readonly string ERROR_DirNotFound = "The Directory {0} Doesn't exist in the System";
        public static readonly string ERROR_FileNotFound = "The File {0} Doesn't exist in the System ";
        public static readonly string ERROR_CommandNotFound = "The Command {0} Wasn't found in the System";
        public static readonly string ERROR_DirCreation = "The Dir {0} Could not be created. Reason: {1}";
        public static readonly string ERROR_MoveFile = "The File {0} Could not be moved to path {1}. Reason: {2}";
        public static readonly string ERROR_ThumbCreation = "The Thumbnail {0} Could not be created. Reason: {1}";

        public static readonly string INFO_StartDirHandler = "Starting Directory Handler For Directory: {0}";
        public static readonly string INFO_RecievedCommand = "Got command ID: {0} Args: {1}";
        public static readonly string INFO_SuccessfulCommand = "Message Code {0} Args:[{1}] Was Sucessful with Result {2}";

        // Close Connection
        public static readonly string INFO_CloseCommand = "The User Has Closed The Handler on the Path: {0}";

        // Add File
        public static readonly string INFO_AddFileCommandSucc = "The File {0} was Sucessfuly Added to The Path {1}";
        public static readonly string ERROR_AddFileCommandFail = "The File {0} couldn't have been added. Reason: {1}";
    }
}
