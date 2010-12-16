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

    enum MergedCode
    {
        MC_MERGED,
        MC_NOT_MERGED,
        MC_ERROR,
        MC_HAMMER //Completely necessary
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

        /// <summary>
        /// Checks if the image is merged by fist checking if it's a PNG, then if it contains the MDump
        /// metadata.
        /// </summary>
        /// <param name="filename">file to test</param>
        /// <returns>true if the file is an MDump PNG merged image</returns>
        [DllImport("PNGOps.dll")]
        public static extern MergedCode IsMergedImage(string filename);

        /// <summary>
        /// Loads an MDump merged PNG.  Does not test to see if the file is a merged png.  For that see
        /// <see cref="IsMergedImage"/>
        /// </summary>
        /// <param name="filename">filename of the merged image</param>
        /// <param name="bitmapOut">sets this to a pointer to the bitmap data in unmanaged memory</param>
        /// <param name="widthOut"></param>
        /// <param name="heightOut"></param>
        /// <param name="mdDataOut"></param>
        /// <param name="mdDataLenOut"></param>
        /// <returns></returns>
        [DllImport("PNGOps.dll")]
        public static extern ECode LoadMergedImage(string filename, out IntPtr bitmapOut,
            out int widthOut, out int heightOut, out IntPtr mdDataOut, out int mdDataLenOut);
        
        /// <summary>
        /// The magic string that determines if the file we're looking at is an MDump merged image
        /// </summary>
        private const string MagicString = "MDmpMrge";

        [DllImport("PNGOps.dll")]
        public static extern ECode SavePNG(IntPtr bitmap, int width, int height, string filename,
            bool flipRGB, byte[] mdData, int mdDataLen);
    }
}
