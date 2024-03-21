using SFIDataAccess.DataAccessObjects;
using SFIDataAccess.Model;
using SFIServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIServices
{
    public partial class SFIService : ICreditConditionsService
    {
        public List<CreditCondition> RecoverCreditConditionsByCreditType(int creditTypeIdentifier)
        {
            return CreditConditionsDAO.RecoverCreditConditionsByCreditType(creditTypeIdentifier);
        }
        public bool RegisterCreditCondition(CreditCondition NewCondition)
        {
            return CreditConditionsDAO.RegisterCreditCondition(NewCondition);
        }
        public List<CreditCondition> RecoverAllCreditConditions()
        {
            return CreditConditionsDAO.RecoverAllCreditConditions();
        }
    }
}
