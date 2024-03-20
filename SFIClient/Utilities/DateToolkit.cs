using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIClient.Utilities
{
    public class DateToolkit
    {
        public static string FormatAsDDMMYYY(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string FormatAsText(DateTime date)
        {
            return date.ToString("d 'de' MMMM 'de' yyyy");
        }
    }
}
