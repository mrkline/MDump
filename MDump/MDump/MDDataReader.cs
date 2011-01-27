using System;
using System.Collections.Generic;
using System.Text;

namespace MDump
{
    /// <summary>
    /// A class for reading MDump data from a buffer.
    /// Reads back the data and provides information to split the merged image
    /// </summary>
    class MDDataReader : MDDataBase
    {
        /// <summary>
        /// Used as a return code to indicate what a given line contains
        /// </summary>
        public enum LineType
        {
            NumImages,
            Directory,
            Image,
            Unknown
        }

        public static string[] DecodeAndSplitData(byte[] data)
        {
            return Encoding.GetString(data).Split(separator);
        }

        public static LineType GetTokenType(string line)
        {
            switch (line[0])
            {
                case numImagesIndicator:
                    return LineType.NumImages;
                    
                case directoryIndicator:
                    return LineType.Directory;

                case imageIndicator:
                    return LineType.Image;

                default:
                    return LineType.Unknown;
            }
        }


    }
}
