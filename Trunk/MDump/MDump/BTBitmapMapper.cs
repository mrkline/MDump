using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;


/// <summary>
/// Uses a binary tree to efficiently map images into a given rectangle of space
/// </summary>
class BTBitmapMapper
{
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
        public int Area { get { return Rect.Width * Rect.Height; } }
    }
    
    private static int EdgeLenSorter(Bitmap bmp1, Bitmap bmp2)
    {
        return bmp2.Width - bmp1.Width;
    }

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

    public static Bitmap MergeImages(IEnumerable<Bitmap> bitmaps, Size maxSize, PixelFormat pixelFormat)
    {
        int totalArea = 0;
        foreach (Bitmap bmp in bitmaps)
        {
            totalArea += bmp.Width * bmp.Height;
        }
        if (totalArea > maxSize.Width * maxSize.Height)
        {
            throw new ArgumentException("The provided rectangle cannot fit the provided bitmaps.");
        }

        int maxX = 0;
        int maxY = 0;

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
            if (currRect.X + bmp.Width > maxX)
            {
                maxX = currRect.X + bmp.Width;
            }
            if (currRect.Y + bmp.Height > maxY)
            {
                maxY = currRect.Y + bmp.Height;
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

        //Assemble our image
        Bitmap merged = new Bitmap(maxX, maxY, pixelFormat);
        using (Graphics g = Graphics.FromImage(merged))
        {
            foreach (NodeData data in root)
            {
                if (data.Bmp != null)
                {
                    g.DrawImage(data.Bmp, data.Rect.X, data.Rect.Y,
                        data.Bmp.Width, data.Bmp.Height);
                }
            }
        }

        return merged;
    }
}