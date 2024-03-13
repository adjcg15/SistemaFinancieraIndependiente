using SFIDataAccess.Model;
using System.ServiceModel;

namespace SFIServices.Contracts
{
    [ServiceContract]
    public interface IServiceExample
    {
        [OperationContract]
        DataTypeExample GetDataUsingDataContract();
    }
}
