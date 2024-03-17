using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIClient.Utilities
{
    public class FileToolkit
    {
        public static byte[] GetFileContent(string fileName, int maxSizeAllowed = 150)
        {
            FileInfo file = new FileInfo(fileName);
            byte[] fileContent = null;

            if (file.Exists)
            {
                long fileSizeInBytes = file.Length;
                long fileSizeInKilobytes = fileSizeInBytes / 1024;

                if (fileSizeInKilobytes <= 150)
                {
                    fileContent = File.ReadAllBytes(file.FullName);
                }
            }

            return fileContent;
        }
    }
}
