using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class BankAccount
    {
        [DataMember]
        public string Card_number { get; set; }

        [DataMember]
        public string Bank { get; set; }

        [DataMember]
        public string Holder { get; set; }
    }
}
