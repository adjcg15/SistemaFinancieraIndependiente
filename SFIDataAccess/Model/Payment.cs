using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class Payment
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public double amount { get; set; }

        [DataMember]
        public string invoice { get; set; }

        [DataMember]
        public DateTime planned_date { get; set; }

        [DataMember]
        public string credit_invoice { get; set; }

        [DataMember]
        public DateTime? reconciliation_date { get; set; }
    }
}