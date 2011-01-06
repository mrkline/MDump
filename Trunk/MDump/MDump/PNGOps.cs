using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MDump
{
    /// <summary>
    /// Error codes for PNGOps functions
    /// </summary>
    public enum ECode
    {
        /// <summary>
        /// Function was successful
        /// </summary>
        EC_SUCCESS,
        /// <summary>
        /// Bad or incorrect arguments passed to the function
        /// </summary>
        EC_BAD_ARGS,
        /// <summary>
        /// Incorrect image format
        /// </summary>
        EC_BAD_FORMAT,
        /// <summary>
        /// Failure to initialize libpng structs
        /// </summary>
        EC_INIT_FAILURE,
        /// <summary>
        /// Failure to perform I/O such as opening the file, etc.
        /// </summary>
        EC_IO_FAILURE,
        /// <summary>
        /// Failure to write image header
        /// </summary>
        EC_HEADER_FAILURE,
        /// <summary>
        /// Failure to allocate memory
        /// </summary>
        EC_ALLOC_FAILURE,
        /// <summary>
        /// Failure to read or write the bitmap image data
        /// </summary>
        EC_RW_IMAGE_FAILURE,
        /// <summary>
        /// Failure to read or write MDump info
        /// </summary>
        EC_RW_INFO_FAILURE,
        /// <summary>
        /// Failure to read/write the end of the file
        /// </summary>
        EC_RW_END_FAILURE
    };

    /// <summary>
    /// Return code for IsMergedImage
    /// </summary>
    enum MergedCode
    {
        /// <summary>
        /// Image in question is an MDump merged image
        /// </summary>
        MC_MERGED,
        /// <summary>
        /// Image in question is not an MDump merged image
        /// </summary>
        MC_NOT_MERGED,
        /// <summary>
        /// An error occurred and it cannot be determined
        /// if image in question is an MDump merged image
        /// </summary>
        MC_ERROR,
        /// <summary>
        /// Can't touch this
        /// </summary>
        MC_HAMMER
    };

    /// <summary>
    /// Provides access to PNGOPS.dll
    /// </summary>
    class PNGOps
    {
        private const string dllName = "PNGOps.dll";

        /// <summary>
        /// A simple wrapper around free() to release unmanaged memory once we've copied data out of it
        /// </summary>
        /// <param name="ptr">Pointer to the unmanaged memory</param>
        [DllImport(dllName)]
        public static extern void FreeUnmanagedData(IntPtr ptr);

        /// <summary>
        /// Checks if the image is merged by fist checking if it's a PNG, then if it contains the MDump
        /// metadata.
        /// </summary>
        /// <param name="filename">file to test</param>
        /// <returns>true if the file is an MDump PNG merged image</returns>
        [DllImport(dllName)]
        public static extern MergedCode IsMergedImage(string filename);

        /// <summary>
        /// Loads an MDump merged PNG.  Does not test to see if the file is a merged png.  For that see
        /// <see cref="IsMergedImage"/>
        /// </summary>
        /// <param name="filename">filename of the merged image</param>
        /// <param name="bitmapOut">is set to a pointer to the bitmap data in unmanaged memory</param>
        /// <param name="widthOut">is set to the width of the bitmap</param>
        /// <param name="heightOut">is set to the height of the bitmap</param>
        /// <param name="mdDataOut">is set to a pointer to the MDump data in unmanaged memory</param>
        /// <param name="mdDataLenOut">is set to the length of the MDump data</param>
        /// <returns></returns>
        [DllImport(dllName)]
        public static extern ECode LoadMergedImageData(string filename, 
            out IntPtr mdDataOut, out int mdDataLenOut);


        [DllImport(dllName)]
        public static extern ECode SavePNGToMemory(IntPtr bitmap, int width, int height,
            bool flipRGB, byte[] mdData, int mdDataLen, int compLevel,
            out IntPtr memPngOut, out int memPngLenOut);
    }
}
