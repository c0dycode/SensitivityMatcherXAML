using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SensitivityMatcherCS.Classes
{
    [DataContract]
    public class ConfigSetting
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Sens { get; set; }

        [DataMember]
        public double Yaw { get; set; }
    }
}
