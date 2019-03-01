using SensitivityMatcherCS.Classes;
using SimpleJsonWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensitivityMatcherXAML.ExtensionMethods
{
    public static class ExtensionMethods
    {
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
    }
}
