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
        public string capture_line { get; set; }
        [DataMember]
        public DateTime generation_date { get; set; }

        [DataMember]
        public int id_payment { get; set; }
    }
}
