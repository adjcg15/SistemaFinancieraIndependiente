using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
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
        public static bool RegisterCreditCondition(CreditCondition newCondition)
        {
            try
            {
                using (var context = new SFIDatabaseContext())
                {

                    var interestRateParam = new SqlParameter("@InterestRate", newCondition.InterestRate);
                    var isActiveParam = new SqlParameter("@IsActive", newCondition.IsActive);
                    var isIvaAppliedParam = new SqlParameter("@IsIvaApplied", newCondition.IsIvaApplied);
                    var interestOnArrearsParam = new SqlParameter("@InterestOnArrears", newCondition.InterestOnArrears);
                    var advancePaymentReductionParam = new SqlParameter("@AdvancePaymentReduction", newCondition.AdvancePaymentReduction);
                    var paymentMonthsParam = new SqlParameter("@PaymentMonths", newCondition.PaymentMonths);
                    var creditTypeIdParam = new SqlParameter("@CreditTypeId", newCondition.CreditType.Identifier);
                    var identifierParam = new SqlParameter("@Identifier", newCondition.Identifier);
                    var resultParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };

                    context.Database.ExecuteSqlCommand("InsertCreditConditionProcedure @InterestRate, @IsActive, @IsIvaApplied, @InterestOnArrears, @AdvancePaymentReduction, @PaymentMonths, @CreditTypeId, @Identifier, @Result OUTPUT",
                                     interestRateParam, isActiveParam, isIvaAppliedParam, interestOnArrearsParam, advancePaymentReductionParam, paymentMonthsParam, creditTypeIdParam, identifierParam, resultParam);

                    int result = (int)resultParam.Value;
                    return result == 0;
                }
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
        }
        public static List<CreditCondition> RecoverAllCreditConditions()
        {
            List<CreditCondition> conditionsList = new List<CreditCondition>();

            try
            {
                using (var context = new SFIDatabaseContext())
                {
                    context.credit_conditions
                        .ToList().ForEach(storedCondition =>
                        {
                            CreditCondition creditCondition = new CreditCondition();
                            creditCondition.Identifier = storedCondition.identifier;
                            creditCondition.AdvancePaymentReduction = storedCondition.advance_payment_reduction;
                            creditCondition.InterestOnArrears = storedCondition.interest_on_arrears;
                            creditCondition.InterestRate = storedCondition.interest_rate;
                            creditCondition.IsActive = storedCondition.is_active;
                            creditCondition.IsIvaApplied = storedCondition.is_iva_applied;
                            creditCondition.PaymentMonths = storedCondition.payment_months;
                            conditionsList.Add(creditCondition);

                        });
                }
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbUpdateException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            catch (DbEntityValidationException)
            {
                throw new FaultException<ServiceFault>(new ServiceFault("No fue posible recuperar los datos"));
            }
            return conditionsList;
        }
    }
}
