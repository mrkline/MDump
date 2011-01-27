using System;
using System.Collections.Generic;
using System.Text;

namespace MDump
{
    /// <summary>
    /// Class that stores information to tag along with loaded images.
    /// </summary>
    abstract class ImageTagBase
    {
        /// <summary>
        /// Gets the name of the image
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Tag specific to merging images
    /// </summary>
    class MergeImageTag : ImageTagBase
    {
        /// <summary>
        /// Gets the MDump directory info of this image
        /// </summary>
        public string MDumpDir { get; set; }

        public MergeImageTag(string name, string dir)
        {
            Name = name;
            MDumpDir = dir;
        }
    }

    /// <summary>
    /// Tag specific to splitting images
    /// </summary>
    class SplitImageTag : ImageTagBase
    {
        /// <summary>
        /// Gets the MDData needed to split the merged image apart
        /// </summary>
        public byte[] MDData { get; private set; }

        public SplitImageTag(string name, byte[] mdData)
        {
            Name = name;
            MDData = mdData;
        }
    }
}
