using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SensitivityMatcherXAML.Classes
{
    [DataContract]
    public class Hotkey
    {
        [DataMember]
        public uint KeyCode { get; set; }

        [DataMember]
        public uint ModifierKeys { get; set; }

        public Hotkey()
        {
            this.KeyCode        = 0;
            this.ModifierKeys   = 0;
        }

        public Hotkey(uint keyCode, uint modifierKeys)
        {
            this.KeyCode        = keyCode;
            this.ModifierKeys   = modifierKeys;
        }
    }
}
