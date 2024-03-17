using SFIClient.SFIServices;
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
        public static string INE = "INE";
        public static string PROOF_ADDRESS = "COMDOM";
        public static string PROOF_INCOME = "COMINGR";

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

        public static string GenerateDefaultIneName(Client client)
        {
            return "copia_ine_" + client.Rfc + ".pdf";
        }

        public static string GenerateDefaultAddressDocumentName(Client client)
        {
            return "copia_domicilio_" + client.Rfc + ".pdf";
        }

        public static string GenerateDefaultIncomeDocumentName(Client client)
        {
            return "copia_cingresos_" + client.Rfc + ".pdf";
        }
    }
}
