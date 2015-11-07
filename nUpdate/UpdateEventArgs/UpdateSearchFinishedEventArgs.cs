// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using nUpdate.Updating;

namespace nUpdate.UpdateEventArgs
{
    /// <summary>
    ///     Provides data for the <see cref="Updater.UpdateSearchFinished" />-event.
    /// </summary>
    public class UpdateSearchFinishedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateSearchFinishedEventArgs" />-class.
        /// </summary>
        /// <param name="updatesAvailable">A value which indicates whether a new update is available or not.</param>
        internal UpdateSearchFinishedEventArgs(bool updatesAvailable, Dictionary<UpdateVersion, List<UpdateRequirement>> unfulfilledRequirements)
        {
            UpdatesAvailable = updatesAvailable;
            UnfulfilledRequirements = unfulfilledRequirements;
        }

        /// <summary>
        ///     Gets a value indicating whether new updates are available, or not.
        /// </summary>
        public bool UpdatesAvailable { get; private set; }

        /// <summary>
        ///     Gets the unfulfilled <see cref="UpdateRequirement"/>s of available versions that have been associated with the update search.
        /// </summary>
        public Dictionary<UpdateVersion, List<UpdateRequirement>> UnfulfilledRequirements { get; private set; }
    }
}