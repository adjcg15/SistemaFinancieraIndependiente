using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.DataAccessObjects
{
    public class ExampleDAO
    {
        public static DataTypeExample getDataTypeExample() 
        {
            try 
            {
                using (var context = new SFIDatabaseContext())
                {
                    //Alguna operación importante o llamada a procedimiento almacenado
                    List<bank_accounts> bankAccounts = context.bank_accounts.ToList();
                    foreach (bank_accounts account in bankAccounts)
                    {
                        Console.WriteLine("CUENTA DE BANCO: " + account.card_number);
                    }
                }
            } 
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }

            DataTypeExample dataType = new DataTypeExample();
            dataType.IsExample = true;
            dataType.Name = "Example";

            return dataType;
        }
    }
}
