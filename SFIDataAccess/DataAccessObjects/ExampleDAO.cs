using SFIDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.DataAccessObjects
{
    public class ExampleDAO
    {
        public static DataTypeExample getDataTypeExample()
        {
            DataTypeExample dataType = new DataTypeExample();
            dataType.IsExample = true;
            dataType.Name = "Example";

            return dataType;
        }
    }
}
