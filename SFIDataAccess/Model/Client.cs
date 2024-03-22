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
        public int Id_address { get; set; }

        [DataMember]
        public int Id_work_center { get; set; }

        [DataMember]
        public string Card_number { get; set; }

        [DataMember]
        public bool Has_credit_application { get; set; }

        [DataMember]
        public bool Has_active_credit { get; set; }

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