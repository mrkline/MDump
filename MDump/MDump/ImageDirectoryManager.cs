using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace MDump
{
    /// <summary>
    /// Used to manage and display images and directories to be merged
    /// </summary>
    class ImageDirectoryManager
    {
        private const string rootName = "root";

        /// <summary>
        /// A tree-like class that stores its parent directory, other directories, and images,
        /// as well as a GUI representation of itself using ListViewItems.
        /// </summary>
        private class Directory
        {
            private List<Bitmap> images;
            private List<Directory> children;
            private List<ListViewItem> listViewItems;

            public string Name { get; set; }
            
            public Directory Parent { get; private set; }

            public ReadOnlyCollection<ListViewItem> LVIRepresentation
            {
                get { return listViewItems.AsReadOnly(); }
            }

            public Directory(string name, Directory parent)
            {
                images = new List<Bitmap>();
                children = new List<Directory>();
                listViewItems = new List<ListViewItem>();
                Name = name;
                Parent = parent;
            }

            public Directory(string name) : this(name, null) { }

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
                        throw new ArgumentException("This image is already contained in the directory");
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
            /// <param name="imgItem">GUI representation of the item</param>
            public void RemoveImage(ListViewItem imgItem)
            {
                images.Remove((Bitmap)imgItem.Tag);
                listViewItems.Remove(imgItem);
            }

            public void AddDirectory(string name)
            {
                Directory toAdd = new Directory(name, this);
            }

            //TODO: Pick up here with add/remove directory code

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

                if (!(obj is Directory))
                    throw new InvalidCastException("The 'obj' argument is not a Directory object.");
                else
                    return Equals(obj as Directory);   
            }

            /// <summary>
            /// Equality test. Since the directories are unique based on their name (ignoring case),
            /// we weill base equality on this
            /// </summary>
            /// <param name="dir">Directory to test equality with</param>
            /// <returns>True if the directories are equal</returns>
            public bool Equals(Directory dir)
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

        private Directory root;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageDirectoryManager()
        {
            root = new Directory(rootName);
        }

        /// <summary>
        /// Moves all images to the root directory and kills subdirectories.
        /// Used for when the user switches options to not saving directory info.
        /// </summary>
        public void MoveAllToRoot()
        {
        }
    }
}
