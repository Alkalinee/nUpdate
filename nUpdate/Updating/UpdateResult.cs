﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Linq;

namespace nUpdate.Updating
{
    internal class UpdateResult
    {
        private readonly List<UpdateConfiguration> _newUpdateConfigurations = new List<UpdateConfiguration>();
        private readonly bool _updatesFound;
        private UpdateConfiguration _newestConfiguration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateResult" /> class.
        /// </summary>
        public UpdateResult(IEnumerable<UpdateConfiguration> packageConfigurations, UpdateVersion currentVersion,
            bool isAlphaWished, bool isBetaWished)
        {
            if (packageConfigurations != null)
            {
                var is64Bit = Environment.Is64BitOperatingSystem;
                foreach (
                    var config in
                        packageConfigurations.Where(
                            item => new UpdateVersion(item.LiteralVersion) > currentVersion || item.NecessaryUpdate)
                            .Where(
                                config =>
                                    new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                    DevelopmentalStage.Release ||
                                    new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                    DevelopmentalStage.ReleaseCandidate ||
                                    ((isAlphaWished &&
                                      new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                      DevelopmentalStage.Alpha) ||
                                     (isBetaWished &&
                                      new UpdateVersion(config.LiteralVersion).DevelopmentalStage ==
                                      DevelopmentalStage.Beta)))
                    )
                {
                    if (config.UnsupportedVersions != null)
                    {
                        if (
                            config.UnsupportedVersions.Any(
                                unsupportedVersion =>
                                    new UpdateVersion(unsupportedVersion).BasicVersion == currentVersion.BasicVersion))
                            continue;
                    }

                    if (config.Architecture == Architecture.X86 && is64Bit ||
                        config.Architecture == Architecture.X64 && !is64Bit)
                        continue;

                    if (new UpdateVersion(config.LiteralVersion) <= currentVersion)
                        continue;

                    _newUpdateConfigurations.Add(config);
                }

                var highestVersion =
                    UpdateVersion.GetHighestUpdateVersion(
                        _newUpdateConfigurations.Select(item => new UpdateVersion(item.LiteralVersion)));
                _newUpdateConfigurations.RemoveAll(
                    item => new UpdateVersion(item.LiteralVersion) < highestVersion && !item.NecessaryUpdate);
            }

            _updatesFound = _newUpdateConfigurations.Count != 0;
        }

        /// <summary>
        ///     Gets a value indicating whether updates were found, or not.
        /// </summary>
        public bool UpdatesFound => _updatesFound;

        /// <summary>
        ///     Returns all new configurations.
        /// </summary>
        public IEnumerable<UpdateConfiguration> NewestConfigurations => _newUpdateConfigurations;
    }
}