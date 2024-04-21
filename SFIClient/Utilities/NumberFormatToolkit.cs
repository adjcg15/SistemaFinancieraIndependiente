using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIClient.Utilities
{
    public class NumberFormatToolkit
    {
        public static string FormatAsTenDigits(string number)
        {
            return $"{number.Substring(0, 3)} {number.Substring(3, 4)} {number.Substring(7)}";
        }

        public static string FormatAsTwelveDigits(string number)
        {
            return $"{number.Substring(0, 2)} {number.Substring(2, 3)} {number.Substring(5, 4)} {number.Substring(9)}";
        }
    }
}
