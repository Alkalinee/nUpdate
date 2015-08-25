﻿// Author: Dominic Beger (Trade/ProgTrade)

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nUpdate.Test
{
    [TestClass]
    public class ConnectionCheckerTest
    {
        /// <summary>
        ///     Checks if the "wininet.dll" returns the right stats.
        /// </summary>
        [TestMethod]
        public void CanReturnConnectionStatus()
        {
            Assert.IsTrue(WebConnection.IsAvailable());
        }
    }
}