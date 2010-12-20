using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;


/// <summary>
/// Uses a binary tree to efficiently map images into a given rectangle of space
/// </summary>
class BTBitmapMapper
{
    /// <summary>
    /// Used as binary tree node data while building the image map
    /// </summary>
    class NodeData
    {
        public Bitmap Bmp { get; set; }
        private Rectangle _rect;
        public Rectangle Rect
        {
            get { return _rect; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("A BTBitmapMapper tree node cannot have a null rectangle");
                }
                _rect = value;
            }
        }
        public NodeData(Rectangle rct) : this(rct, null) { }
        public NodeData(Rectangle rct, Bitmap bmp)
        {
            if (rct == null)
            {
                throw new ArgumentException("A BTBitmapMapper tree node cannot have a null rectangle");
            }
            Bmp = bmp;
            Rect = rct;
        }
        /// <summary>
        /// Gets the area of this NodeData's rectangle
        /// </summary>
        public int Area { get { return Rect.Width * Rect.Height; } }
    }
    
    /// <summary>
    /// A sort function that sorts bitmaps by their width, largest to smallest
    /// </summary>
    private static int EdgeLenSorter(Bitmap bmp1, Bitmap bmp2)
    {
        return bmp2.Width - bmp1.Width;
    }

    /// <summary>
    /// Finds 
    /// </summary>
    /// <param name="root">The root node of the binary tree</param>
    /// <param name="requiredSize">The size of the next image that needs to be fit in the tree</param>
    /// <returns>the node to insert the next image in, if one can be found, null otherwise</returns>
    private static BinaryTreeNode<NodeData> GetNextAppropriateNode(BinaryTreeNode<NodeData> root,
        Size requiredSize)
    {
       //Run a BFS for the next appropriate node to hold a bitmap of the given size
        List<BinaryTreeNode<NodeData>> appropriateNodes = new List<BinaryTreeNode<NodeData>>();
        Queue<BinaryTreeNode<NodeData>> q = new Queue<BinaryTreeNode<NodeData>>();
        q.Enqueue(root);

        while (q.Count > 0)
        {
            BinaryTreeNode<NodeData> curr = q.Dequeue();
            Size currSz = curr.Data.Rect.Size;
            if (currSz.Width >= requiredSize.Width && currSz.Height >= requiredSize.Height
                && curr.Data.Bmp == null)
            {
                appropriateNodes.Add(curr);
            }
            else
            {
                if (curr.Left != null)
                {
                    q.Enqueue(curr.Left);
                }
                if (curr.Right != null)
                {
                    q.Enqueue(curr.Right);
                }
            }
        }
                
        //This shouldn't happen, but return null if no fitting node can be found
        if (appropriateNodes.Count == 0)
        {
            return null;
        }

        //Pick the best node (closest to the top, to conserve height) to insert the next image in to.

        BinaryTreeNode<NodeData> bestNode = appropriateNodes[0];

        //Pick the best node (by smallest y value)
        foreach( BinaryTreeNode<NodeData> node in appropriateNodes)
        {
            if (node.Data.Rect.Y < bestNode.Data.Rect.Y)
            {
                bestNode = node;
            }
        }

        return bestNode;
    }

    /// <summary>
    /// Uses a binary tree to merge images into a single bitmap.
    /// </summary>
    /// <param name="bitmaps">Bitmaps to merge into one</param>
    /// <param name="maxSize">The maximum size the merged bimap can take (in pixels)</param>
    /// <param name="pixelFormat">The pixel format the merged image should use</param>
    /// <param name="mdData">MDump Data to save</param>
    /// <returns>The single bitmap containing all the provided bitmaps.</returns>
    public static Bitmap MergeImages(IEnumerable<Bitmap> bitmaps, Size maxSize, PixelFormat pixelFormat,
        out byte[] mdData)
    {
        //Do a quick check to make sure the provided size is <= the total area of all images.
        int totalArea = 0;
        foreach (Bitmap bmp in bitmaps)
        {
            totalArea += bmp.Width * bmp.Height;
        }
        if (totalArea > maxSize.Width * maxSize.Height)
        {
            throw new ArgumentException("The provided rectangle cannot fit the provided bitmaps.");
        }

        //To be used later to clip final bitmap if it takes up less space than the given max
        Size actualSize = new Size(0, 0);

        List<Bitmap> sortedBitmaps = new List<Bitmap>(bitmaps);
        sortedBitmaps.Sort(EdgeLenSorter);

        BinaryTreeNode<NodeData> root = new BinaryTreeNode<NodeData>(
            new NodeData(new Rectangle(new Point(0, 0), maxSize)));

        foreach (Bitmap bmp in sortedBitmaps)
        {
            BinaryTreeNode<NodeData> curr = GetNextAppropriateNode(root, bmp.Size);
            curr.Data.Bmp = bmp;
            Rectangle currRect = curr.Data.Rect;

            //Push the boundaries of our image
            if (currRect.X + bmp.Width > actualSize.Width)
            {
                actualSize.Width = currRect.X + bmp.Width;
            }
            if (currRect.Y + bmp.Height > actualSize.Height)
            {
                actualSize.Height = currRect.Y + bmp.Height;
            }

            Rectangle lRect, rRect;
            //If the image is wider than it is big, make one child the area below and the other the rest
            if (bmp.Width > bmp.Height)
            {
                lRect = new Rectangle(currRect.X, currRect.Y + bmp.Height + 1,
                    currRect.Width, currRect.Height - bmp.Height);
                rRect = new Rectangle(currRect.X + bmp.Width + 1, currRect.Y,
                    currRect.Width - bmp.Width, bmp.Height);
            }
            //If the image is taller than it is big, make one child the area to the right and the other the rest
            else
            {
                lRect = new Rectangle(currRect.X, currRect.Y + bmp.Height + 1,
                    bmp.Width, currRect.Height - bmp.Height);
                rRect = new Rectangle(currRect.X + bmp.Width + 1, currRect.Y,
                    currRect.Width - bmp.Width, currRect.Height);
            }
            curr.Left = new BinaryTreeNode<NodeData>(new NodeData(lRect));
            curr.Right = new BinaryTreeNode<NodeData>(new NodeData(rRect));
        }

        //Assemble our image and generate our data
        Bitmap merged = new Bitmap(actualSize.Width, actualSize.Height, pixelFormat);

        MemoryStream mdStream = new MemoryStream();

        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        using (Graphics g = Graphics.FromImage(merged))
        {
            using(BinaryWriter bw = new BinaryWriter(mdStream, System.Text.Encoding.UTF8))
            {
                foreach (NodeData data in root)
                {
                    if (data.Bmp != null)
                    {
                        g.DrawImage(data.Bmp, data.Rect.X, data.Rect.Y,
                            data.Bmp.Width, data.Bmp.Height);
                        bw.Write(encoder.GetBytes((string)data.Bmp.Tag)); //Write filename, without usual string len prefix
                        Rectangle r = data.Rect;
                        //Convert these to UTF8/ASCII (We'll have to parse back later.)
                        bw.Write(';');
                        bw.Write(encoder.GetBytes(r.X.ToString()));
                        bw.Write(';');
                        bw.Write(encoder.GetBytes(r.Y.ToString()));
                        bw.Write(';');
                        bw.Write(encoder.GetBytes(r.Width.ToString()));
                        bw.Write(';');
                        bw.Write(encoder.GetBytes(r.Height.ToString()));
                        bw.Write('\n'); //Split each image and its data by newlines
                    }
                }
            }
        }

        Byte[] buff = mdStream.GetBuffer();
        Byte[] retBuff = null;
        //Trim the buffer
        for (int c = buff.Length - 1; c > 0; --c)
        {
            if (buff[c] != 0)
            {
                retBuff = new Byte[c];
                Buffer.BlockCopy(buff, 0, retBuff, 0, c);
                break;
            }
        }


        mdData = retBuff;
        return merged;
    }
}