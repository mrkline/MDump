using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace MDump
{
    /// <summary>
    /// Used to manage and display images and directories to be merged
    /// </summary>
    class ImageDirectoryManager
    {
        private const string rootName = "root";
        private const string defaultDirName = "New Folder";
        private const string cannotEscapeRoot = "There is no directory higher than the root directory";
        private const string noSuchPathMsg = "The path entered does not exist.";
        private const string noSuchPathTitle = "No such path";

        #region ImageDirectory Class
        /// <summary>
        /// A tree-like class that stores its parent directory, other directories, and images,
        /// as well as a GUI representation of itself using ListViewItems.
        /// </summary>
        private class ImageDirectory
        {
            #region String Constants
            public const string duplicateImgMsg = "This directory already has an image with the name ";
            public const string duplicateDirMsg = "This directory already has a subdirectory with the name ";
            public const string noSuchItemMsg = "The provided item is not in this directory's list.";
            public const string notImgOrDirMsg = "The provided ListViewItem is not tagged to an image or an "
                + "image directory.";
            #endregion

            private const int folderIconIndex = 1;

            private List<Bitmap> images;
            private List<ImageDirectory> children;

            public ListViewItem LVI { get; private set; }

            /// <summary>
            /// Gets the name of the directory
            /// </summary>
            public string Name { get { return LVI.Text; } }

            /// <summary>
            /// Gets the Parent directory of this directory
            /// </summary>
            public ImageDirectory Parent { get; private set; }

            /// <summary>
            /// Gets a list of all images in the directory and its children
            /// </summary>
            public List<Bitmap> ImagesRecursive
            {
                get
                {
                    List<Bitmap> ret = new List<Bitmap>();
                    Queue<ImageDirectory> q = new Queue<ImageDirectory>();
                    q.Enqueue(this);
                    while (q.Count > 0)
                    {
                        ImageDirectory curr = q.Dequeue();
                        ret.AddRange(curr.images);
                        foreach (ImageDirectory child in curr.children)
                        {
                            q.Enqueue(child);
                        }
                    }
                    return ret;
                }
            }

            /// <summary>
            /// Builds a list of ListViewItems based on images and child directories
            /// contained within this directory.
            /// </summary>
            /// <returns>A list of ListViewItems representing the contents of this directory</returns>
            public List<ListViewItem> ContentsLVIList
            {
                get
                {
                    List<ListViewItem> ret = new List<ListViewItem>();
                    //Add child directories
                    foreach (ImageDirectory child in children)
                    {
                        ret.Add(child.LVI);
                    }

                    //Add images
                    foreach (Bitmap bmp in images)
                    {
                        ret.Add((bmp.Tag as ImageTagBase).LVI);
                    }

                    return ret;
                }
            }

            /// <summary>
            /// Gets the path of current directory, without "root\"
            /// </summary>
            public string Path
            {
                get
                {
                    string ret = string.Empty;
                    for (ImageDirectory curr = this;
                        curr.Name != rootName; curr = curr.Parent)
                    {
                        ret = ret.Insert(0, curr.Name + System.IO.Path.DirectorySeparatorChar);
                    }
                    //Knock off the trailing \ character
                    if (ret.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                    {
                        ret = ret.Substring(0, ret.Length - 1);
                    }
                    return ret;
                }
            }

            /// <summary>
            /// Constructor that takes a name and a parent
            /// </summary>
            /// <param name="name">name of this directory</param>
            /// <param name="parent">parent of this directory</param>
            public ImageDirectory(string name, ImageDirectory parent)
            {
                images = new List<Bitmap>();
                children = new List<ImageDirectory>();
                LVI = new ListViewItem(name, folderIconIndex);
                LVI.Tag = this;
                Parent = parent;
            }

            /// <summary>
            /// Constructor that takes a name
            /// </summary>
            /// <param name="name">name of the directory</param>
            public ImageDirectory(string name) : this(name, null) { }

            /// <summary>
            /// Adds an already created and tagged image to this directory.
            /// </summary>
            /// <param name="img">Image to add, tagged with its file name</param>
            /// <returns>The GUI representation of this image</returns>
            public void AddTaggedImage(Bitmap img)
            {
                foreach (Bitmap bmp in images)
                {
                    if ((bmp.Tag as ImageTagBase).Name.Equals(
                       (img.Tag as ImageTagBase).Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Duplicate image found.  Kill it with fire
                        throw new ArgumentException(duplicateImgMsg + ((ImageTagBase)img.Tag).Name);
                    }
                }
                images.Add(img);
            }

            /// <summary>
            /// Adds an already created and tagged image to a path within this directory
            /// </summary>
            /// <param name="img">Image to add</param>
            /// <param name="dirPath">Path of image, starting in this folder.
            /// Directories are created automatically if they do not exist.</param>
            /// <returns>
            /// GUI representation of the topmost folder of the path, if it does not already exist.
            /// Otherwise returns null.
            /// </returns>
            public ListViewItem AddImagePath(Bitmap img, string dirPath)
            {
                //Get rid of any formatting concerns
                char dirSepChar = System.IO.Path.DirectorySeparatorChar;
                dirPath = dirPath.Replace(System.IO.Path.AltDirectorySeparatorChar, dirSepChar);
                string[] dirs = dirPath.Split(new char[] { dirSepChar },
                    StringSplitOptions.RemoveEmptyEntries);
                
                ImageDirectory currDir = this;
                int dirIdx;

                //dirs[dirs.Length - 1] is the image name itself
                for (dirIdx = 0; dirIdx < dirs.Length; ++dirIdx)
                {
                    if (currDir.HasChild(dirs[dirIdx]))
                    {
                        currDir = currDir.GetChild(dirs[dirIdx]);
                    }
                    else
                    {
                        break;
                    }
                }

                bool returnLVI = dirIdx == 0;

                //If the directories don't exist all the way in, create them
                if (dirIdx < dirs.Length)
                {
                    for (; dirIdx < dirs.Length; ++dirIdx)
                    {
                        currDir = currDir.AddDirectoryInternal(dirs[dirIdx]);
                    }
                }
                
                currDir.AddTaggedImage(img);

                if (returnLVI)
                {
                    return GetChild(dirs[0]).LVI;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Adds a child directory to this directory
            /// </summary>
            /// <param name="name">Name of directory to add</param>
            /// <returns>The GUI representation of this directory</returns>
            public ListViewItem AddDirectory(string name)
            {
                ImageDirectory toAdd = new ImageDirectory(name, this);
                if (children.Contains(toAdd))
                {
                    throw new ArgumentException(duplicateDirMsg + name);
                }
                children.Add(toAdd);
                return toAdd.LVI;
            }

            /// <summary>
            /// Identical to AddDirectory except returns the new directory itself.
            /// </summary>
            private ImageDirectory AddDirectoryInternal(string name)
            {
                ImageDirectory toAdd = new ImageDirectory(name, this);
                if (children.Contains(toAdd))
                {
                    throw new ArgumentException(duplicateDirMsg + name);
                }
                children.Add(toAdd);
                return toAdd;
            }

            /// <summary>
            /// Removes an image or a child directory from the current directory
            /// </summary>
            /// <param name="item">The GUI representation of the item to remove</param>
            public void RemoveItem(ListViewItem item)
            {
                Bitmap bmp = item.Tag as Bitmap;
                if (bmp == null)
                {
                    ImageDirectory dir = item.Tag as ImageDirectory;
                    if (dir == null)
                    {
                        throw new ArgumentException(notImgOrDirMsg);
                    }
                    else
                    {
                        if (!children.Remove(dir))
                        {
                            throw new ArgumentException(noSuchItemMsg);
                        }
                    }
                }
                else
                {
                    foreach (Bitmap img in images)
                    {
                        //We should just be able to do a reference test since the name should be
                        //pointing at the bitmap in our list
                        if (bmp == img)
                        {
                            images.Remove(img);
                            return;
                        }
                    }
                    //If we didn't find it, we didn't have it to begin with
                    throw new ArgumentException(noSuchItemMsg);
                }
            }

            /// <summary>
            /// Checks if this directory has a given directory as a child
            /// </summary>
            /// <param name="dir">child directory</param>
            /// <returns>true if this directory is the parent of directory</returns>
            public bool HasChild(ImageDirectory dir)
            {
                return children.Contains(dir);
            }

            /// <summary>
            /// Checks if this directory has a given directory as a child
            /// </summary>
            /// <param name="dirName">Name of the child directory</param>
            /// <returns>true if a directory with the given name is a child of this directory</returns>
            public bool HasChild(String dirName)
            {
                foreach (ImageDirectory child in children)
                {
                    if (child.Name.Equals(dirName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Get a child directory, given its name
            /// </summary>
            /// <param name="dirName">Name of child directory</param>
            /// <returns>The child directory with the given name</returns>
            public ImageDirectory GetChild(string dirName)
            {
                foreach (ImageDirectory child in children)
                {
                    if (child.Name.Equals(dirName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return child;
                    }
                }
                throw new ArgumentException(noSuchItemMsg);
            }

            /// <summary>
            /// Removes all child directories from this directory, moving their images to this one.
            /// </summary>
            public void MoveChildDirImagesHere()
            {
                foreach (ImageDirectory child in children)
                {
                    images.AddRange(child.ImagesRecursive);
                }
                children.Clear();
            }

            /// <summary>
            /// Clears the directory of all images and children
            /// </summary>
            public void Reset()
            {
                children.Clear();
                images.Clear();
            }

            #region Predicates
            /// <summary>
            /// Checks if a ListViewItem represents an image in this directory
            /// </summary>
            /// <param name="item">ListViewItem to check</param>
            /// <returns>true if item represents an image in this directory</returns>
            public static bool RepresentsImage(ListView item)
            {
                return item.Tag as string != null;
            }

            /// <summary>
            /// Checks if a ListViewItem represent a child directory of this directory
            /// </summary>
            /// <param name="item">ListViewItem to check</param>
            /// <returns>true if item represents a child directory</returns>
            public static bool RepresentsChildDir(ListViewItem item)
            {
                return item.Tag as ImageDirectory != null;
            }
            #endregion

            #region Equality
            /// <summary>
            /// Equality test. Since the directories are unique based on their name (ignoring case),
            /// we weill base equality on this
            /// </summary>
            /// <param name="obj">Object to test equality with</param>
            /// <returns>True if the objects are equal</returns>
            public override bool Equals(object obj)
            {
                if (obj == null) return base.Equals(obj);

                if (!(obj is ImageDirectory))
                    throw new InvalidCastException("The 'obj' argument is not a Directory object.");
                else
                    return Equals(obj as ImageDirectory);
            }

            /// <summary>
            /// Equality test. Since the directories are unique based on their name (ignoring case),
            /// we weill base equality on this
            /// </summary>
            /// <param name="dir">Directory to test equality with</param>
            /// <returns>True if the directories are equal</returns>
            public bool Equals(ImageDirectory dir)
            {
                return Name.Equals(dir.Name, StringComparison.InvariantCultureIgnoreCase);
            }

            /// <summary>
            /// Just to make the compiler happy and give Equals its best friend GetHashCode
            /// </summary>
            /// <returns>Hash code</returns>
            public override int GetHashCode()
            {
                return Name.ToLowerInvariant().GetHashCode();
            }
            #endregion
        }
#endregion
        
        private ImageDirectory root;
        private ImageDirectory activeDirectory;

        /// <summary>
        /// Gets the ListViewItem Representation of the active directory
        /// </summary>
        /// <seealso cref="ImageDirectory.ContentsLVIList"/>
        public List<ListViewItem> LVIRepresentation
        {
            get
            {
                return activeDirectory.ContentsLVIList;
            }
        }

        /// <summary>
        /// Gets the path of the active directory, sans root\
        /// </summary>
        public string CurrentPath
        {
            get
            {
                return activeDirectory.Path;
            }
        }

        /// <summary>
        /// Gets all bitmaps in the directory structure, starting at the root directory.
        /// </summary>
        /// <returns></returns>
        public List<Bitmap> ImageList
        {
            get
            {
                return root.ImagesRecursive;
            }
        }

        /// <summary>
        /// Gets true if the directory structure, starting at the root directory,
        /// contains no images
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return ImageList.Count == 0;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageDirectoryManager()
        {
            root = new ImageDirectory(rootName);
            activeDirectory = root;
        }

        /// <summary>
        /// Sets the active directory to the root directory
        /// </summary>
        public void SetActiveToRoot()
        {
            activeDirectory = root;
        }

        /// <summary>
        /// Returns true if the active directory has a parent
        /// </summary>
        /// <returns>true if the active directory has a parent</returns>
        public bool ActiveHasParent()
        {
            return activeDirectory.Parent != null;
        }

        /// <summary>
        /// Sets the active directory to its parent, if possible
        /// </summary>
        public void MoveUpDirectory()
        {
            if(activeDirectory.Parent == null)
            {
                throw new InvalidOperationException(cannotEscapeRoot);
            }
            activeDirectory = activeDirectory.Parent;
        }

        /// <summary>
        /// Returns true if the provided ListViewItem represents an image directory
        /// </summary>
        /// <param name="item">ListViewItem to test</param>
        /// <returns>true if the provided ListViewItem represents an image directory</returns>
        public bool ItemRepresentsDirectory(ListViewItem item)
        {
            return item.Tag as ImageDirectory != null;
        }

        /// <summary>
        /// Sets the active directory to a given child
        /// </summary>
        /// <param name="dirItem">the GUI representation of the child directory</param>
        public void MoveToChildDirecory(ListViewItem dirItem)
        {
            ImageDirectory dir = dirItem.Tag as ImageDirectory;
            if (!activeDirectory.HasChild(dir))
            {
                throw new ArgumentException(ImageDirectory.noSuchItemMsg);
            }
            activeDirectory = dir;
        }

        /// <summary>
        /// Sets the active directory using a given path
        /// </summary>
        /// <param name="path">The path of the directory, from root\</param>
        public void SetActiveDirectory(string path)
        {
            //If all fails, fall back to the current directory
            ImageDirectory fallback = activeDirectory;

            try
            {
                //Get rid of any formatting concerns
                path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                string[] dirs = path.Split(new char[] { Path.DirectorySeparatorChar },
                    StringSplitOptions.RemoveEmptyEntries);
                activeDirectory = root;

                for (int c = 0; c < dirs.Length; ++c)
                {
                    //Will throw an ArgumentException if the activedirectory does not have the desired child
                    activeDirectory = activeDirectory.GetChild(dirs[c]);
                }

            }
            catch (ArgumentException)
            {
                //A directory couldn't be resolved from the given path
                MessageBox.Show(noSuchPathMsg, noSuchPathTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                activeDirectory = fallback;
            }
        }

        /// <summary>
        /// Calls AddTaggedImage on the active directory
        /// </summary>
        /// <seealso cref="ImageDirectory.AddTaggedImage"/>
        public void AddImage(Bitmap bmp)
        {
            activeDirectory.AddTaggedImage(bmp);
        }

        /// <summary>
        /// Calls AddImagePath on the active directory
        /// </summary>
        /// <seealso cref="ImageDirectory.AddImagePath"/>
        public ListViewItem AddImagePath(Bitmap img, string dirPath)
        {
            return activeDirectory.AddImagePath(img, dirPath);
        }

        /// <summary>
        /// Adds a child directory with a default name.
        /// </summary>
        /// <returns>LVI representation of the directory</returns>
        public ListViewItem AddChildDirectory()
        {
            string name = defaultDirName;

            if (activeDirectory.HasChild(name))
            {
                for (int c = 2; activeDirectory.HasChild(name); ++c)
                {
                    name = defaultDirName + " (" + c.ToString() + ")";
                }
            }

            return activeDirectory.AddDirectory(name);
        }

        /// <summary>
        /// Calls AddDirectory on the active directory
        /// </summary>
        /// <seealso cref="ImageDirectory.AddDirectory"/>
        public ListViewItem AddChildDirectory(string dirName)
        {
            return activeDirectory.AddDirectory(dirName);
        }

        /// <summary>
        /// Calls RemoveItem on the active directory
        /// </summary>
        /// <seealso cref="ImageDirectory.RemoveItem"/>
        public void RemoveItem(ListViewItem imgItem)
        {
            activeDirectory.RemoveItem(imgItem);
        }

        /// <summary>
        /// Clears out everything and sets the active directory to root.
        /// </summary>
        public void Reset()
        {
            activeDirectory = root;
            root.Reset();
        }

        /// <summary>
        /// Moves all images to the root directory and kills subdirectories.
        /// Used for when the user switches options to not saving directory info.
        /// </summary>
        /// <seealso cref=">ImageDirectory.MoveChildDirImagesHere"/>
        public void MoveAllToRoot()
        {
            root.MoveChildDirImagesHere();
        }
    }
}
