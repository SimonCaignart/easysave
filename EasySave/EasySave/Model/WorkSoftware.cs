using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;
using EasySave.Helper_Classes;
using System.Diagnostics;

namespace EasySave
{
    /// <summary>
    /// Runs through active processes to add work software.
    /// </summary>
    class WorkSoftware : ObservableObject
    {
        /// <summary>
        /// Constructor to use ProcessList elswhere.
        /// </summary>
        public WorkSoftware()
        {
            proclist = ProcessList();
        }
        public string[] proclist;

        /// <summary>
        /// Runs through processes and adds the process name and id to a list.
        /// </summary>
        /// <returns>Full process list</returns>
        public string[] ProcessList()
        {
            Process[] processes = Process.GetProcesses();
            List<string> processlist = new List<string>();
            foreach (Process allProcesses in processes)
            {
                if (!String.IsNullOrEmpty(allProcesses.ProcessName))
                {
                    string processnameid = allProcesses.ProcessName;
                    processlist.Add(processnameid);
                }
            }
            processlist.Sort();
            proclist = processlist.ToArray();
            return proclist;
        }
    }
}

