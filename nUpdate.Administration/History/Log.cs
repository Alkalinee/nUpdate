﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using nUpdate.Administration.Application;

namespace nUpdate.Administration.History
{
    public class Log
    {
        /// <summary>
        ///     The name of the project that contains the log file.
        /// </summary>
        public UpdateProject Project { get; set; }

        /// <summary>
        ///     The time when the entry was made.
        /// </summary>
        public string EntryTime { get; set; }

        /// <summary>
        ///     The entry that was made.
        /// </summary>
        public LogEntry Entry { get; set; }

        /// <summary>
        ///     The version of the package that was given in the entry.
        /// </summary>
        public string PackageVersion { get; set; }

        /// <summary>
        ///     Writes an entry to the log.
        /// </summary>
        /// <param name="entry">The entry to set.</param>
        /// <param name="packageVersionString">The package version for the entry.</param>
        public void Write(LogEntry entry, string packageVersionString)
        {
            var log = new Log
            {
                EntryTime = DateTime.Now.ToString(),
                Entry = entry,
                PackageVersion = packageVersionString
            };

            if (Project.Log == null)
                Project.Log = new List<Log>();
            Project.Log.Add(log);
        }
    }
}