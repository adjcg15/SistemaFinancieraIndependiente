using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SFIDataAccess.CustomExceptions
{
    [DataContract]
    public class ServiceFault
    {
        private string message;

        public ServiceFault(string message)
        {
            this.message = message;
        }

        [DataMember]
        public string Message { get { return message; } set { message = value; } }
    }
}
