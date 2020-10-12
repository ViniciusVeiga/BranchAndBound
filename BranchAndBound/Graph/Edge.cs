using System;
using System.Collections.Generic;
using System.Text;

namespace Eletiva.BranchAndBound.Graph
{
    public sealed class Edge
    {
        public Edge(Node from, Node to) : this(from, to, 0) { }
        public Edge(Node from, Node to, double cost)
        {
            From = from;
            To = to;
            Cost = cost;
        }

        public Node From { get; private set; }
        public Node To { get; private set; }
        public double Cost { get; private set; }
    }
}
