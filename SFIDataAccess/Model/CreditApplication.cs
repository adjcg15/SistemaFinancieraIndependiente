using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class CreditApplication
    {
        [DataMember]
        public List<DigitalDocument> DigitalDocuments {  get; set; }

        [DataMember]
        public CreditType CreditType { get; set; }

        [DataMember]
        public CreditCondition CreditCondition { get; set; }

        //TODO: agregar asociación con cliente

        [DataMember]
        public DateTime ExpeditionDate { get; set; }

        [DataMember]
        public string Invoice { get; set; }

        [DataMember]
        public decimal MinimumAmountAccepted { get; set; }

        [DataMember]
        public string Purpose { get; set; }

        [DataMember]
        public decimal RequestedAmount {  get; set; }

        [DataMember]
        public string State { get; set; }
    }
}
