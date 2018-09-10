using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectAether.src.configs
{
    public class ConfiguredSettings
    {
        private Dictionary<string, ConfiguredParameter> configurationMap;

        public ConfiguredSettings(Dictionary<string, ConfiguredParameter> configs) { configurationMap = configs; }

        public string get(string configName)
        {
            if (configurationMap.ContainsKey(configName))
            {
                return configurationMap[configName].getValue();
            }
            else
            {
                return String.Empty;
            }
        }

        public void setSearchPhrase(string phrase)
        {
            configurationMap[ConfiguredConstants.SEARCH_PHRASE_LBL] = new ConfiguredParameter(ConfiguredConstants.SEARCH_PHRASE_DESC, phrase);
        }

        public int configurations() { return configurationMap.Count; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(string str in configurationMap.Keys)
            {
                sb.Append("[" + str + "]: " + configurationMap[str].getValue() + "\n");
            }
            return sb.ToString();
        }

        public string template()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in configurationMap.Keys)
            {
                sb.Append("[ " + str + " ]:  " + configurationMap[str].getDescription() + " HowToSet( " + str +"=\"value\" )\n");
            }
            return sb.ToString();
        }
    }
}
