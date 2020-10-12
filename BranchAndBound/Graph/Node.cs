using Eletiva.BranchAndBound.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eletiva.BranchAndBound.Graph
{
    public sealed class Node
    {
        private readonly List<Edge> _edges = new List<Edge>();

        public Node(Info info)
        {
            Info = info;
        }

        public Info Info { get; private set; }
        public Node Parent { get; private set; }
        public IReadOnlyCollection<Edge> Edges => _edges;
    }
}
