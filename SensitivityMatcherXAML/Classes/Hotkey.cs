using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SensitivityMatcherXAML.ExtensionMethods;

namespace SensitivityMatcherXAML.Classes
{
    [DataContract]
    public class Hotkey : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public uint KeyCode { get; set; }

        [DataMember]
        public Modifier ModifierKeys { get; set; }

        [DataMember]
        public string Target { get; set; }

        public bool CTRLModifier { get; set; }

        public bool ALTModifier { get; set; }

        public bool SHIFTModifier { get; set; }

        private Hotkey()
        {
        }

        public Hotkey(uint keyCode, bool ctrlMod, bool altMod, bool shiftMod, string target)
        {
            this.KeyCode        = keyCode;
            this.CTRLModifier   = ctrlMod;
            this.ALTModifier    = altMod;
            this.SHIFTModifier  = shiftMod;
            this.Target         = target;
            UpdateModifier();
        }

        /// <summary>
        /// Override the ToString method to display the current configuration in the GUI
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var kc = new KeyConverter();
            string stringToReturn = "";
            if (CTRLModifier)
                stringToReturn += "CTRL + ";
            if (ALTModifier)
                stringToReturn += "ALT + ";
            if (SHIFTModifier)
                stringToReturn += "SHIFT + ";
            return stringToReturn += this.KeyCode.KeyCodeToString();
        }

        public void ReadModifier()
        {
            if (this.ModifierKeys.HasFlag(Modifier.Ctrl))
                this.CTRLModifier = true;
            if (this.ModifierKeys.HasFlag(Modifier.Alt))
                this.ALTModifier = true;
            if (this.ModifierKeys.HasFlag(Modifier.Shift))
                this.SHIFTModifier = true;
        }

        public void UpdateModifier()
        {
            Modifier modifier = 0;
            if (CTRLModifier)
                modifier |= Modifier.Ctrl;
            else
                modifier &= ~Modifier.Ctrl;
            if (ALTModifier)
                modifier |= Modifier.Alt;
            else
                modifier &= ~Modifier.Alt;
            if (SHIFTModifier)
                modifier |= Modifier.Shift;
            else
                modifier &= ~Modifier.Shift;

            this.ModifierKeys = modifier;

            ReadModifier();
        }

        public static List<Hotkey> CreateDefaultHotkeys()
        {
            var hotkeys = new List<Hotkey>();
            hotkeys.Add(new Hotkey(((uint)Key.NumPad2).ConvertToVirtualKeyCode(), false, false, false, HotkeyTarget.TurnMultiple.ToString()));
            hotkeys.Add(new Hotkey(((uint)Key.NumPad4).ConvertToVirtualKeyCode(), false, false, false, HotkeyTarget.TurnLess.ToString()));
            hotkeys.Add(new Hotkey(((uint)Key.NumPad5).ConvertToVirtualKeyCode(), false, false, false, HotkeyTarget.TurnOnce.ToString()));
            hotkeys.Add(new Hotkey(((uint)Key.NumPad6).ConvertToVirtualKeyCode(), false, false, false, HotkeyTarget.TurnMore.ToString()));
            hotkeys.Add(new Hotkey(((uint)Key.NumPad8).ConvertToVirtualKeyCode(), false, false, false, HotkeyTarget.ClearBounds.ToString()));

            return hotkeys;
        }
    }

    [Flags]
    public enum Modifier
    {
        None = 0,
        Ctrl = 2,
        Alt = 1,
        Shift = 4
    }

    public enum HotkeyTarget
    {
        TurnLess = 0,
        TurnOnce,
        TurnMore,
        TurnMultiple,
        ClearBounds
    }
}
