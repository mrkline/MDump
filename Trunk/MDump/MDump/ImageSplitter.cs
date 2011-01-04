using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

namespace MDump
{
    class ImageSplitter
    {
        public const string SplitKeyword = "split";

        /// <summary>
        /// Callback stages for splitting
        /// </summary>
        public enum SplitStage
        {
            /// <summary>
            /// Starting split process. Value is the number of merges to split.
            /// </summary>
            Starting,
            /// <summary>
            /// Starting to split a new merged image.
            /// Value is the number of images in the merge.
            /// </summary>
            SplittingNewMerge,
            /// <summary>
            /// Splitting an image out of a merged image.
            /// Value is the number of images split out of the current merge so far.
            /// </summary>
            SplittingImage,
            /// <summary>
            /// Split operation done. Value is not used.
            /// </summary>
            Done
        }

        /// <summary>
        /// A callback to inform the GUI thread of what the split thread is doing
        /// </summary>
        /// <param name="stage">Current stage of split procedure</param>
        /// <param name="value">Has different menaing for each stage</param>
        public delegate void SplitCallback(SplitStage stage, int value);

        private class SplitThreadArgs
        {
            public List<Bitmap> Bitmaps { get; private set; }
            public MDumpOptions Options { get; private set; }
            public string SplitPath { get; private set; }
            public SplitCallback Callback { get; private set; }

            public SplitThreadArgs(List<Bitmap> bitmaps, MDumpOptions opts, string splitPath,
                SplitCallback callback)
            {
                Bitmaps = bitmaps;
                Options = opts;
                SplitPath = splitPath;
                Callback = callback;
            }
        }

        public static void SplitImages(List<Bitmap> bitmaps, MDumpOptions opts, string splitPath,
            SplitCallback callback)
        {
            SplitThreadArgs ta = new SplitThreadArgs(bitmaps, opts, splitPath, callback);
            Thread thread = new Thread(SplitThreadProc);
            thread.Start(ta);
        }

        private static void SplitThreadProc(object args)
        {
            SplitThreadArgs sa = args as SplitThreadArgs;


        }
    }
}
