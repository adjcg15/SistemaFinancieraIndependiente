using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class Employee
    {
        [DataMember]
        public string EmployeeNumber { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public EmployeeRole EmployeeRole { get; set; }
    }
}
