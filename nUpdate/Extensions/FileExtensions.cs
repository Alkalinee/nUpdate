using System.IO;
using System.Threading;

namespace nUpdate.Extensions
{
    public static class FileExtensions
    {
        /// <summary>
        ///     Blocks until the file is not locked any more.
        /// </summary>
        /// <param name="fullPath"></param>
        public static FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
        {
            for (int numTries = 0; numTries < 10; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access, share);

                    fs.ReadByte();
                    fs.Seek(0, SeekOrigin.Begin);

                    return fs;
                }
                catch (IOException)
                {
                    fs?.Dispose();
                    Thread.Sleep(50);
                }
            }

            return null;
        }
    }
}