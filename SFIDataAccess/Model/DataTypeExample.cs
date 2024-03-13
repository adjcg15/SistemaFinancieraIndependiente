using System.Runtime.Serialization;

namespace SFIDataAccess.Model
{
    [DataContract]
    public class DataTypeExample
    {
        bool isExample = true;
        string name = "Hello ";

        [DataMember]
        public bool IsExample
        {
            get { return isExample; }
            set { isExample = value; }
        }

        [DataMember]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
