using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;

namespace MDump
{
    class JPEGHandler : ImageFormatHandler
    {
        private const string author = "MDump";
        private const string magicString = "MDmpMrge";

        public string FormatName
        {
            get { return "JPEG"; }
        }

        public bool SupportsMergedImage(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
               BitmapMetadata meta = new JpegBitmapDecoder(fs,
                    BitmapCreateOptions.DelayCreation, BitmapCacheOption.None).Metadata;

                if (meta.Author.Count == 1 && meta.Author[0] == author
                    && meta.Comment.StartsWith(magicString, StringComparison.InvariantCulture))
                {
                    return true;
                }

                return false;
            }
        }

        public string LoadMergedImageData(string filepath)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                BitmapMetadata meta = new JpegBitmapDecoder(fs,
                    BitmapCreateOptions.DelayCreation, BitmapCacheOption.None).Metadata;

                return meta.Comment.Substring(magicString.Length);
            }
        }

        public byte[] SaveToMemory(System.Drawing.Bitmap bitmap, string mdData, MDumpOptions.CompressionLevel compLevel)
        {
            byte[] buff;
            using (MemoryStream ms = new MemoryStream())
            {
                JpegBitmapEncoder enc = new JpegBitmapEncoder();
                //TODO: Save bitmap using enc
                buff = ms.GetBuffer();
            }

            for (int c = buff.Length - 1; c > 0; --c)
            {
                if (buff[c] != 0)
                {
                    byte[] trimmed = new byte[c];
                    Array.Copy(buff, trimmed, c);
                    return trimmed;
                }
            }
            return buff;
        }
    }
}
