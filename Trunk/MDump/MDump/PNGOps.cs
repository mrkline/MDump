using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MDump
{
    public enum ECode
    {
        EC_SUCCESS,
        EC_BAD_ARGS,
        EC_INIT_FAILURE,
        EC_IO_FAILURE,
        EC_HEADER_FAILURE,
        EC_ALLOC_FAILURE,
        EC_WRITE_IMAGE_FAILURE,
        EC_WRITE_INFO_FAILURE,
        EC_WRITE_END_FAILURE
    };

    /// <summary>
    /// Provides access to PNGOPS.dll
    /// </summary>
    class PNGOps
    {
        /// <summary>
        /// A simple wrapper around free() to release unmanaged memory once we've copied data out of it
        /// </summary>
        /// <param name="ptr">Pointer to the unmanaged memory</param>
        [DllImport("PNGOps.dll")]
        public static extern void FreeBitmap(IntPtr ptr);

        [DllImport("PNGOps.dll")]
        public static extern bool IsPNG(string filename);

        [DllImport("PNGOps.dll")]
        public static extern ECode LoadPNG(string filename, out IntPtr bitmapOut, out int widthOut, out int heightOut);
        
        /// <summary>
        /// The magic string that determines if the file we're looking at is an MDump merged image
        /// </summary>
        private const string MagicString = "MDmpMrge";

        [DllImport("PNGOps.dll")]
        public static extern ECode SavePNG(IntPtr bitmap, int width, int height, string filename,
            bool flipRGB, byte[] mdData, int mdDataLen);
    }
}
