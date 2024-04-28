using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SFIClient.Utilities
{
    public class DataValidator
    {
        public static bool IsValidMoneyAmount(string amount)
        {
            return amount.Length <= 7
                && double.TryParse(amount, out double result)
                && result >= 0
                && result <= 1000000;
        }

        public static bool IsPlainText(string text)
        {
            string plainTextPattern = @"^[a-zA-ZáéíóúÁÉÍÓÚüÜ\s]+$";

            return Regex.IsMatch(text, plainTextPattern);
        }
    }
}
