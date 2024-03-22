using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class WorkCenter
    {
        [DataMember]
        public string CompanyName { get; set; }
        
        [DataMember]
        public string EmployeeSeniority { get; set; }

        [DataMember]
        public string HumanResourcesPhone { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public decimal Salary { get; set; }

        [DataMember]
        public string EmployeePosition { get; set; }

        [DataMember]
        public Address Address { get; set; }
    }
}
