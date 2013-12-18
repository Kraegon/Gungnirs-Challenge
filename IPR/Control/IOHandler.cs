using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows;
using System.Xml.Linq;
 

namespace IPR.Control
{
    class IOHandler
    {
        public XElement XMLTree;

        public IOHandler()
        {

        }

        /// <summary>
        /// Initializes the current XML list
        /// Still work in progress
        /// </summary>
        public void Initialize()
        {
            //TODO: Implement a way to read from a xml file
            XMLTree = new XElement("Player", new XElement("Score", 10));
        }
    }
}
