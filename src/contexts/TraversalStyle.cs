using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.configs
{
    public class TraversalStyle
    {
        public static readonly TraversalStyle DEFAULT_TRAVERSAL_SEARCH;
        private static readonly string DTS_DESC;
        public static readonly TraversalStyle PRECENTAGE_SHIFT_SEARCH;
        private static readonly string PSS_DESC;
        public static readonly TraversalStyle STRICT_RELEVANCE_SEARCH;
        private static readonly string SRS_DESC;
        public static readonly TraversalStyle RANDOM_HALF_SEARCH;
        private static readonly string RHS_DESC;
        public static readonly TraversalStyle NEXT_DIFFERING_SEARCH;
        private static readonly string NDS_DESC;

        private string m_name;
        private string m_description;
        private string[] m_keywords;
        private static TraversalStyle[] m_traversals;

        static TraversalStyle()
        {
            DEFAULT_TRAVERSAL_SEARCH = new TraversalStyle("default traversal", new string[] { "dt", "default", "traversal", "default traversal" });
            DTS_DESC = "Based on the traversal collection, if the collection is a stack, performs depth first search, else and defaults to breadth first search";
            PRECENTAGE_SHIFT_SEARCH = new TraversalStyle("Precentage shift", new string[] { "ps", "percent", "shift", "precentage shift" });
            PSS_DESC = "The element at the 20% mark(based on enqueued amount) will be traversed to while everything prior will be requeued to the back of the queue";
            STRICT_RELEVANCE_SEARCH = new TraversalStyle("Strict relevance", new string[] { "sr", "strict", "relevance", "strict relevance" });
            SRS_DESC = "All enqueued elements will have had a parent that had a search occurence";
            RANDOM_HALF_SEARCH = new TraversalStyle("Random half", new string[] { "rh", "random",  "half", "random half" });
            RHS_DESC = "The next element will be retrieved randomly from up to half the elements in the collection";
            NEXT_DIFFERING_SEARCH = new TraversalStyle("Next differing", new string[] { "nd", "next", "differing", "next differing" });
            NDS_DESC = "The next element's host will be different if found within a reasonable time within the collection.";

            instantiateStaticMembers();
        }

        private static void instantiateStaticMembers()
        {
            m_traversals = new TraversalStyle[5];
            m_traversals[0] = DEFAULT_TRAVERSAL_SEARCH;
            m_traversals[0].setDescription(DTS_DESC);

            m_traversals[1] = PRECENTAGE_SHIFT_SEARCH;
            m_traversals[1].setDescription(PSS_DESC);

            m_traversals[2] = STRICT_RELEVANCE_SEARCH;
            m_traversals[2].setDescription(SRS_DESC);

            m_traversals[3] = RANDOM_HALF_SEARCH;
            m_traversals[3].setDescription(RHS_DESC);

            m_traversals[4] = NEXT_DIFFERING_SEARCH;
            m_traversals[4].setDescription(NDS_DESC);
        }

        private TraversalStyle(string name, string[] keywords)
        {
            m_name = name;
            m_keywords = keywords;
        }

        private void setDescription(string str) { m_description = str; }
        public string getDescription() { return m_description; }
        public string getName() { return m_name; }

        /// <summary>
        /// Returns the type of traversal base on the given traversal name
        /// returns Depth first Search traversal if no name was found
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TraversalStyle getTraversalType(string str)
        {
            // all keywords are lowercase
            str = str.ToLower();
            // check all the traversal types
            for (int i = 0; i < m_traversals.Length; i++)
            {
                // check all the keywords
                for (int j = 0; j < m_traversals[i].m_keywords.Length; j++)
                {
                    // we found the desired traversal
                    if (m_traversals[i].m_keywords[j].Equals(str))
                    {
                        return m_traversals[i];
                    }
                }
            }
            // default
            return DEFAULT_TRAVERSAL_SEARCH;
        }

        public override string ToString()
        {
            return "[ " + m_name + " ]: " + m_description + " ]";
        }

        public static string template()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_traversals.Length; i++)
            {
                sb.Append(m_traversals[i].ToString() + "\n");
            }
            return sb.ToString();
        }
    }
}
