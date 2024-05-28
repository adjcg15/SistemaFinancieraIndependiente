using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [DataMember]
        public int IdAddress { get; set; }

        [DataMember]
        public int IdWorkCenter { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public bool HasCreditApplication { get; set; }

        [DataMember]
        public bool HasActiveCredit { get; set; }

        [DataMember]
        public Address Address { get; set; }

        [DataMember]
        public WorkCenter WorkCenter { get; set; }

        [DataMember]
        public BankAccount BankAccount { get; set; }

        [DataMember]
        public List<ContacMethod> ContacMethods { get; set; }

        [DataMember]
        public List<PersonalReference> PersonalReferences { get; set; }
    }
}