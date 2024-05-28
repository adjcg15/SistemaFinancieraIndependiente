using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    public class PaymentLayout
    {
        [DataMember]
        public string CaptureLine { get; set; }

        [DataMember]
        public DateTime GenerationDate { get; set; }

        [DataMember]
        public int IdPayment { get; set; }
    }
}