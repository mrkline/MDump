using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace CompressionRatioTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Out.WriteLine("Incorrect args.");
                return;
            }
            try
            {
                Bitmap bmp = new Bitmap(args[0]);
                long area = bmp.Width * bmp.Height;
                long fileSize = new FileInfo(args[0]).Length;
                Console.Out.Write("Average bytes per pixel is ");
                Console.Out.WriteLine(((float)fileSize / (float)area).ToString());
            }
            catch
            {
                Console.Out.WriteLine("Exceptions got thrown.");
            }
        }
    }
}
