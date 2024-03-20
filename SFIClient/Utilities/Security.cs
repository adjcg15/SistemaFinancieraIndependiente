using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SFIClient.Utilities
{
    public class Security
    {
        public static class Roles
        {
            public const string MANAGER = "GERENTE";
            public const string CREDIT_ANALYST = "ANALISTA";
            public const string CREDIT_ADVISOR = "ASESOR";
            public const string DEBT_COLLECTOR = "COBRADOR";
        }

        public static string HashPasswordWithSHA256(string pplainPassword)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pplainPassword));

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
    }
}
