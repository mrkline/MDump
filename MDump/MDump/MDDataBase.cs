
namespace MDump
{
    /// <summary>
    /// Contains information shared by the generation and retrieval of the MDump data stored in merged images
    /// </summary>
    abstract class MDDataBase
    {
        #region Character Constants
        /// <summary>
        /// Separates pieces of data in the MDump data buffer
        /// </summary>
        protected const char separator = '\n';
        /// <summary>
        /// Acts as a sub-separator in the MDump data buffer
        /// </summary>
        protected const char subSeparator = '*';
        /// <summary>
        /// Indicates that the following line contains how many images are contained in this merged image
        /// </summary>
        protected const char numImagesIndicator = 'n';
        /// <summary>
        /// Indicates that the following line contains data on how to split an image from this merged image
        /// </summary>
        protected const char imageIndicator = 'i';
        /// <summary>
        /// Indicates that the following line contains a directory where the following split images should be placed
        /// </summary>
        protected const char directoryIndicator = 'd';
        #endregion
    }
}
