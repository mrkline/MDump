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
            Bitmap ret = CreateUntaggedImage(filepath, true);
            ret.Tag = new IndividualImageTag(Path.GetFileNameWithoutExtension(filepath), mdPath);
            GC.Collect(); //TEMP to make sure image doesn't disappear
            return ret;
        }

        public static Bitmap CreateMergedImage(string filepath)
        {
            Bitmap ret = CreateUntaggedImage(filepath, false);
            ret.Tag = new MergedImageTag(Path.GetFileName(filepath),
                          MasterFormatHandler.Instance.LoadMergedImageData(filepath));
            GC.Collect(); //TEMP
            return ret;
        }

        public static Bitmap CreateUntaggedImage(string filepath, bool force32BitARGB)
        {
            byte[] imageBytes = File.ReadAllBytes(filepath);
            byte[] copiedBytes = new byte[imageBytes.LongLength];
            imageBytes.CopyTo(copiedBytes, 0);
            Bitmap ret;
            using (MemoryStream ms = new MemoryStream(copiedBytes))
            {
                ret = new Bitmap(ms);

                //Merger assumes images will be 32-bit argb
                if (force32BitARGB
                    && ret.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    ret = ret.Clone(new Rectangle(0, 0, ret.Width, ret.Height),
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }
            }
            return ret;
        }
    }
}
