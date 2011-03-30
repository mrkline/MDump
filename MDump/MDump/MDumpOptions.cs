using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MDump
{
    /// <summary>
    /// Holds the application options.
    /// </summary>
    [Serializable]
    public class MDumpOptions
    {
        #region String Constants
        private const string couldNotInitOptions = "Default options could not be initialized.";
        private const string invalidPathOptionsExMsg = "Did not pass FormatPathFromOpts a valid PathOptions value";
        #endregion

        #region Enums
        /// <summary>
        /// Options for compressions
        /// </summary>
        public enum CompressionLevel
        {
            Low,
            Medium,
            High,
            Maximum
        }

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
        #endregion

        /// <summary>
        /// Default options file filepath
        /// </summary>
        [XmlIgnore]
        public const string fileName = "MDumpOptions.xml";

        /// <summary>
        /// Gets or sets the path options for saving file paths in to the merged image
        /// </summary>
        public PathOptions MergePathOpts { get; set; }
        /// <summary>
        /// Gets or sets the path options for loading file paths from a merged image
        /// </summary>
        public PathOptions SplitPathOpts { get; set; }
        /// <summary>
        /// Gets or sets the maximum size of merged images (in bytes)
        /// </summary>
        public int MaxMergeSize { get; set; }
        /// <summary>
        /// Gets or sets the currently used merge format
        /// </summary>
        public string MergeFormat { get; set; }
        /// <summary>
        /// Gets or sets the compression level used by merger
        /// </summary>
        public CompressionLevel CompLevel { get; set; }
        /// <summary>
        /// Gets or sets whether or not merges will add title bars to merged images
        /// </summary>
        public bool AddTitleBar { get; set; }

        /// <summary>
        /// We're not using a true singleton pattern as it is logical to create an options
        /// object from a file or from the options menu. We do want, however, a simple way to
        /// access the options we're using globally.
        /// </summary>
        public static MDumpOptions Instance { get; set; }

        /// <summary>
        /// Default constructor.  Initializes options to defaults.
        /// </summary>
        public MDumpOptions()
        {
            MergePathOpts = PathOptions.PreservePath; //Save file path while merging
            SplitPathOpts = PathOptions.PreservePath; //Respect file path info when splitting
            MaxMergeSize = 2048 * 1024; //Default max merge size of 2 megabytes
            //Use first format in the master format handler
            using (IEnumerator<string> enumer =
                MasterFormatHandler.Instance.SupportedFormatNames.GetEnumerator())
            {
                if (enumer.MoveNext())
                {
                    MergeFormat = enumer.Current;
                }
                else
                {
                    throw new Exception(couldNotInitOptions);
                }
            }
            CompLevel = CompressionLevel.High;
            AddTitleBar = true;
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
        /// Tests if one options object is the same as another.
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
                    && MergeFormat == other.MergeFormat
                    && CompLevel == other.CompLevel
                    && MaxMergeSize == other.MaxMergeSize
                    && AddTitleBar == other.AddTitleBar;
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
            int hash = (int)MergePathOpts;
            hash += (int)SplitPathOpts << 2;
            //HACK: Not guaranteed to be unique.
            hash += Convert.ToInt32(MaxMergeSize);
            hash += Convert.ToInt32(CompLevel);
            hash += Convert.ToInt32(AddTitleBar);
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
