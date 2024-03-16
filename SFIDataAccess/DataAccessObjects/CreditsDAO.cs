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
    public class CreditsDAO
    {
        public static List<CreditType> GetAllCreditTypes()
        {
            List<CreditType> creditTypes = new List<CreditType>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.credit_types.ToList().ForEach(storedCreditType =>
                    {
                        CreditType creditType = new CreditType();
                        creditType.Identifier = storedCreditType.id_credit_type;
                        creditType.Name = storedCreditType.name;

                        creditTypes.Add(creditType);
                    });
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible recuperar los tipos de crédito, intente más tarde")
                );
            }

            return creditTypes;
        }

        public static void RegisterCreditApplication(CreditApplication newApplication)
        {

        }
    }
}
