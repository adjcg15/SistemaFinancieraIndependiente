using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    internal class Regime
    {
        [DataMember]
        public DateTime application_end_date { get; set; }
        [DataMember]
        public DateTime application_start_date { get; set; }
        [DataMember]
        public string credit_condition_identifier { get; set; }
        [DataMember]
        public string credit_invoice { get; set; }
    }
}