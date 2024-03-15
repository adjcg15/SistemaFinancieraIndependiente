using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class CreditType
    {
        public int Identifier {get; set;}
        public string Name { get; set;}
    }
}
