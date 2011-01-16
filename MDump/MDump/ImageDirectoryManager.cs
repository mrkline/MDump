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

        #region Directory Class
        /// <summary>
        /// A tree-like class that stores its parent directory, other directories, and images,
        /// as well as a GUI representation of itself using ListViewItems.
        /// </summary>
        private class ImageDirectory : IEnumerable
        {
            private const string duplicateImgMsg = "This image is already contained in the directory";
            private const string duplicateDirMsg = "This directory already has a child with the name ";
            private const string noSuchItemMsg = "The provided item is not in this directory's list";

            private List<Bitmap> images;
            private List<ImageDirectory> children;
            private List<ListViewItem> listViewItems;

            public string Name { get; set; }
            
            public ImageDirectory Parent { get; private set; }

            //Lists are not made directly public as listViewItems depends on the contents of
            //images and children

            public ReadOnlyCollection<Bitmap> Images
            {
                get { return images.AsReadOnly(); }
            }

            public ReadOnlyCollection<ImageDirectory> Children
            {
                get { return children.AsReadOnly(); }
            }

            public ReadOnlyCollection<ListViewItem> LVIRepresentation
            {
                get { return listViewItems.AsReadOnly(); }
            }

            public ImageDirectory(string name, ImageDirectory parent)
            {
                images = new List<Bitmap>();
                children = new List<ImageDirectory>();
                listViewItems = new List<ListViewItem>();
                Name = name;
                Parent = parent;
            }

            public ImageDirectory(string name) : this(name, null) { }

            /// <summary>
            /// Adds an image to this directory
            /// </summary>
            /// <param name="imgPath">Path of image to add</param>
            /// <returns>The GUI representation of this image</returns>
            public ListViewItem AddImage(string imgPath)
            {
                string name = Path.GetFileName(imgPath);
                foreach(Bitmap image in images)
                {
                    if(((string)image.Tag).Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new ArgumentException(duplicateImgMsg);
                    }
                }
                Bitmap bmp = new Bitmap(imgPath);
                bmp.Tag = name;
                images.Add(bmp);
                ListViewItem ret = new ListViewItem(name);
                ret.Tag = bmp;
                listViewItems.Add(ret);
                return ret;
            }
            
            /// <summary>
            /// Removes an image from this directory through its GUI representation.
            /// </summary>
            /// <param name="imgItem">GUI representation of the image to be removed</param>
            public void RemoveImage(ListViewItem imgItem)
            {
                if (!listViewItems.Contains(imgItem))
                {
                    throw new ArgumentException(noSuchItemMsg);
                }
                images.Remove((Bitmap)imgItem.Tag);
                listViewItems.Remove(imgItem);
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
                listViewItems.Add(ret);
                return ret;
            }

            /// <summary>
            /// Removes a child directory from this directory through its GUI representation
            /// </summary>
            /// <param name="dirItem">GUI representation of the child directory to be removed</param>
            public void RemoveDirectory(ListViewItem dirItem)
            {
                if (!listViewItems.Contains(dirItem))
                {
                    throw new ArgumentException(noSuchItemMsg);
                }
                children.Remove((ImageDirectory)dirItem.Tag);
                listViewItems.Remove(dirItem);
            }

            /// <summary>
            /// Removes all child directories (and their graphical representations) from this directory
            /// </summary>
            public void MoveChildDirImagesHere()
            {
                foreach (ImageDirectory child in children)
                {
                    foreach (Bitmap bmp in child)
                    {
                        images.Add(bmp);
                        ListViewItem lvi = new ListViewItem(Path.GetFileName((string)bmp.Tag));
                        lvi.Tag = bmp;
                        listViewItems.Add(lvi);
                    }
                }
                children.Clear();
                listViewItems.RemoveAll(RepresentsChildDir);
            }

            #region Predicates
            /// <summary>
            /// Checks if a ListViewItem represents an image in this directory
            /// </summary>
            /// <param name="item">ListViewItem to check</param>
            /// <returns>true if item represents an image in this directory</returns>
            private static bool RepresentsImage(ListView item)
            {
                return item.Tag as string != null;
            }

            /// <summary>
            /// Checks if a ListViewItem represent a child directory of this directory
            /// </summary>
            /// <param name="item">ListViewItem to check</param>
            /// <returns>true if item represents a child directory</returns>
            private static bool RepresentsChildDir(ListViewItem item)
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

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageDirectoryManager()
        {
            root = new ImageDirectory(rootName);
        }

        /// <summary>
        /// Moves all images to the root directory and kills subdirectories.
        /// Used for when the user switches options to not saving directory info.
        /// </summary>
        public void MoveAllToRoot()
        {
            root.MoveChildDirImagesHere();   
        }
    }
}
