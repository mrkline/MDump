using System;
using System.Collections.Generic;
using System.Text;
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
            ImageCache.ImageCacheTicket ticket;
            Bitmap ret = ImageCache.Instance.CreateCachedImage(filepath, true, out ticket);
            IndividualImageTag tag = new IndividualImageTag(Path.GetFileNameWithoutExtension(filepath),
                ret, mdPath);
            tag.CacheTicket = ticket;
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
            ImageCache.ImageCacheTicket ticket;
            Bitmap ret = ImageCache.Instance.CreateCachedImage(filepath, false, out ticket);
            MergedImageTag tag = new MergedImageTag(Path.GetFileName(filepath), ret,
                          MasterFormatHandler.Instance.LoadMergedImageData(filepath));
            tag.CacheTicket = ticket;
            ret.Tag = tag;
            return ret;
        }
    }
}
