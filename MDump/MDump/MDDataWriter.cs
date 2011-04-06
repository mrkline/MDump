
namespace MDump
{
    /// <summary>
    /// A class for writing MDump data to a string.
    /// The number of images should be written first, followed by directory and image info.
    /// When writing is complete, the generated string can be retrieved from the MDumpData property.
    /// </summary>
    class MDDataWriter : MDDataBase
    {
        private string mdData;
        /// <summary>
        /// Gets MDump data written by this object
        /// </summary>
        public string MDumpData { get { return mdData; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public MDDataWriter()
        {
            mdData = string.Empty;
        }

        /// <summary>
        /// Writes the number of images in the merged imge
        /// </summary>
        /// <param name="numImages">Number of images in the merged image this data represents</param>
        public void WriteNumImages(int numImages)
        {
            mdData += numImagesIndicator + subSeparator.ToString()
                + numImages.ToString() + separator.ToString();
        }

        /// <summary>
        /// Write a path to insert following images into
        /// </summary>
        /// <param name="dir">MDump path to write</param>
        public void WriteDirectory(string dir)
        {
            mdData += directoryIndicator + subSeparator.ToString() + dir + separator.ToString();
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
            mdData += imageIndicator.ToString() + subSeparator.ToString() + name + subSeparator.ToString()
                                    + x.ToString() + subSeparator.ToString() + y.ToString() + subSeparator.ToString()
                                    + width.ToString() + subSeparator.ToString() + height.ToString() + separator.ToString();
        }
    }
}
