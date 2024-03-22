using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class PersonalReference
    {
        [DataMember]
        public string IneKey { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public string Kinship { get; set; }

        [DataMember]
        public string RelationshipYears { get; set; }

        [DataMember]
        public Address Address { get; set; }
    }
}
