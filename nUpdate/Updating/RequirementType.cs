// Author: Dominic Beger (Trade/ProgTrade)

using System.ComponentModel;

namespace nUpdate.Updating
{
    public enum RequirementType
    {
        [Description("DotNetFrameworkText")]
        DotNetFramework,
        // ReSharper disable once InconsistentNaming
        [Description("OperatingSystemText")]
        OSVersion,
    }
}