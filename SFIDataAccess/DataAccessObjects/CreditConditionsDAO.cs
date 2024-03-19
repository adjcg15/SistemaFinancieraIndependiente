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
    public class CreditConditionsDAO
    {
        public static List<CreditCondition> RecoverCreditConditionsByCreditType(int creditTypeIdentifier)
        {
            List<CreditCondition> contidionsList = new List<CreditCondition>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.credit_conditions
                        .Where(condition => condition.id_credit_type == creditTypeIdentifier)
                        .ToList().ForEach(storedCondition => {
                            CreditCondition condition = new CreditCondition();

                            condition.Identifier = storedCondition.identifier;
                            condition.AdvancePaymentReduction = storedCondition.advance_payment_reduction;
                            condition.InterestOnArrears = storedCondition.interest_on_arrears;
                            condition.InterestRate = storedCondition.interest_rate;
                            condition.IsActive = storedCondition.is_active;
                            condition.IsIvaApplied = storedCondition.is_iva_applied;
                            condition.PaymentMonths = storedCondition.payment_months;

                            contidionsList.Add(condition);
                        });
                }
            }
            catch (EntityException)
            {
                throw new FaultException<ServiceFault>(
                    new ServiceFault("No fue posible recuperar las condiciones de crédito, intente más tarde"),
                    new FaultReason("Error")
                );
            }

            return contidionsList;
        }
    }
}
