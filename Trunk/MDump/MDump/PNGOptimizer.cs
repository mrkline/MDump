using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace MDump
{
    /// <summary>
    /// Used to optimize PNG images using an external optimizer program (pngcrush)
    /// </summary>
    class PNGOptimizer
    {
        /// <summary>
        /// Passed into an optimizer thread.  Contains the source and destination
        /// filenames as well as the wait handle the thread sets when done.
        /// </summary>
        class ThreadArgs
        {
            public string SourceImg;
            public string DestImg;

            public ThreadArgs(string src, string dest)
            {
                SourceImg = src;
                DestImg = dest;
            }
        }

        private const string unoptPostfix = "_unop";

        //Use max compression (-l 9), RLE strategy (-z 3), and adaptive filtering (-f 5)
        //Also strip any color-correction data.
        private const string argList = "-brute -l 9 -z 1 -f 5 -rem gAMA -rem cHRM -rem iCCP -rem sRGB ";

        /// <summary>
        /// Thread procedure.  Optimizes the image, then sets its wait handle.
        /// </summary>
        /// <param name="arg">The ThreadArgs class being passed in</param>
        private static void OptimizeThreadProc(object arg)
        {
            ThreadArgs args = (ThreadArgs)arg;

            Process proc = new Process();
            ProcessStartInfo procSI = new ProcessStartInfo();
            procSI.FileName = "pngcrush.exe";
            procSI.WindowStyle = ProcessWindowStyle.Hidden;
            procSI.Arguments = argList + args.SourceImg + "\""
                + " " + "\"" + args.DestImg + "\"";
            proc.StartInfo = procSI;
            proc.Start();
            proc.WaitForExit();
        }

        /// <summary>
        /// Optimizes a set of PNG images using pngcrush.  Multithreading is used
        /// to speed up the process
        /// </summary>
        /// <param name="filenames">The PNG images to optimize</param>
        public static void OptimizeImages(IEnumerable<string> filenames)
        {
            List<string> unoptNames = new List<string>();
            List<Thread> threads = new List<Thread>();
            foreach (string fn in filenames)
            {
                string unopFn = fn.Insert(fn.IndexOf('.'), unoptPostfix);
                unoptNames.Add(unopFn);
                File.Copy(fn, unopFn, true);
                File.Delete(fn);
                Thread optThread = new Thread(OptimizeThreadProc);
                threads.Add(optThread);
                optThread.Start(new ThreadArgs(unopFn, fn));
            }

            //Wait for threads to finish
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            foreach (string fn in unoptNames)
            {
                File.Delete(fn);
            }
        }

        public static void OptimizeImage(string fn)
        {
            string unopFn = fn.Insert(fn.IndexOf('.'), unoptPostfix);
            File.Copy(fn, unopFn, true);
            File.Delete(fn);
            Process proc = new Process();
            ProcessStartInfo procSI = new ProcessStartInfo();
            procSI.FileName = "pngcrush.exe";
            procSI.WindowStyle = ProcessWindowStyle.Hidden;
            procSI.Arguments = argList + "\"" + unopFn + "\""
                + " " + "\"" + fn + "\"";
            proc.StartInfo = procSI;
            proc.Start();
            proc.WaitForExit();
            File.Delete(unopFn);
        }
    }
}
