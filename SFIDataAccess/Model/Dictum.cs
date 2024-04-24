using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class Dictum
    {
        [DataMember]
        public DateTime GenerationDate { get; set; }

        [DataMember]
        public bool IsApproved { get; set; }

        [DataMember]
        public string Justification { get; set; }
    }
}
