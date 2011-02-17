using System;
using System.Collections.Generic;
using System.Text;

namespace MDump
{
    class JPEGHandler : ImageFormatHandler
    {
        public string FormatName
        {
            get { return "JPEG"; }
        }

        public bool SupportsMergedImage(string filepath)
        {
            throw new NotImplementedException();
        }

        public byte[] LoadMergedImageData(string filepath)
        {
            throw new NotImplementedException();
        }

        public byte[] SaveToMemory(System.Drawing.Bitmap bitmap, byte[] mdData, MDumpOptions.CompressionLevel compLevel)
        {
            throw new NotImplementedException();
        }
    }
}
