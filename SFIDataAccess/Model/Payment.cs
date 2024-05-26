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
        public int Id { get; set; }

        [DataMember]
        public double Amount { get; set; }

        [DataMember]
        public string Invoice { get; set; }

        [DataMember]
        public DateTime PlannedDate { get; set; }

        [DataMember]
        public string CreditInvoice { get; set; }

        [DataMember]
        public DateTime? ReconciliationDate { get; set; }

        [DataMember]
        public decimal Interest { get; set; }
    }
}