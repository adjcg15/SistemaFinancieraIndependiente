using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.DataAccessObjects
{
    public class ExampleDAO
    {
        public static DataTypeExample getDataTypeExample()
        {
            using (var context = new SFIDatabaseContext())
            {
                //Alguna operación importante o llamada a procedimiento almacenado
                List<bank_accounts> bankAccounts = context.bank_accounts.ToList();
                foreach(bank_accounts account in bankAccounts)
                {
                    Console.WriteLine("CUENTA DE BANCO: " + account.card_number);
                }
            }

            DataTypeExample dataType = new DataTypeExample();
            dataType.IsExample = true;
            dataType.Name = "Example";

            return dataType;
        }
    }
}
