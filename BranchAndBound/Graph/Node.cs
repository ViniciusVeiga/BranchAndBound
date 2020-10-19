using Eletiva.BranchAndBound.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace Eletiva.BranchAndBound.Graph
{
    public sealed class Node
    {
        private readonly List<Edge> _edges = new List<Edge>();

        private Node() { }
        public Node(Info info) => Info = info;

        public Info Info { get; private set; }
        public bool IsInteger { get; private set; }
        public Node Parent { get; private set; }
        public IReadOnlyCollection<Edge> Edges => _edges;

        public void AddEdge(Node to) =>
            _edges.Add(new Edge(this, to));

        public void ThisIsInteger() =>
            IsInteger = true;

        #region Clone

        public Node Clone()
        {
            var node = new Node
            {
                Info = new Info()
            };
            foreach (var variable in Info.Variables)
                node.Info.AddVariable(new Variable(variable.Description));
            foreach (var rule in Info.Rules)
            {
                var variable = GetVariable(rule.Variable.Description);
                node.Info.AddRule(variable, rule.Value);
            }
            foreach (var restriction in Info.Restrictions)
            {
                var newRestriction = new Restriction(restriction.Value);
                foreach (var variableValue in restriction.VariableValues)
                    newRestriction.AddVariable(variableValue.Value, GetVariable(variableValue.Variable.Description));
                node.Info.AddRestrition(restriction);
            }
            return node;
        }

        private Variable GetVariable(string description) => Info.Variables.First(variable => description.Equals(variable.Description));

        #endregion
    }
}
