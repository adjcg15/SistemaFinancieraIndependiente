﻿using SFIDataAccess.DataAccessObjects;
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
        public CreditCondition RecoverCreditConditionDetails(string identifier)
        {
            return CreditConditionsDAO.RecoverCreditConditionDetails(identifier);
        }
        public bool UpdateCreditCondition(CreditCondition updateCreditCondition)
        {
            return CreditConditionsDAO.UpdateCreditCondition(updateCreditCondition);
        }
        public bool VerifyUsageInCreditApplications(string conditionIdentifier)
        {
            return CreditConditionsDAO.VerifyUsageInCreditApplications(conditionIdentifier);
        }
        public bool VerifyUsageInRegimen(string conditionIdentifier)
        {
            return CreditConditionsDAO.VerifyUsageInRegimen(conditionIdentifier);
        }
        public CreditCondition GetCurrentCreditConditionByCreditInvoice(string creditInvoice)
        {
            return CreditConditionsDAO.GetCurrentCreditConditionByCreditInvoice(creditInvoice);
        }
    }
}
