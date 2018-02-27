using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Configuration
{
    public interface IConfigurationModal
    {
        string LoadFile(string path);                 // Loading the Config File           
        IEnumerable<string> this[string path] { get; }       // The Indexer of The Configuration Service
    }
}
