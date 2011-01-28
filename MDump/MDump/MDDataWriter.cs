using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace MDump
{
    /// <summary>
    /// A class for writing MDump data to a binary stream.
    /// Strings are written using Encoding.GetBytes to avoid the string length prefix
    /// that the BinaryWriter.Write overload for strings uses
    /// The number of images should be written first, followed by directory and image info.
    /// </summary>
    class MDDataWriter : MDDataBase
    {
        private BinaryWriter bw;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="binWriter">The binary writer to write the MDump data to</param>
        public MDDataWriter(BinaryWriter binWriter)
        {
            bw = binWriter;
        }

        /// <summary>
        /// Writes the number of images in the merged imge
        /// </summary>
        /// <param name="numImages">Number of images in the merged image this data represents</param>
        public void WriteNumImages(int numImages)
        {
            bw.Write(Encoding.GetBytes(numImagesIndicator + subSeparator
                + numImages.ToString() + separator));
        }

        /// <summary>
        /// Write a path to insert following images into
        /// </summary>
        /// <param name="dir">MDump path to write</param>
        public void WriteDirectory(string dir)
        {
            string write = directoryIndicator + subSeparator + dir + separator;
            bw.Write(Encoding.GetBytes(write));
        }

        /// <summary>
        /// Write information about an image so MDump can split it from this merged image
        /// </summary>
        /// <param name="name">The name of the image, extracted from its tag</param>
        /// <param name="x">X-coordinate of the upper left corner of the image in the merged image</param>
        /// <param name="y">Y-coordinate of the upper left corner of the image in the merged image</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        public void WriteImageData(string name, int x, int y, int width, int height)
        {
            string write = imageIndicator + subSeparator + name + subSeparator
                                    + x.ToString() + subSeparator + y.ToString() + subSeparator
                                    + width.ToString() + subSeparator + height.ToString() + separator;
            bw.Write(Encoding.GetBytes(write));
        }

        /// <summary>
        /// Trims empty bytes from a byte buffer
        /// </summary>
        /// <param name="buff">Buffer to trim</param>
        /// <returns>Trimmed copy of buff</returns>
        public static byte[] TrimBuffer(byte[] buff)
        {
            byte[] ret;

            for (int c = buff.Length - 1; c > 0; --c)
            {
                if (buff[c] != 0)
                {
                    ret = new byte[c];
                    Array.Copy(buff, ret, c);
                    return ret;
                }
            }
            return buff;
        }
    }
}
