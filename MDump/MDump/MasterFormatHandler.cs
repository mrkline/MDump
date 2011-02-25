using System;
using System.Collections.Generic;
using System.Drawing;

namespace MDump
{
    /// <summary>
    /// Uses a collection of all other format handlers to handle all formats
    /// </summary>
    class MasterFormatHandler : ImageFormatHandler
    {
        private const string unsupportedFormat = "The program could not find"
            + " a handler that supports the given image format.";

        private Dictionary<string, ImageFormatHandler> handlers;

        #region Singleton
        private static MasterFormatHandler _instance = null;
        public static MasterFormatHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MasterFormatHandler();
                }
                return _instance;
            }
        }

        private MasterFormatHandler()
        {
            handlers = new Dictionary<string, ImageFormatHandler>();

            ImageFormatHandler handler = new PNGHandler();

            handlers.Add(handler.FormatName, handler);
            handler = new JPEGHandler();
            handlers.Add(handler.FormatName, handler);
        }
        #endregion

        /// <summary>
        /// Gets the names of supported formats (for the options menu to propagate a drop-down)
        /// </summary>
        public IEnumerable<string> SupportedFormatNames
        {
            get { return handlers.Keys; }
        }

        /// <summary>
        /// Gets the extensions of supported formats (for finding merge files)
        /// </summary>
        public IEnumerable<string> SupportedExtensions
        {
            get
            {
                List<string> extensions = new List<string>(handlers.Count);
                foreach (ImageFormatHandler handler in handlers.Values)
                {
                    extensions.Add(handler.Extension);
                }
                return extensions;
            }
        }

        /// <summary>
        /// Gets the extension of the current merge format, as set in the options
        /// </summary>
        public string Extension
        {
            get
            {
                if (!handlers.ContainsKey(MDumpOptions.Instance.MergeFormat))
                {
                    throw new NotSupportedException(unsupportedFormat);
                }
                return handlers[MDumpOptions.Instance.MergeFormat].Extension;
            }
        }


        /// <summary>
        /// Checks if the image is a MDump merged image.
        /// </summary>
        /// <param name="filepath">File path</param>
        /// <returns>true if the image is indeed an MDump merged image</returns>
        public bool SupportsMergedImage(string filepath)
        {
            foreach(ImageFormatHandler handler in handlers.Values)
            {
                if (handler.SupportsMergedImage(filepath))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the name of the format this handler handles.
        /// </summary>
        public string FormatName { get { return "N/A (Master Format Handler)"; } }

        /// <summary>
        /// Loads MDump data from an image.
        /// Finds correct image handler and uses it
        /// </summary>
        /// <param name="filepath">Filename of the merged image</param>
        /// <returns>MDump image data from image</returns>
        public string LoadMergedImageData(string filename)
        {
            foreach (ImageFormatHandler handler in handlers.Values)
            {
                if(handler.SupportsMergedImage(filename))
                {
                    return handler.LoadMergedImageData(filename);
                }
            }
            throw new NotSupportedException(unsupportedFormat);
        }

        /// <summary>
        /// Saves a (assumed merged) image to managed memory
        /// </summary>
        /// <param name="bitmap">Image to save (assumed to be 32-bpp RGBA)</param>
        /// <param name="mdData">MDump data string to save</param>
        /// <param name="compLevel">Compression level to use</param>
        /// <returns>Memory containing saved image</returns>
        public byte[] SaveToMemory(Bitmap bitmap, string mdData,
            MDumpOptions.CompressionLevel compLevel)
        {
            if(!handlers.ContainsKey(MDumpOptions.Instance.MergeFormat))
            {
                throw new NotSupportedException(unsupportedFormat);
            }
            return handlers[MDumpOptions.Instance.MergeFormat].SaveToMemory(bitmap, mdData, compLevel);
        }
    }
}
