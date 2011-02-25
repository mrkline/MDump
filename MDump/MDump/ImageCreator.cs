using System.Drawing;
using System.IO;

namespace MDump
{
    /// <summary>
    /// Creates and images for use by the rest of MDump 
    /// </summary>
    static class ImageCreator
    {
        /// <summary>
        /// Create a Bitmap representing an individual image
        /// </summary>
        /// <param name="filepath">File path of the individual image</param>
        /// <param name="mdPath">MDump directory path for the image</param>
        /// <returns>A Bitmap tagged with an IndividualImageTag</returns>
        public static Bitmap CreateIndividualImage(string filepath,
            string mdPath)
        {
            Bitmap ret = CreateMemoryBitmap(filepath);
            IndividualImageTag tag = new IndividualImageTag(Path.GetFileNameWithoutExtension(filepath),
                ret, mdPath);
            ret.Tag = tag;
            return ret;
        }

        /// <summary>
        /// Create a Bitmap representing an merged image
        /// </summary>
        /// <param name="filepath">File path of the merged image</param>
        /// <returns>A Bitmap tagged with a MergedImageTag</returns>
        public static Bitmap CreateMergedImage(string filepath)
        {
            Bitmap ret = CreateMemoryBitmap(filepath);
            MergedImageTag tag = new MergedImageTag(Path.GetFileName(filepath), ret,
                          MasterFormatHandler.Instance.LoadMergedImageData(filepath));
            ret.Tag = tag;
            return ret;
        }

        /// <summary>
        /// Create a Bitmap tied to managed memory instead of a file itself
        /// </summary>
        /// <returns>Memory-based bitmap</returns>
        private static Bitmap CreateMemoryBitmap(string filepath)
        {
            return new Bitmap(new MemoryStream(File.ReadAllBytes(filepath)));
        }
    }
}
