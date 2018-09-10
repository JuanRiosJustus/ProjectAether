namespace ProjectAether.src.configs
{
    public static class ConfiguredConstants
    {
        // Search keywords
        public static readonly string ENTRY_POINT_LBL = "entryPoint";
        public static readonly string ENTRY_POINT_DESC = "Defines the starting search page/engine to begin aggregation on";
        public static readonly string SEARCH_PHRASE_LBL = "searchPhrase";
        public static readonly string SEARCH_PHRASE_DESC = "The text that ill be looked for and compared to";
        public static readonly string TRAVERSAL_STYLE_LBL = "traversalStyle";
        public static readonly string TRAVERSAL_STYLE_DESC = "Determines how each webpage is to be visited";
        public static readonly string SEARCH_LIMIT_LBL = "searchLimit";
        public static readonly string SEARCH_LIMIT_DESC = "After reaching this limit, haults the crawling process";
        public static readonly string THREADLING_COUNT_LBL = "threadlings";
        public static readonly string THREADLING_COUNT_DESC = "Dictates the speed of aggregation and RAM in use";
        public static readonly string USE_QUEUE_LBL = "useQueue";
        public static readonly string USE_QUEUE_DESC = "Decides the internal ordering of the webpage traversal";
        public static readonly string USE_FILTER_LBL = "useFilter";
        public static readonly string USE_FILTER_DESC = "Removes text from webpages which contain code, comments, and etc";
        public static readonly string ALL_PARAMS = "entryPoint=\"\" searchPhrase=\"\" traversalStyle=\"\" searchLimit=\"\" threadlings=\"\" useQueue=\"\" useFilter=\"\"";
        public static readonly string TEST_PARAMS = "show() searchPhrase=\"\" traversalStyle=\"sr\" threadlings=\"2\" ";
    }
}
