using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class DigitalDocument
    {
        [DataMember]
        public byte[] Content { get; set; }

        [DataMember]
        public string Format { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
