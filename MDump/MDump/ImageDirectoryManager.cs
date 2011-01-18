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
        private const string cannotEscapeRoot = "There is no directory higher than the root directory";

        #region Directory Class
        /// <summary>
        /// A tree-like class that stores its parent directory, other directories, and images,
        /// as well as a GUI representation of itself using ListViewItems.
        /// </summary>
        private class ImageDirectory : IEnumerable
        {
            #region String Constants
            public const string duplicateImgMsg = "This directory already has an image with the name ";
            public const string duplicateDirMsg = "This directory already has a subdirectory with the name ";
            public const string noSuchItemMsg = "The provided item is not in this directory's list";
            #endregion

            private List<Bitmap> images;
            private List<ImageDirectory> children;

            public string Name { get; set; }
            
            public ImageDirectory Parent { get; private set; }

            public ImageDirectory(string name, ImageDirectory parent)
            {
                images = new List<Bitmap>();
                children = new List<ImageDirectory>();
                Name = name;
                Parent = parent;
            }

            public ImageDirectory(string name) : this(name, null) { }

            /// <summary>
            /// Adds an already created and tagged image to this directory.
            /// </summary>
            /// <param name="img">Image to add, tagged with its file name</param>
            /// <returns>The GUI representation of this image</returns>
            public ListViewItem AddTaggedImage(Bitmap img)
            {
                foreach (Bitmap bmp in images)
                {
                    if (((string)bmp.Tag).Equals((string)img.Tag, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Duplicate image found.  Kill it with fire
                        throw new ArgumentException(duplicateImgMsg + img.Tag);
                    }
                }
                images.Add(img);
                ListViewItem ret = new ListViewItem();
                ret.Tag = img;
                return ret;
            }
            
            /// <summary>
            /// Removes an image from this directory through its GUI representation.
            /// </summary>
            /// <param name="imgItem">GUI representation of the image to be removed</param>
            public void RemoveImage(ListViewItem imgItem)
            {
                Bitmap img = imgItem.Tag as Bitmap;
                if (img == null || !images.Contains(img))
                {
                    throw new ArgumentException(noSuchItemMsg);
                }
                images.Remove(img);
            }

            /// <summary>
            /// Adds a child directory to this directory
            /// </summary>
            /// <param name="name">Name of directory to add</param>
            /// <returns>The GUI representation of this directory</returns>
            public ListViewItem AddDirectory(string name)
            {
                ImageDirectory toAdd = new ImageDirectory(name, this);
                if(children.Contains(toAdd))
                {
                    throw new ArgumentException(duplicateDirMsg + name);
                }
                children.Add(toAdd);
                ListViewItem ret = new ListViewItem(name);
                ret.Tag = toAdd;
                return ret;
            }

            /// <summary>
            /// Removes a child directory from this directory through its GUI representation
            /// </summary>
            /// <param name="dirItem">GUI representation of the child directory to be removed</param>
            public void RemoveDirectory(ListViewItem dirItem)
            {
                ImageDirectory dir = dirItem.Tag as ImageDirectory;
                if (dir == null || !children.Contains(dir))
                {
                    throw new ArgumentException(noSuchItemMsg);
                }
                children.Remove(dir);
            }

            /// <summary>
            /// Checks if this directory has a given directory as a child
            /// </summary>
            /// <param name="dir">child directory</param>
            /// <returns>true if this directory is the parent of dir</returns>
            public bool HasDirectory(ImageDirectory dir)
            {
                return children.Contains(dir);
            }

            /// <summary>
            /// Get a child directory, given its name
            /// </summary>
            /// <param name="dirName">Name of child directory</param>
            /// <returns>The child directory with the given name</returns>
            public ImageDirectory GetDirectory(string dirName)
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
            /// Builds a list of ListViewItems based on images and child directories
            /// contained within this directory.
            /// </summary>
            /// <returns>A list of ListViewItems representing the contents of this directory</returns>
            public List<ListViewItem> CreateListViewRepresentation()
            {
                List<ListViewItem> ret = new List<ListViewItem>();

                //Add child directories
                foreach (ImageDirectory child in children)
                {
                    ListViewItem toAdd = new ListViewItem();
                    toAdd.Tag = child;
                    ret.Add(toAdd);
                }

                //Add images
                foreach (Bitmap bmp in images)
                {
                    ListViewItem toAdd = new ListViewItem();
                    toAdd.Tag = bmp;
                    ret.Add(toAdd);
                }

                return ret;
            }

            /// <summary>
            /// Removes all child directories from this directory, moving their images to this one.
            /// </summary>
            public void MoveChildDirImagesHere()
            {
                foreach (ImageDirectory child in children)
                {
                    foreach (Bitmap bmp in child)
                    {
                        images.Add(bmp);
                    }
                }
                children.Clear();
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

            #region IEnumerator
            public IEnumerator GetEnumerator()
            {
                //First return our items, then any child items
                foreach (Bitmap bmp in images)
                {
                    yield return bmp;
                }
                foreach (ImageDirectory child in children)
                {
                    foreach (Bitmap bmp in child)
                    {
                        yield return bmp;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion
        }
#endregion

        private ImageDirectory root;
        private ImageDirectory activeDirectory;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageDirectoryManager()
        {
            root = new ImageDirectory(rootName);
            activeDirectory = root;
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
        /// Sets the active directory to a given child
        /// </summary>
        /// <param name="dirItem">the GUI representation of the child directory</param>
        public void MoveToChildDirecory(ListViewItem dirItem)
        {
            ImageDirectory dir = dirItem.Tag as ImageDirectory;
            if (!activeDirectory.HasDirectory(dir))
            {
                throw new ArgumentException(ImageDirectory.noSuchItemMsg);
            }
            activeDirectory = dir;
        }

        /// <summary>
        /// Sets the active directory using a given path
        /// </summary>
        /// <param name="path"></param>
        public void SetActiveDirectory(string path)
        {
            //Get rid of any formatting concerns
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            string[] dirs = path.Split(new char[] { Path.DirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);
            activeDirectory = root;

            for (int c = 0; c < dirs.Length; ++c)
            {
                //Will throw an ArgumentException if the activedirectory does not have the desired child
                activeDirectory = activeDirectory.GetDirectory(dirs[c]);
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
        /// Calls RemoveImage on the active directory
        /// </summary>
        /// <seealso cref="ImageDirectory.AddImage"/>
        public void DeleteImage(ListViewItem imgItem)
        {
            activeDirectory.RemoveImage(imgItem);
        }

        /// <summary>
        /// Calls AddDirectory on the active directory
        /// </summary>
        /// <seealso cref="ImageDirectory.AddDirectory"/>
        public void AddChildDirectory(string dirName)
        {
            activeDirectory.AddDirectory(dirName);
        }

        /// <summary>
        /// Calls RemoveDirectory on the active directory
        /// </summary>
        /// <seealso cref="ImageDirectory.RemoveDirectory"/>
        public void RemoveChildDirectory(ListViewItem dirItem)
        {
            activeDirectory.RemoveDirectory(dirItem);
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
