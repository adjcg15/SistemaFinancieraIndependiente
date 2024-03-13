using SFIDataAccess.Model;
using SFIServices.Contracts;

namespace SFIServices
{
    public partial class SFIService : IServiceExample
    {
        public DataTypeExample GetDataUsingDataContract()
        {
            DataTypeExample dataType = new DataTypeExample();
            dataType.IsExample = true;
            dataType.Name = "Example";

            return dataType;
        }
    }
}
