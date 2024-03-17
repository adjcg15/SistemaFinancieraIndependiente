using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class Client
    {
        [DataMember]
        public DateTime Birthdate { get; set; }

        [DataMember]
        public string Curp { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Rfc { get; set; }

        [DataMember]
        public string Surname { get; set; }
    }
}
