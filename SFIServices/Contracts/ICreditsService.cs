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
    public interface ICreditsService
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<CreditType> GetAllCreditTypes();

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<Credit> GetAllCredits();

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void RegisterCreditApplication(CreditApplication newApplication);

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        List<CreditApplication> GetAllCreditApplications();

        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        CreditApplication RecoverCreditApplication(string invoice);
    }
}
