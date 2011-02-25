using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MDump
{
    class JPEGHandler : ImageFormatHandler
    {
        private const string magicString = "MDmpMrge";

        public string FormatName
        {
            get { return "JPEG"; }
        }

        public bool SupportsMergedImage(string filepath)
        {
            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    BitmapMetadata meta = new JpegBitmapDecoder(fs,
                         BitmapCreateOptions.DelayCreation, BitmapCacheOption.None).Metadata;

                    if (meta.Comment.StartsWith(magicString, StringComparison.InvariantCulture))
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch
            {
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
                
                //HACK: We're jumping in and out of native handles to convert the Bitmap to a BitmapSource
                IntPtr hBitmap = bitmap.GetHbitmap();

                try
                {
                    enc.Frames.Add(BitmapFrame.Create(
                        System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions())));
                }
                finally
                {
                    DeleteObject(hBitmap);
                }

                switch (compLevel)
                {
                    case MDumpOptions.CompressionLevel.Low:
                        enc.QualityLevel = 90;
                        break;

                    case MDumpOptions.CompressionLevel.Medium:
                        enc.QualityLevel = 80;
                        break;

                    case MDumpOptions.CompressionLevel.High:
                        enc.QualityLevel = 70;
                        break;

                    case MDumpOptions.CompressionLevel.Maximum:
                        enc.QualityLevel = 60;
                        break;
                }

                enc.Metadata.Comment = magicString + mdData;

                enc.Save(ms);
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

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}
