using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MDump
{
    public enum ECode
    {
        EC_SUCESSS,
        EC_BAD_ARGS,
        EC_INIT_FAILURE,
        EC_IO_FAILURE,
        EC_HEADER_FAILURE,
        EC_ALLOC_FAILURE,
        EC_WRITE_IMAGE_FAILURE,
        EC_WRITE_END_FAILURE
    };

    /// <summary>
    /// Provides access to PNGOPS.dll
    /// </summary>
    class PNGOps
    {
        [DllImport("PNGOps.dll")]
        public static extern ECode SavePNG(IntPtr bitmap, int width, int height, string filename, bool flipRGB);

    }
}
