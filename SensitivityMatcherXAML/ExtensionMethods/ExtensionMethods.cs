using SensitivityMatcherXAML.Classes;
using SimpleJsonWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SensitivityMatcherXAML.ExtensionMethods
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Save all currently available settings to the Settings.json-file
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="configSettings"></param>
        /// <returns></returns>
        public static bool SaveConfigSetting(this ConfigSetting setting, List<ConfigSetting> configSettings)
        {
            if (configSettings.Any(x => x.Name == setting.Name))
            {
                var item = configSettings?.FirstOrDefault(x => x.Name == setting.Name);
                var index = configSettings.IndexOf(item);
                configSettings[index] = setting;
            }
            else
                configSettings.Add(setting);

            return JsonWrapper.WriteToJsonFile("Settings.json", configSettings);
        }

        /// <summary>
        /// Save the Hotkeys to the Hotkeys.json-file
        /// </summary>
        /// <param name="hotkeys"></param>
        /// <returns></returns>
        public static bool SaveHotkeys(this List<Hotkey> hotkeys)
        {
            return JsonWrapper.WriteToJsonFile("Hotkeys.json", hotkeys);
        }


        /// <summary>
        /// Transform the Text of the Textbox into a KeybindInfo object
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Hotkey StringToHotkey(this string text, string target)
        {
            string info = text.Replace("+", "");
            var data = info.Split(' ');
            bool bCtrl = false, bAlt = false, bShift = false;
            string key = "";
            foreach (var setting in data)
            {
                if (setting == "CTRL")
                    bCtrl = true;
                else if (setting == "ALT")
                    bAlt = true;
                else if (setting == "SHIFT")
                    bShift = true;
                else
                    key = setting;
            }

            KeyConverter kc = new KeyConverter();
            var keyCode = (uint)(Key)kc.ConvertFromString(key);
            var vkKeyCode = KeyInterop.VirtualKeyFromKey((Key)kc.ConvertFromString(key));
            
            return new Hotkey((uint)vkKeyCode, bCtrl, bAlt, bShift, target);
        }

        public static uint ConvertToVirtualKeyCode(this uint keyCode)
        {
            return (uint)KeyInterop.VirtualKeyFromKey((Key)keyCode);
        }

        public static string KeyCodeToString(this uint keyCode)
        {
            return KeyInterop.KeyFromVirtualKey((int)keyCode).ToString();
        }
    }
}
