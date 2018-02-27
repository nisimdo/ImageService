using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Infrastructure
{
    public static class ConfigurationInfrastructure
    {
        public static readonly string Node_Handler = "Handler";                 // The Handler Node That Contains the Path To Handle
        public static readonly string Node_OutputDir = "OutputDir";             // The Directory of the output
        public static readonly string Node_SourceName ="SourceName";             // The Source Name of log
        public static readonly string Node_LogName = "LogName";             // The Source Name of log
    }
}
