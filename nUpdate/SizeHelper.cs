using System.Text;
using nUpdate.Win32;

namespace nUpdate
{
    public class SizeHelper
    {
        public static string ToAdequateSizeString(long fileSize)
        {
            var sb = new StringBuilder(20);
            NativeMethods.StrFormatByteSize(fileSize, sb, 20);
            return sb.ToString();
        }
    }
}
