using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ImageService.Configuration
{
    public class ConfigurationModal : IConfigurationModal
    {
        #region Members
        private XDocument m_doc;            // The Xml Document
        #endregion

        /// <summary>
        ///  The Function Tries To Load The File
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string LoadFile(string path)
        {
            try
            {
                m_doc = XDocument.Load(path);
            }
            catch(Exception e)
            {
                return e.Message;
            }
            return null;
        }

        public IEnumerable<string> this[string path]
        {
            get
            {
                var elements = m_doc.XPathSelectElements(path);     // Getting the Elements
                string[] values = new string[elements.Count()];     // Creating the Values Array

                int index = 0;
                foreach(var element in elements)
                {
                    values[index] = element.Value;                  // Getting the Element Value
                    index++;
                }
                return values;              // Return the Values Array
            }
        }

    }
}
