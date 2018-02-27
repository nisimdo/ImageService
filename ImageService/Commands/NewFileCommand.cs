using ImageService.Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal m_modal;

        public NewFileCommand(IImageServiceModal modal)
        {
            m_modal = modal;            // Storing the Modal
        }

        public string Execute(string[] args, out bool result)
        {
            string newPath = m_modal.AddFile(args[0] ,out result);
            if (result)
            {
                return String.Format(MessageInfrastructure.INFO_AddFileCommandSucc, args[0], newPath);          // Return The Message
            }
            return newPath;          // Return The Message
        }
    }
}
