using System;
using System.Drawing;

namespace MDump
{
    /// <summary>
    /// An exception type thrown by an ImageFormatHandler
    /// </summary>
    class FormatHandlerException : ApplicationException
    {
        public FormatHandlerException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Interface for image merging and splitting images in different formats
    /// </summary>
    interface ImageFormatHandler
    {
        /// <summary>
        /// Gets the name of the format this handler handles.
        /// </summary>
        string FormatName { get; }

        /// <summary>
        /// Gets the extension this format uses
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Checks if the image is a MDump merged image supported by this handler
        /// </summary>
        /// <param name="filepath">File path</param>
        /// <returns>true if the image is indeed an MDump merged image</returns>
        bool SupportsMergedImage(string filepath);

         /// <summary>
        /// Loads MDump data from an image.
        /// Does not test to see if the file is an MDump merged image supported by this handler
        /// </summary>
        /// <param name="filepath">Filename of the merged image</param>
        /// <returns>MDump image data from image</returns>
        string LoadMergedImageData(string filepath);

        /// <summary>
        /// Saves a (assumed merged) image to managed memory
        /// </summary>
        /// <param name="bitmap">Image to save (assumed to be 32-bpp RGBA)</param>
        /// <param name="mdData">MDump data string to save</param>
        /// <param name="compLevel">Compression level to use</param>
        /// <returns>Memory containing saved image</returns>
        byte[] SaveToMemory(Bitmap bitmap, string mdData, MDumpOptions.CompressionLevel compLevel);
    }
}
