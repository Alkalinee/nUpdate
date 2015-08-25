﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;

namespace nUpdate
{
    public class UriConnector
    {
        public static Uri ConnectUri(string start, string end)
        {
            if (!Uri.IsWellFormedUriString(start, UriKind.RelativeOrAbsolute))
                return null;

            if (!start.EndsWith("/"))
                start += "/";
            var baseUri = new Uri(start);
            return new Uri(baseUri, end);
        }
    }
}