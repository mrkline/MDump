using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace MDump
{

    /// <summary>
    /// Provides access to PNGOPS.dll
    /// </summary>
    class PNGHandler : ImageFormatHandler
    {
        private const string dllNotFound = "PNGOps.dll could not be found.";
        private readonly Encoding enc = Encoding.UTF8;

        #region Return Code Enums

        /// <summary>
        /// Error codes for PNGOps functions
        /// </summary>
        enum ECode
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
        #endregion

        /// <summary>
        /// Name of the DLL which provides libpng functionality
        /// </summary>
        public const string DllName = "PNGOps.dll";

        /// <summary>
        /// Constructor which checks that PNGOps.dll is around to use
        /// </summary>
        public PNGHandler()
        {
            //Makes sure our DLL is around
            if (!System.IO.File.Exists(DllName))
            {
                throw new Exception(dllNotFound);
            }
        }

        public string FormatName { get { return "PNG"; } }

        public string Extension { get { return "png"; } }

        /// <summary>
        /// Checks if the image is a MDump merged image
        /// </summary>
        /// <param name="filepath">File path</param>
        /// <returns>true if the image is indeed an MDump merged image</returns>
        public bool SupportsMergedImage(string filepath)
        {
            MergedCode ret = IsMergedImage(filepath);
            if (ret == MergedCode.MC_MERGED)
            {
                return true;
            }
            if (ret == MergedCode.MC_NOT_MERGED)
            {
                return false;
            }
            throw new FormatHandlerException("IsDumpMergedImage failed with the error code " + ret.ToString());
        }

        /// <summary>
        /// Loads MDump data from an image.
        /// Does not test to see if the file is an MDump merged image.
        /// </summary>
        /// <param name="filename">Filename of the merged image</param>
        /// <returns>MDump image data from image</returns>
        public string LoadMergedImageData(string filename)
        {
            IntPtr unmanaged;
            int unmanagedLen;
            ECode result = LoadMergedImageData(filename, out unmanaged, out unmanagedLen);
            if (result != ECode.EC_SUCCESS)
            {
                throw new FormatHandlerException("LoadMergedImageData failed with the error code " + result.ToString());
            }
            byte[] ret = new byte[unmanagedLen];
            Marshal.Copy(unmanaged, ret, 0, unmanagedLen);
            FreeUnmanagedData(unmanaged);
            return enc.GetString(ret);
        }

        /// <summary>
        /// Saves a (assumed merged) image to managed memory
        /// </summary>
        /// <param name="bitmap">Image to save (assumed to be 32-bpp RGBA)</param>
        /// <param name="mdData">MDump data string to save</param>
        /// <param name="compLevel">Compression level to use</param>
        /// <returns>Memory containing saved image</returns>
        public byte[] SaveToMemory(Bitmap bitmap, string mdData, MDumpOptions.CompressionLevel compLevel)
        {
            System.Drawing.Imaging.BitmapData data =
                bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int compVal = 6; //Default, decent compression
            switch (compLevel)
            {
                case MDumpOptions.CompressionLevel.Low:
                    compVal = 5;
                    break;

                case MDumpOptions.CompressionLevel.Medium:
                    compVal = 6;
                    break;

                case MDumpOptions.CompressionLevel.High:
                    compVal = 7;
                    break;

                case MDumpOptions.CompressionLevel.Maximum:
                    compVal = 9;
                    break;
            }

            IntPtr memOut;
            int memOutLen;
            ECode result = SavePNGToMemory(data.Scan0, bitmap.Width, bitmap.Height, true,
                mdData, mdData.Length, compVal, out memOut, out memOutLen);
            if (result != ECode.EC_SUCCESS)
            {
                throw new FormatHandlerException("SavePNGToMemory failed with the error code " + result.ToString());
            }
            bitmap.UnlockBits(data);
            byte[] ret = new byte[memOutLen];
            Marshal.Copy(memOut, ret, 0, memOutLen);
            FreeUnmanagedData(memOut);
            return ret;
        }

        /// <summary>
        /// A simple wrapper around free() to release unmanaged memory once we've copied data out of it
        /// </summary>
        /// <param name="ptr">Pointer to the unmanaged memory</param>
        [DllImport(DllName)]
        private static extern void FreeUnmanagedData(IntPtr ptr);

        /// <summary>
        /// Checks if the image is merged by fist checking if it's a PNG, then if it contains the MDump
        /// metadata.
        /// </summary>
        /// <param name="filepath">File to test</param>
        /// <returns>MC_MERGED if the file is an MDump PNG merged image</returns>
        [DllImport(DllName)]
        private static extern MergedCode IsMergedImage(string filepath);

        /// <summary>
        /// Loads an MDump data from a merged PNG.  Does not test to see if the file is a merged png.
        /// </summary>
        /// <see cref="IsMergedImage"/>
        /// <param name="filename">Filename of the merged image</param>
        /// <param name="mdDataOut">Is set to a pointer to the MDump data in unmanaged memory</param>
        /// <param name="mdDataLenOut">Is set to the length of the MDump data</param>
        /// <returns></returns>
        [DllImport(DllName)]
        private static extern ECode LoadMergedImageData(string filename, 
            out IntPtr mdDataOut, out int mdDataLenOut);


        /// <summary>
        /// Saves a PNG to unmanaged memory
        /// </summary>
        /// <param name="bitmap">Image data to save (assumed to be 32-bpp)</param>
        /// <param name="width">Width of the image</param>
        /// <param name="height">Height of image</param>
        /// <param name="flipRGB">Flip RGB to BGR and vice versa</param>
        /// <param name="mdData">MDump data string to save</param>
        /// <param name="mdDataLen">Length of MDump data string</param>
        /// <param name="compLevel">Compression level to use</param>
        /// <param name="memPngOut">Returns assigned to the PNG file in unmanaged memory</param>
        /// <param name="memPngLenOut">Returns with the length of the PNG file in unmanaged memory</param>
        /// <returns>EC_SUCCESS on success</returns>
        [DllImport(DllName)]
        private static extern ECode SavePNGToMemory(IntPtr bitmap, int width, int height,
            bool flipRGB, string mdData, int mdDataLen, int compLevel,
            out IntPtr memPngOut, out int memPngLenOut);
    }
}
