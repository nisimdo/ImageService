using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        #endregion

        public ImageServiceModal(string outputFolder)
        {
            m_OutputFolder = outputFolder;          // Storing the output folder
            CreateDirectory(m_OutputFolder);        // Creating the Output Folder
        }

        //  return String.Format(MessageInfrastructure.ERROR_DirCreation, m_OutputFolder, e.Message);
        public String CreateDirectory(string directoryPath)
        {
            // Checking if the Directory Doesn't Exist
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(directoryPath);
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }
                catch (Exception e)
                {
                    return String.Format(MessageInfrastructure.ERROR_DirCreation, directoryPath, e.Message);
                }
            }
            return null;                // Return True if the Directory Already Exists
        }

        public string AddFile(string path, out bool result)
        {
            WaitToDownload(path);           // Waiting for the File To Be Downloaded

            result = false;         // Setting that the Creation Has Failed Until Proven Otherwise
            DateTime creation = GetFileDate(path);      // Getting the Creation Date of the Image

            string subDirPath = Path.Combine(creation.Year.ToString(), creation.Month.ToString());
            string newFilePath = Path.Combine(m_OutputFolder, subDirPath);      // Getting the Path of the Directory that need to contain the File

            string fileName = RenameFile(path, newFilePath);  // Getting the Name of the File

            // Checking if the Path Exists
            if (!Directory.Exists(newFilePath))
            {
                string createResult = CreateDirectory(newFilePath);     // Creating The Directory To Store The Picture In
                if (createResult != null)
                {
                    return createResult;    // Return the Error in The Craetion
                }
            }

            newFilePath = Path.Combine(newFilePath, fileName);          // Creating the New File Path
            try
            {
                File.Move(path, newFilePath);       // Moving the File
            }
            catch (Exception e)
            {
                return String.Format(MessageInfrastructure.ERROR_MoveFile, path, newFilePath, e.Message);
            }

            string createThumbnailResult = CreateThumbnail(subDirPath, newFilePath);           // Creating the Thumbnail
            if (createThumbnailResult != null)
            {
                return String.Format(MessageInfrastructure.ERROR_ThumbCreation, newFilePath, createThumbnailResult);        // Return the Error in the Creation
            }

            result = true;      // Setting that the Addition Was Sucessful
            return newFilePath;     // Return that the Command Wasd Succesful
        }

        #region Assistance Functions
        private DateTime GetFileDate(string filePath)
        {
            FileInfo info = new FileInfo(filePath);     // Creating the File Info for the File

            return info.CreationTime;                   // Return the Creation Time Of The File
        }

        /// <summary>
        /// The Function Gets the Image Path On which to create the image Path
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private string CreateThumbnail(string subDirPath, string imagePath)
        {
            string fileName = new FileInfo(imagePath).Name;        // Getting the File Name

            string thumbnailFolder = Path.Combine(m_OutputFolder, "thumbnail", subDirPath);     // Getting the Thumbnail Folder

            if (!Directory.Exists(thumbnailFolder))
            {
                string createResult = CreateDirectory(thumbnailFolder);     // Creating The Directory To Store The Picture In
                if (createResult != null)
                {
                    return createResult;    // Return the Error in The Craetion
                }
            }

            Image image = Image.FromFile(imagePath);
            Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

            fileName = Path.ChangeExtension(fileName, "thumb");     // Changing the Extension
            thumb.Save(Path.Combine(thumbnailFolder, fileName));     // Saving The Thumbnail


            return null;            // The Function Worked as Expected
        }

        private string RenameFile(string filePath, string dirPath)
        {
            int count = 1;
            string fileOrigName = Path.GetFileName(filePath);
            string fileNameOnly = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string tempFileName = fileNameOnly;

            string newFullPath = Path.Combine(dirPath, fileOrigName);       // Setting the Full Path Of The File

            while (File.Exists(newFullPath))
            {
                tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(dirPath, tempFileName + extension);
            }
            return tempFileName + extension;
        }

        private void WaitToDownload(string file)
        {
            FileInfo fInfo = new FileInfo(file);        // Creating the file Info
            while (IsFileLocked(fInfo))
            {
                Thread.Sleep(500);
            }
        }


        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
        #endregion
    }
}
