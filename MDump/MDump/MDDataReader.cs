using System;
using System.Drawing;

namespace MDump
{
    /// <summary>
    /// A class for reading MDump data from a buffer.
    /// Reads back the data and provides information to split the merged image
    /// </summary>
    class MDDataReader : MDDataBase
    {
        #region String Constants
        private const string notNumImagesTokenMsg 
            = "The provided token does not contain the number of images in this merge.";
        private const string notImageTokenMsg = "The provided token does not contain image information.";
        private const string notDirTokenMsg = "The provided token does not contain directory information.";
        #endregion

        /// <summary>
        /// Used as a return code to indicate what a given line contains
        /// </summary>
        public enum TokenType
        {
            NumImages,
            Directory,
            Image,
            Unknown
        }

        /// <summary>
        /// Decodes the MDump data buffer and splits it into information tokens
        /// </summary>
        /// <param name="data">The MDump data buffer recovered from a merged image</param>
        /// <returns>Tokens containing information about the images to split and directories to place them in</returns>
        public static string[] SplitData(string data)
        {
            return data.Split(separator);
        }

        /// <summary>
        /// Gets the type of information a token returned from <see cref="SplitData"/> contains.
        /// </summary>
        /// <param name="token">Token returned from DecodeAndSplitData</param>
        /// <returns>The type of information token contains</returns>
        public static TokenType GetTokenType(string token)
        {
            switch (token[0])
            {
                case numImagesIndicator:
                    return TokenType.NumImages;
                    
                case directoryIndicator:
                    return TokenType.Directory;

                case imageIndicator:
                    return TokenType.Image;

                default:
                    return TokenType.Unknown;
            }
        }

        /// <summary>
        /// Gets the number of images in the merged image from the appropriate token
        /// </summary>
        /// <param name="token">Token containint the number of images in the merged image</param>
        /// <returns>The number of images in the merged image</returns>
        public static int GetNumImages(string token)
        {
            string[] tokens = token.Split(subSeparator);
            if (tokens[0] != numImagesIndicator.ToString())
            {
                throw new ArgumentException(notNumImagesTokenMsg);
            }
            return Convert.ToInt32(tokens[1]);
        }

        /// <summary>
        /// Gets the directory to add from an appropriate token
        /// </summary>
        /// <param name="token">Token containing a directory in which to place split images</param>
        /// <returns>A directory in which to place split images described in following tokens</returns>
        public static string GetDirectory(string token)
        {
            string[] tokens = token.Split(subSeparator);
            if (tokens[0] != directoryIndicator.ToString())
            {
                throw new ArgumentException(notDirTokenMsg);
            }
            return tokens[1];
        }

        /// <summary>
        /// Splits an image from the merged image, given an appropriate token
        /// </summary>
        /// <param name="token">Token containing split data</param>
        /// <param name="mergedImage">The merged image to split from</param>
        /// <returns>The split image with an its name tagged on</returns>
        public static Bitmap GetSplitImage(string token, Bitmap mergedImage)
        {
            string[] tokens = token.Split(subSeparator);
            if (tokens[0] != imageIndicator.ToString())
            {
                throw new ArgumentException(notImageTokenMsg);
            }
            int x = Convert.ToInt32(tokens[2]);
            int y = Convert.ToInt32(tokens[3]);
            int width = Convert.ToInt32(tokens[4]);
            int height = Convert.ToInt32(tokens[5]);
            Rectangle r = new Rectangle(x, y, width, height);
            Bitmap ret = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(ret))
            {
                g.DrawImage(mergedImage, 0, 0, r, GraphicsUnit.Pixel);
            }
            ret.Tag = tokens[1];
            return ret;
        }
    }
}
