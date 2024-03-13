using SFIDataAccess.DataAccessObjects;
using SFIDataAccess.Model;
using SFIServices.Contracts;

namespace SFIServices
{
    public partial class SFIService : IServiceExample
    {
        public DataTypeExample GetDataUsingDataContract()
        {
            return ExampleDAO.getDataTypeExample();
        }
    }
}
