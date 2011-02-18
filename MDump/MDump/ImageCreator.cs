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
        public static Bitmap CreateIndividualImage(string filepath,
            string mdPath)
        {
            byte[] imageBytes = File.ReadAllBytes(filepath);
            MemoryStream ms = new MemoryStream(imageBytes);

            Bitmap ret = new Bitmap(ms);

            //Merger assumes images will be 32-bit argb
            if (ret.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                ret = ret.Clone(new Rectangle(0, 0, ret.Width, ret.Height),
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }

            ret.Tag = new IndividualImageTag(Path.GetFileNameWithoutExtension(filepath), mdPath);
            GC.Collect(); //TEMP to make sure image doesn't disappear
            return ret;
        }

        public static Bitmap CreateMergedImage(string filepath)
        {
            byte[] imageBytes = File.ReadAllBytes(filepath);
            MemoryStream ms = new MemoryStream(imageBytes);

            Bitmap ret = new Bitmap(ms);
            ret.Tag = new MergedImageTag(Path.GetFileName(filepath),
                          MasterFormatHandler.Instance.LoadMergedImageData(filepath));
            GC.Collect(); //TEMP
            return ret;
        }
    }
}
