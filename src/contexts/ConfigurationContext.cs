using ProjectAether.src.configs;
using ProjectAether.src.globals;
using ProjectAether.src.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectAether.src.contexts
{
    public class ConfigurationContext : BaseContext
    {
        private ConfiguredSettings configuredSettings = null;
        private Dictionary<string, string> defaultEntrySearchEngines = new Dictionary<string, string>();

        public ConfigurationContext(ConcurrentQueue<string> mainDisplayQueue, ConcurrentQueue<string> secondaryDisplayQueue, string query) : base(mainDisplayQueue, secondaryDisplayQueue)
        {
            defaultEntrySearchEngines.Add("google", "https://www.google.com/search?q=");
            defaultEntrySearchEngines.Add("bing", "https://www.bing.com/search?q=");
            defaultEntrySearchEngines.Add("reddit", "https://www.reddit.com/search?q=");
            defaultEntrySearchEngines.Add("yahoo", "https://search.yahoo.com/search?p=");
            defaultEntrySearchEngines.Add("youtube", "https://www.youtube.com/results?search_query=");
            defaultEntrySearchEngines.Add("britannica", "https://www.britannica.com/search?query=");
            defaultEntrySearchEngines.Add("wikipedia", "https://en.wikipedia.org/w/index.php?search=");
            defaultEntrySearchEngines.Add("yahooAnswers", "https://answers.search.yahoo.com/search?p=");

            // this must be called after adding all elements to the map
            configuredSettings = constructConfiguredSettings(query);
            mainDisplayQueue.Enqueue(ApplicationConstants.GREATER_DIVIDER + "\r\n");
            mainDisplayQueue.Enqueue(configuredSettings.ToString() + "\r\n");
        }
        /// <summary>
        /// populates the map of configured parameters
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private ConfiguredSettings constructConfiguredSettings(string query)
        {
            Dictionary<string, ConfiguredParameter> settings = new Dictionary<string, ConfiguredParameter>();
            // get the entry point, this is must have a value
            string parameterValue = getWellFormedAndContainedParamter(query, ConfiguredConstants.ENTRY_POINT_LBL);
            if (parameterValue != null)
            {
                string attemptedConversion = tryConvertEntryPoint(parameterValue);
                settings.Add(ConfiguredConstants.ENTRY_POINT_LBL, new ConfiguredParameter(ConfiguredConstants.ENTRY_POINT_DESC, attemptedConversion));
            }
            else
            {
                settings.Add(ConfiguredConstants.ENTRY_POINT_LBL, new ConfiguredParameter(ConfiguredConstants.ENTRY_POINT_DESC, defaultEntrySearchEngines[getRandomEntryPointKey()]));
            }
            // a term to search for is required, or else its just looking up this programs name "into the aether"
            parameterValue = getWellFormedAndContainedParamter(query, ConfiguredConstants.SEARCH_PHRASE_LBL);
            if (parameterValue != null)
            {
                settings.Add(ConfiguredConstants.SEARCH_PHRASE_LBL, new ConfiguredParameter(ConfiguredConstants.SEARCH_PHRASE_DESC, parameterValue));
            }
            else
            {
                settings.Add(ConfiguredConstants.SEARCH_PHRASE_LBL, new ConfiguredParameter(ConfiguredConstants.SEARCH_PHRASE_DESC, ApplicationConstants.APPLICATION_NAME));
            }
            // gets the collection for traversals to use
            parameterValue = getWellFormedAndContainedParamter(query, ConfiguredConstants.USE_QUEUE_LBL);
            if (parameterValue != null)
            {
                settings.Add(ConfiguredConstants.USE_QUEUE_LBL, new ConfiguredParameter(ConfiguredConstants.USE_QUEUE_DESC, parameterValue.ToBoolean().ToString()));
            }
            else
            {
                settings.Add(ConfiguredConstants.USE_QUEUE_LBL, new ConfiguredParameter(ConfiguredConstants.USE_QUEUE_DESC, bool.TrueString));
            }

            // changes the traversal style to either 
            parameterValue = getWellFormedAndContainedParamter(query, ConfiguredConstants.TRAVERSAL_STYLE_LBL);
            if (parameterValue != null)
            {
                string temp = TraversalStyle.getTraversalType(parameterValue).getName();
                settings.Add(ConfiguredConstants.TRAVERSAL_STYLE_LBL, new ConfiguredParameter(ConfiguredConstants.TRAVERSAL_STYLE_DESC, temp));
            }
            else
            {
                settings.Add(ConfiguredConstants.TRAVERSAL_STYLE_LBL, new ConfiguredParameter(ConfiguredConstants.TRAVERSAL_STYLE_DESC, TraversalStyle.DEFAULT_TRAVERSAL_SEARCH.getName()));
            }

            // get the amount of searches
            parameterValue = getWellFormedAndContainedParamter(query, ConfiguredConstants.SEARCH_LIMIT_LBL);
            if (parameterValue != null && parameterValue.Length < 6 && parameterValue.IsNumeric())
            {
                settings.Add(ConfiguredConstants.SEARCH_LIMIT_LBL, new ConfiguredParameter(ConfiguredConstants.SEARCH_LIMIT_DESC, parameterValue));
            }
            else
            {
                settings.Add(ConfiguredConstants.SEARCH_LIMIT_LBL, new ConfiguredParameter(ConfiguredConstants.SEARCH_LIMIT_DESC, ApplicationConstants.GREATER_STORAGE_LIMIT + ""));
            }
            // determines the amount of threads the user wants to use, seriously, no more than 10 threads, even this is meh
            parameterValue = getWellFormedAndContainedParamter(query, ConfiguredConstants.THREADLING_COUNT_LBL);
            if (parameterValue != null && parameterValue.IsNumeric() && Int32.Parse(parameterValue) > 0 && Int32.Parse(parameterValue) < 10)
            {
                settings.Add(ConfiguredConstants.THREADLING_COUNT_LBL, new ConfiguredParameter(ConfiguredConstants.THREADLING_COUNT_DESC, Int32.Parse(parameterValue) + ""));
            }
            else
            {
                settings.Add(ConfiguredConstants.THREADLING_COUNT_LBL, new ConfiguredParameter(ConfiguredConstants.THREADLING_COUNT_DESC, "1"));
            }

            // determines if the user wants the raw gathered information
            parameterValue = getWellFormedAndContainedParamter(query, ConfiguredConstants.USE_FILTER_LBL);
            if (parameterValue != null)
            {
                settings.Add(ConfiguredConstants.USE_FILTER_LBL, new ConfiguredParameter(ConfiguredConstants.USE_FILTER_DESC, parameterValue.ToBoolean().ToString()));
            }
            else
            {
                settings.Add(ConfiguredConstants.USE_FILTER_LBL, new ConfiguredParameter(ConfiguredConstants.USE_FILTER_DESC, Boolean.TrueString));
            }
            
            return new ConfiguredSettings(settings);
        }
        /// <summary>
        /// If the entry point is a key withi the default entry points,
        /// then we can use the default supported value
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string tryConvertEntryPoint(string str)
        {
            if (defaultEntrySearchEngines.ContainsKey(str))
            {
                return defaultEntrySearchEngines[str];
            }
            return str;
        }

        /// <summary>
        /// Fetches the value within the parameter
        /// </summary>
        /// <param name="str"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private string getWellFormedAndContainedParamter(string str, string param)
        {
            str = str.ToLower();
            param = param.ToLower();
            int index = str.IndexOf(param);
            // didnt find the parameter
            if (index == -1) { return null; }
            // get to the equality sign, where it should be the next char after the parameter
            index += param.Length;
            if (index > str.Length) { return null; }
            if (str[index] != '=') { return null; }
            index++;
            // check that the first quote is there and the parameter is not empty
            if (index > str.Length) { return null; }
            if (str[index] != '"') { return null; }
            if (index + 1 > str.Length || str[index + 1] == '"') { return null; }
            // get the value between the quotes
            int endIndex = index + 1;
            if (endIndex > str.Length) { return null; }
            StringBuilder sb = new StringBuilder();
            // find the last quote, and append until its found or index is out of bounds
            while (str[endIndex] != '"')
            {
                sb.Append(str[endIndex]);
                endIndex++;
                if (endIndex > str.Length) { return null; }
            }
            return sb.ToString().ToLower();
        }
        /// <summary>
        /// retrieves a random key from the map of default entry points
        /// </summary>
        /// <returns></returns>
        private string getRandomEntryPointKey()
        {
            Random rand = new Random();
            return defaultEntrySearchEngines.ElementAt(rand.Next(0, defaultEntrySearchEngines.Count)).Key;
        }

        public ConfiguredSettings releaseConfiguredSettings()
        {
            ConfiguredSettings temp = configuredSettings;
            configuredSettings = null;
            return temp;
        }
    }
}
