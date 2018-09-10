namespace ProjectAether.src.structures
{
    public class Graph<T>
    {
        private T[] graphNodes = new T[0];
        private int[] graphWeights = new int[0];
        private int nodeCount = 0;
        private int spacingNeeded = 0;

        /// <Summary>
        /// Returns true if the from and to nodes have been connected
        /// </Summary>
        public bool connect(T fromNode, T toNode, int weight)
        {
            int indexFrom = indexOf(fromNode);
            int indexTo = indexOf(toNode);
            if (indexFrom < 0 || indexTo < 0 || weight > 1000000) { return false; }
            setWeight(indexFrom, indexTo, weight);
            ensureEnoughSpaceing(weight.ToString());
            return true;
        }

        /// <Summary>
        /// Returns true if the from and to nodes have been connected
        /// </Summary>
        public bool connect(T fromNode, T toNode)
        {
            int indexFrom = indexOf(fromNode);
            int indexTo = indexOf(toNode);
            if (indexFrom < 0 || indexTo < 0) { return false; }
            setWeight(indexFrom, indexTo, 1);
            return true;
        }

        /// <Summary>
        /// Returns true if the from and to nodes have been disconnect
        /// </Summary>
        public bool disconnect(T fromNode, T toNode)
        {
            int indexFrom = indexOf(fromNode);
            int indexTo = indexOf(toNode);
            if (indexFrom < 0 || indexTo < 0) { return false; }
            setWeight(indexFrom, indexTo, 0);
            return true;
        }

        /// <Summary>
        /// Returns the weight between the from and to node if directly linked.
        /// </Summary>
        public int weightBetween(T fromNode, T toNode)
        {
            int indexFrom = indexOf(fromNode);
            int indexTo = indexOf(toNode);
            if (indexFrom < 0 || indexTo < 0)
            {
                return 0;
            }
            else
            {
                return getWeight(indexFrom, indexTo);
            }
        }

        /// <Summary>
        /// Returns true if there is a edge between the from and to nodes that is a non-zero value
        /// </Summary>
        public bool isAdjacent(T fromNode, T toNode)
        {
            int indexFrom = indexOf(fromNode);
            int indexTo = indexOf(toNode);
            if (indexFrom < 0 || indexTo < 0)
            {
                return false;
            }
            else
            {
                return getWeight(indexFrom, indexTo) != 0;
            }
        }

        /// <Summary>
        /// Returns the index of the given node within the graph
        /// </Summary>
        public int indexOf(T node)
        {
            int i = 0;
            while (i < nodeCount)
            {
                if (graphNodes[i].Equals(node)) { return i; }
                i++;
            }
            return -1;
        }

        /// <Summary>
        /// Adds the given node to the next available space in the graph
        /// </Summary>
        public void add(T node)
        {
            ensureNodeSpace();
            ensureWeightSpace();
            graphNodes[nodeCount] = node;
            ensureEnoughSpaceing(node.ToString());
            nodeCount++;
        }

        /// <Summary>
        /// Sets the node at the given index to the given node.
        /// </Summary>
        public void set(T node, int index)
        {
            if (index > nodeCount - 1) { return; }
            graphNodes[index] = node;
            ensureEnoughSpaceing(node.ToString());
        }

        /// <Summary>
        /// Sets the node at the position of the deleting node to the given node.
        /// </Summary>
        public void set(T node, T deletingNode)
        {
            int index = indexOf(deletingNode);
            if (index > nodeCount - 1) { return; }
            graphNodes[index] = node;
            ensureEnoughSpaceing(node.ToString());
        }

        /// <Summary>
        /// Removes the node at the given index within the graph
        /// </Summary>
        public void delete(int index)
        {
            if (index > nodeCount - 1) { return; }
            // shift the header to the left
            for (int i = index; i < nodeCount; i++)
            {
                graphNodes[i] = graphNodes[i + 1];
            }
            // everything in that nodes row and column should be deleted
            for (int i = 0; i < nodeCount; i++)
            {
                setWeight(i, index, 0);
                setWeight(index, i, 0);
            }
            // shift everything on a row, to the left once
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = index; j < nodeCount; j++)
                {
                    setWeight(i, j, getWeight(i, j + 1));
                }
            }
            // move everything up to the column that was deleted
            for (int i = index; i < nodeCount; i++)
            {
                for (int j = 0; j < nodeCount; j++)
                {
                    setWeight(i, j, getWeight(i + 1, j));
                }
            }
            nodeCount--;
        }

        /// <Summary>
        /// Removes the given node from the graph
        /// </Summary>
        public void delete(T node) { delete(indexOf(node)); }

        /// <Summary>
        ///  Returns the node at the given index within the graph
        /// </Summary>
        public T get(int index) { return graphNodes[index]; }

        /// <Summary>
        /// Returns a list of all neighbor nodes the given node has them
        /// </Summary>
        public T[] connections(T node)
        {
            int index = indexOf(node);
            T[] connectedNodes = new T[edges(node)];
            for (int i = 0, j = 0; i < nodeCount; i++)
            {
                if (getWeight(index, i) != 0)
                {
                    connectedNodes[j] = graphNodes[i];
                    j++;
                }
            }
            return connectedNodes;
        }
        /// <Summary>
        /// Returns the amount of edges the given node has to other nodes
        /// </Summary>
        public int edges(T node)
        {
            int count = 0;
            int index = indexOf(node);
            for (int i = 0; i < nodeCount; i++)
            {
                if (getWeight(index, i) != 0) { count++; }
            }
            return count;
        }

        
        // Ensure that there is adequate room for more nodes.
        private void ensureNodeSpace()
        {
            if (nodeCount < graphNodes.Length) { return; }
            T[] newGraphOfNodes = new T[graphNodes.Length * 3 / 2 + 1];
            for (int i = 0; i < graphNodes.Length; i++) { newGraphOfNodes[i] = graphNodes[i]; }
            graphNodes = newGraphOfNodes;
        }
        // Ensures adequate amount of room for weights of edges
        private void ensureWeightSpace()
        {
            if (nodeCount < System.Math.Sqrt(graphWeights.Length)) { return; }
            int[] newGraphOfWeights = new int[(int)System.Math.Pow(graphWeights.Length * 3 / 2 + 1, 2)];
            for (int i = 0; i < graphWeights.Length; i++)
            {
                newGraphOfWeights[i] = graphWeights[i];
            }
            graphWeights = newGraphOfWeights;
        }
        // Ensures that the amount of spacing needed is accounted for
        private void ensureEnoughSpaceing(string str)
        {
            if (str.Length > spacingNeeded)
            {
                spacingNeeded = str.Length;
            }
        }
        // Ensures enough spaces for output
        private string addEnoughSpaces(string str)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(str);
            while (sb.Length < spacingNeeded)
            {
                sb.Insert(0, ' ');
            }
            return sb.ToString();
        }
        // Sets the weight st the given row, col
        private void setWeight(int row, int col, int val)
        {
            graphWeights[nodeCount * row + col] = val;
        }
        // Gets the weight at the given row, col
        private int getWeight(int row, int col)
        {
            return graphWeights[nodeCount * row + col];
        }

        /// <Summary>
        /// Returns true if the given node is already contained within the graph
        /// </Summary>
        public bool contains(T node) { return indexOf(node) > -1; }

        /// <Summary>
        /// Returns the amount of nodes within the graph
        /// </Summary>
        public int size() { return nodeCount; }

        /// <Summary>
        /// Returns a numerical representation of the graph
        /// </Summary>
        public int[,] toMultiDimensionalArray()
        {
            int[,] nodeArray = new int[nodeCount + 1, nodeCount + 1];
            for (int i = 0; i < nodeCount + 1; i++)
            {
                nodeArray[i, 0] = i - 1;
                nodeArray[0, i] = i - 1;
            }
            for (int i = 1; i < nodeCount + 1; i++)
            {
                for (int j = 1; j < nodeCount + 1; j++)
                {
                    nodeArray[i, j] = getWeight(i - 1, j - 1);
                }
            }
            return nodeArray;
        }
        /// <Summary>
        /// Returns a string representation of the Graph
        /// </Summary>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("[" + addEnoughSpaces("") + "]");
            // make the very first row
            for (int i = 0; i < nodeCount; i++) { sb.Append("[" + addEnoughSpaces(graphNodes[i].ToString()) + "]"); }
            sb.Append("\r\n");
            // make the rest of the rows
            for (int i = 0; i < nodeCount; i++)
            {
                sb.Append("[" + addEnoughSpaces(graphNodes[i].ToString()) + "]");
                for (int j = 0; j < nodeCount; j++)
                {
                    sb.Append("[" + addEnoughSpaces(getWeight(i, j).ToString()) + "]");
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
    }
}
