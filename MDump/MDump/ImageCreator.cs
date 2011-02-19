using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace MDump
{
    //TODO: Being replaced by ImageCache and ImagePackage combo. 

    /// <summary>
    /// Creates and images for use by the rest of MDump 
    /// </summary>
    static class ImageCreator
    {
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
