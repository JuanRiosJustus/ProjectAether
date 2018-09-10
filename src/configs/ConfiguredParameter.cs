namespace ProjectAether.src.configs
{
    public class ConfiguredParameter
    {
        private readonly string name;
        private readonly string value;

        public ConfiguredParameter(string parameterName, string parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }
        public string getDescription() { return name; }
        public string getValue() { return value; }
    }
}
