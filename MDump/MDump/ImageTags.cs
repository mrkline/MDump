using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MDump
{
    /// <summary>
    /// Class that stores information to tag along with loaded images.
    /// </summary>
    class ImageTagBase
    {
        private const int imageIconIndex = 0;

        /// <summary>
        /// Gets the ListViewRepresentation of the 
        /// </summary>
        public ListViewItem LVI { get; private set;}

        /// <summary>
        /// Gets name of the image via it's LVI representation.
        /// Name is set via LVI.BeginEdit()
        /// </summary>
        public string Name
        {
            get { return LVI.Text; }
        }

        public ImageTagBase(string name, Bitmap bmp)
        {
            LVI = new ListViewItem(name, imageIconIndex);
            LVI.Tag = bmp;
        }
    }

    /// <summary>
    /// Tag specific to merging images
    /// </summary>
    class IndividualImageTag : ImageTagBase
    {
        /// <summary>
        /// Gets the MDump directory info of this image
        /// </summary>
        public string MDumpDir { get; set; }

        public IndividualImageTag(string name, Bitmap bmp, string dir)
            : base(name, bmp)
        {
            MDumpDir = dir;
        }
    }

    /// <summary>
    /// Tag specific to splitting images
    /// </summary>
    class MergedImageTag : ImageTagBase
    {
        /// <summary>
        /// Gets the MDData needed to split the merged image apart
        /// </summary>
        public string MDData { get; private set; }

        public MergedImageTag(string name, Bitmap bmp, string mdData)
            : base(name, bmp)
        {
            MDData = mdData;
        }
    }
}
