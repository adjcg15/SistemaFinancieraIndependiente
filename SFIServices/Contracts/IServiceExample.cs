using SFIDataAccess.CustomExceptions;
using SFIDataAccess.Model;
using System.ServiceModel;

namespace SFIServices.Contracts
{
    [ServiceContract]
    public interface IServiceExample
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        DataTypeExample GetDataUsingDataContract();
    }
}
