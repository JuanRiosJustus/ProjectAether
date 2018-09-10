using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether
{
    public class ApplicationConstants
    {
        // generally applicable constants
        public static readonly int APPLICATION_WIDTH = 1020;
        public static readonly int APPLICATION_HEIGHT = 720;
        public static readonly int GREATER_STORAGE_LIMIT = 100000;
        public static readonly int LESSER_STORAGE_LIMIT = 1000;
        public static readonly string APPLICATION_NAME = "Project Aether";
        public static readonly string APPLICATION_USER_AGENT = "ProjectAether 0.1";

        // UI Text
        public static readonly string EXPLORE_TEXT = "Explore";
        public static readonly string RETURN_TEXT = "Return";
        public static readonly string ANALYZE_BUTTON_TEXT = "Analyze";

        // Lesser texts for ui
        public static readonly string STARTED_THREADLING_DEPLOYMENT = "Aetherlings are being deployed";
        public static readonly string FINISHED_THREADLING_DEPLOYMENT = "Aetherlings have all been deployed";
        public static readonly string LESSER_DIVIDER = "=====================================================";
        public static readonly string GREATER_DIVIDER = "¯`·._.··¸.-~*´¨¯¨`*·~-.,-,.-~*´¨¯¨`*·~-.¸··._.·´¯¯`·._.··¸.-~*´¨¯¨`*·~-.,-,.-~*´¨¯¨`*·~-.¸··._.·´¯¯¨`*·~-.¸··._.·´¯¯`·._.··¸.-~*´¨¯¨`*·~-.,-,.-~*´¨¯¨`*·~-.¸··._.·´¯";

        // Special identifiers
        public static readonly string CONFIGURED_SETTINGS_SHOW_DEFAULTS = "help() ";
        public static readonly string CONFIGURED_SETTINGS_SHOW_SETTINGS = "show() ";

        // Timeout  lengths that are to be used globally
        public static readonly int FIVE_SECONDS_AS_MS = 5000;
        public static readonly int TEN_SECONDS_AS_MS = 10000;
        public static readonly int SIXTY_SECONDS_AS_MS = 60000; // probably should be around a minute
        public static readonly int TIMEOUT_COUNTDOWN = 10;
    }
}
