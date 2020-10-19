using Eletiva.BranchAndBound.Graph;
using Eletiva.BranchAndBound.Services;
using System;
using System.Collections.Generic;

namespace Eletiva.BranchAndBound
{
    public sealed class BranchAndBound
    {
        private readonly IMaximizer _maximizer;
        private Stack<Node> _stack;
        private Node _node;

        public BranchAndBound(IMaximizer maximizer) => 
            _maximizer = maximizer;

        public void Execute()
        {
            _stack = new Stack<Node>();

            var info = new Reader().Read();
            var node = new Node(info);
            _stack.Push(node);

            do
            {
                _node = _stack.Peek();
                FillSucessors();

                if (TargetFound())
                {
                    ShowResult();
                    break;
                }

                foreach (var edge in _node.Edges)
                {
                    edge.SetParent(_node);
                    _stack.Push(edge.To);
                }

            } while (_stack.Count > 0);
        }

        private void ShowResult()
        {
            Console.WriteLine("Resultado encontrado!");
        }

        private bool TargetFound() => 
            _node.IsInteger;

        private void FillSucessors()
        {
            var result = _maximizer.Execute(_node.Info);
            
            if (IsInteger(result.Z))
            {
                GetChildrens(result.Z);
                return;
            }
            foreach (var variableResult in result.VariableResults)
            {
                if (IsInteger(variableResult.Value))
                {
                    GetChildrens(variableResult.Value);
                    break;
                }
            }
            _node.ThisIsInteger();
        }

        private void GetChildrens(decimal value)
        {
            var integer = Convert.ToInt32(value);
            var node1 = _node.Clone();
            var node2 = _node.Clone();
            node1.Info.AddRestrition(new Entities.Restriction(0));
            _node.AddEdge(node1);
            _node.AddEdge(node2);
        }

        private bool IsInteger(decimal value) => (value % 1) == 0;
    }
}
