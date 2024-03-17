using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
