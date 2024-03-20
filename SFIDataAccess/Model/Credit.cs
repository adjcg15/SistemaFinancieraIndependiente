using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class Credit
    {
        [DataMember]
        public decimal AmountApproved { get; set; }

        [DataMember]
        public DateTime ApprovalDate { get; set; }

        [DataMember]
        public string Invoice { get; set; }

        [DataMember]
        public Nullable<DateTime> SettlementDate { get; set; }

        [DataMember]
        public DateTime WithdrawalDate { get; set; }

        [DataMember]
        public CreditType CreditType { get; set; }

        [DataMember]
        public CreditCondition CreditCondition { get; set; }

        [DataMember]
        public CreditApplication CreditApplication { get; set; }

        [DataMember]
        public Client Client { get; set; }
    }
}
