using System;
using System.Collections.Generic;
using System.Text;

namespace MDump
{
    /// <summary>
    /// Contains information shared by the generation and retrieval of the MDump data stored in merged images
    /// </summary>
    abstract class MDDataBase
    {
        //Separates pieces of data in the MDump data buffer
        protected const char separator = '\n';
        //Acts as a sub-separator in the MDump data buffer
        protected const char subSeparator = ';';
        //Indicates that the following data is data on how to split an image from this merged image
        protected const char imageIndicator = 'i';
        //Indicates that the following data is a directory where the following split images should be put 
        protected const char directoryIndicator = 'd';

        public static Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
