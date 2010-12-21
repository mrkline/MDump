using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace MDump
{
    /// <summary>
    /// Holds the application options.
    /// </summary>
    [Serializable]
    public class MDumpOptions 
    {
        /// <summary>
        /// Options for saving/loading paths into/from merged images
        /// </summary>
        public enum PathOptions
        {
            /// <summary>
            /// Save/Load the full file name and path
            /// </summary>
            PreservePath,
            /// <summary>
            /// Save/Load only the file's name
            /// </summary>
            PreserveName,
            /// <summary>
            /// Discard both file name and path
            /// </summary>
            Discard
        }

        /// <summary>
        /// Default options file filename
        /// </summary>
        [XmlIgnore]
        public const string fileName = "MDumpOptions.xml";

        /// <summary>
        /// Used as a placeholder for discarded filenames
        /// </summary>
        private const string kDiscardFn = "\a";

        /// <summary>
        /// The base directory that all images being merged share.
        /// Used later for calculating relative paths to write into the merged image
        /// </summary>
        [XmlIgnore]
        public string BaseDirectory { get; set; }

        public void SetBaseDirectory(IEnumerable<Bitmap> bitmaps)
        {
            char[] baseDir = null;
            foreach (Bitmap bmp in bitmaps)
            {
                char[] curr = Path.GetDirectoryName((string)bmp.Tag).ToCharArray();

                //Set the base directory to the first item
                if (baseDir == null)
                {
                    baseDir = curr;
                    BaseDirectory = Path.GetDirectoryName((string)bmp.Tag);
                    continue;
                }

                int shortest = curr.Length < baseDir.Length ? curr.Length : baseDir.Length;
                for (int c = 0; c < shortest; ++c)
                {
                    if (baseDir[c] != curr[c])
                    {
                        BaseDirectory = ((string)bmp.Tag).Substring(0, c);
                        baseDir = BaseDirectory.ToCharArray();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the path options for saving file paths in to the merged image
        /// </summary>
        public PathOptions MergePathOpts { get; set; }
        /// <summary>
        /// Gets or sets the path options for loading file paths from a merged image
        /// </summary>
        public PathOptions SplitPathOpts { get; set; }
        /// <summary>
        /// Gets or sets the folder to split images in to
        /// </summary>
        public string SplitDestination { get; set; }
        /// <summary>
        /// Gets or sets whether or not the user should be prompted for a split destination
        /// <see cref="SplitDestination"/>
        /// </summary>
        public bool PromptForSplitDestination { get; set; }
        /// <summary>
        /// Gets or sets the maximum size of merged images (in bytes)
        /// </summary>
        public ulong MaxMergeSize { get; set; }

        /// <summary>
        /// Formats a path based on the provided options
        /// </summary>
        /// <param name="path">path to format</param>
        /// <param name="opts">options to format path by</param>
        /// <returns>formatted path, or null if the options discard the path</returns>
        private string FormatPathFromOpts(string path, PathOptions opts)
        {
            switch (opts)
            {
                case PathOptions.Discard:
                    return kDiscardFn;

                case PathOptions.PreserveName:
                    if (path.Contains(Path.DirectorySeparatorChar.ToString()))
                    {
                        return path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    }
                    else
                    {
                        return path;
                    }

                case PathOptions.PreservePath:
                    return path.Remove(0, BaseDirectory.Length);

                default:
                    throw new ArgumentException("Did not pass FormatPathFromOpts a valid PathOptions value");
            }
        }

        /// <summary>
        /// Formats a given path based on the current state of MergePathOpts
        /// </summary>
        /// <param name="path">path to format</param>
        /// <returns>formatted path</returns>
        public string FormatPathForMerge(string path)
        {
            return FormatPathFromOpts(path, MergePathOpts);    
        }

        /// <summary>
        /// Formats a given path based on the current state of SplitPathOpts
        /// </summary>
        /// <param name="path">path to format</param>
        /// <returns>formatted path</returns>
        public string FormatPathForSplit(string path)
        {
            return FormatPathFromOpts(path, SplitPathOpts);
        }

        /// <summary>
        /// Returns whether or not the given filename should be discarded.
        /// </summary>
        /// <param name="filename">filename to check</param>
        /// <returns>true if the filename should be discarded (if it equals kDiscardFn)</returns>
        public bool DiscardFilename(string filename)
        {
            return filename == kDiscardFn;
        }

        /// <summary>
        /// Default constructor.  Initializes options to defaults.
        /// </summary>
        public MDumpOptions()
        {
            MergePathOpts = PathOptions.PreservePath; //Save file name while merging
            SplitPathOpts = PathOptions.PreserveName; //Respect file name when splitting
            SplitDestination = string.Empty; //No initial split destination (see below)
            PromptForSplitDestination = true; //Prompt for split destination
            MaxMergeSize = 2048 * 1024; //Default max merge size of 2 megabytes
        }

        /// <summary>
        /// Constructor that builds options from given values.
        /// </summary>
        /// <param name="mergePathOpts">Merge path option</param>
        /// <param name="splitPathOpts">Split path option</param>
        /// <param name="splitDest">Destination of split images</param>
        /// <param name="promptForSplitDest">Prompt for destination of split images</param>
        /// <param name="maxMergeSz">Max size of merged images.</param>
        public MDumpOptions(PathOptions mergePathOpts, PathOptions splitPathOpts,
            string splitDest, bool promptForSplitDest, uint maxMergeSz)
        {
            MergePathOpts = mergePathOpts;
            SplitPathOpts = splitPathOpts;
            SplitDestination = splitDest;
            PromptForSplitDestination = promptForSplitDest;
            MaxMergeSize = maxMergeSz;
        }

        /// <summary>
        /// Loads options from a file using XML serialization
        /// </summary>
        /// <param name="filename">XML serialization to load options from</param>
        /// <returns>The new options from the file</returns>
        public static MDumpOptions FromFile(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MDumpOptions));
            using (StreamReader sw = new StreamReader(filename))
            {
               return ser.Deserialize(sw) as MDumpOptions;
            }
        }

        /// <summary>
        /// Saves the options using XML serialization
        /// </summary>
        /// <param name="filename">the file to serialize to</param>
        public void SaveToFile(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MDumpOptions));
            using (StreamWriter sw = new StreamWriter(filename))
            {
                ser.Serialize(sw, this);
            }
        }

        /// <summary>
        /// Tests if one options object is the same as another
        /// </summary>
        /// <param name="obj">The other options object</param>
        /// <returns>true if the options are the same</returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(MDumpOptions))
            {
                MDumpOptions other = (MDumpOptions)obj;
                return MergePathOpts == other.MergePathOpts
                    && SplitPathOpts == other.SplitPathOpts
                    && SplitDestination == other.SplitDestination
                    && PromptForSplitDestination == other.PromptForSplitDestination
                    && MaxMergeSize == other.MaxMergeSize;
            }
            return false;
        }

        /// <summary>
        /// Hash code override to make the compiler stop whining that we've
        /// overridden Equals but not GetHashCode.
        /// </summary>
        /// <returns>Hash code of options object</returns>
        public override int GetHashCode()
        {
            int hash = SplitDestination.GetHashCode();
            hash += Convert.ToInt32(PromptForSplitDestination);
            hash += (int)MergePathOpts;
            hash += (int)SplitPathOpts << 2;
            hash += Convert.ToInt32(MaxMergeSize);
            return hash;
        }

        /// <summary>
        /// Checks if the provided options are the same as default ones
        /// </summary>
        /// <returns>true if the options are the same</returns>
        public bool IsDefaultOptions()
        {
            return Equals(new MDumpOptions());
        }
    }
}
