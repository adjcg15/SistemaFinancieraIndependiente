using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class Address
    {
        [DataMember]
        public int IdAddress { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string PostCode { get; set; }

        [DataMember]
        public string Neighborhod { get; set; }

        [DataMember]
        public string Municipality { get; set; }

        [DataMember]
        public string OutdoorNumber { get; set; }

        [DataMember]
        public string InteriorNumber { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }
    }
}
