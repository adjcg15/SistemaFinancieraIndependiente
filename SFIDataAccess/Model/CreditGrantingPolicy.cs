﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class CreditGrantingPolicy
    {
        [DataMember]
        public int Identifier { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime EffectiveDate { get; set; }
        [DataMember]
        public Boolean IsActive { get; set; }

        [DataMember]
        public string CreditApllicationInvoice {  get; set; }
    }
}
