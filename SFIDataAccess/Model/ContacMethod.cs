using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class ContacMethod
    {
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string MethodType { get; set; }
    }
}
