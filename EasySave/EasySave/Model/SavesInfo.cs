using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave
{

    /// <summary>
    /// List of the information containing in one Save.
    /// </summary>
    class SavesInfo
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string SaveType { get; set; }
    }

    /// <summary>
    /// Creation of a list of SavesInfo.
    /// </summary>
        class SaveList
            {
                public List<SavesInfo> Save { get; set; }
            }
    }

