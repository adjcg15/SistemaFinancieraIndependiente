using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class CreditCondition
    {
        [DataMember]
        public CreditType CreditType { get; set; }

        [DataMember]
        public double AdvancePaymentReduction { get; set; }

        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public double InterestOnArrears { get; set; }

        [DataMember]
        public double InterestRate { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsIvaApplied { get; set; }

        [DataMember]
        public int PaymentMonths { get; set; }
    }
}
