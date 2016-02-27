﻿using System;
using nUpdate.Updating;

namespace nUpdate.UpdateEventArgs
{
    /// <summary>
    ///     Provides data for the <see cref="UpdateManager.UpdateDownloadProgressChanged" />-event.
    /// </summary>
    public class UpdateDownloadProgressChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateDownloadProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="bytesReceived">The amount of bytes received.</param>
        /// <param name="totalBytesToReceive">The total bytes to receive.</param>
        /// <param name="percentage">The progress percentage.</param>
        internal UpdateDownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, float percentage, double downloadSpeed)
        {
            TotalBytesToReceive = totalBytesToReceive;
            BytesReceived = bytesReceived;
            Percentage = percentage;
            DownloadSpeed = downloadSpeed;
        }

        /// <summary>
        /// Gets the current download speed in bytes / seconds
        /// </summary>
        public double DownloadSpeed { get; private set; }
        /// <summary>
        ///     Gets the total bytes to receive.
        /// </summary>
        public long TotalBytesToReceive { get; private set; }

        /// <summary>
        ///     Gets the amount of received bytes.
        /// </summary>
        public long BytesReceived { get; private set; }

        /// <summary>
        ///     Gets the progress percentage.
        /// </summary>
        public float Percentage { get; private set; }
    }
}
