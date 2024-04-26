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
    public interface ICreditGrantingPolicies
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        bool RegisterCreditGrantingPolicy(CreditGrantingPolicy NewPolicy);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<CreditGrantingPolicy> GetAllCreditGrantingPolicies();

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<CreditGrantingPolicy> RecoverActivesCreditGrantingPolicies();
    }
}
