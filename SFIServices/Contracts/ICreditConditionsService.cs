using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SFIServices.Contracts
{
    [ServiceContract]
    public interface ICreditConditionsService
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<CreditCondition> RecoverCreditConditionsByCreditType(int creditTypeIdentifier);
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool RegisterCreditCondition(CreditCondition NewCondition);
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<CreditCondition> RecoverAllCreditConditions();
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        CreditCondition RecoverCreditConditionDetails(string identifier);
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool UpdateCreditCondition(CreditCondition updateCreditCondition);
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool VerifyUsageInCreditApplications(string conditionIdentifier);
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool VerifyUsageInRegimen(string conditionIdentifier);
    }
}


