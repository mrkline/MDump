using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace MDump
{
    [Serializable]
    public class MDumpOptions 
    {
        public enum PathOptions
        {
            PreservePath,
            PreserveName,
            Discard
        }

        public const string fileName = "MDumpOptions.xml";

        public PathOptions MergePathOpts { get; set; }
        public PathOptions SplitPathOpts { get; set; }
        public string SplitDestination { get; set; }
        public bool PromptForSplitDestination { get; set; }
        public uint MaxMergeSize { get; set; }

        /// <summary>
        /// Default constructor.  Initializes options to defaults.
        /// </summary>
        public MDumpOptions()
        {
            MergePathOpts = PathOptions.Discard; //Discard file info while merging
            SplitPathOpts = PathOptions.PreservePath; //Respect any file info when splitting
            SplitDestination = string.Empty; //No initial split destination (see below)
            PromptForSplitDestination = true; //Prompt for split destination
            MaxMergeSize = 2048; //Default max merge size of 2 megabytes
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

        public static MDumpOptions FromFile(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MDumpOptions));
            using (StreamReader sw = new StreamReader(filename))
            {
               return ser.Deserialize(sw) as MDumpOptions;
            }
        }

        public void SaveToFile(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(MDumpOptions));
            using (StreamWriter sw = new StreamWriter(filename))
            {
                ser.Serialize(sw, this);
            }
        }

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

        public override int GetHashCode()
        {
            int hash = SplitDestination.GetHashCode();
            hash += Convert.ToInt32(PromptForSplitDestination);
            hash += (int)MergePathOpts;
            hash += (int)SplitPathOpts << 2;
            hash += Convert.ToInt32(MaxMergeSize);
            return hash;
        }

        public bool IsDefaultOptions()
        {
            return Equals(new MDumpOptions());
        }
    }
}
