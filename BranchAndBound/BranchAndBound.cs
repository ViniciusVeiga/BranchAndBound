using Eletiva.BranchAndBound.Graph;
using Eletiva.BranchAndBound.Services;
using System;
using System.Collections.Generic;

namespace Eletiva.BranchAndBound
{
    public sealed class BranchAndBound
    {
        private readonly IMaximizer _maximizer;
        private Queue<Node> _queue;

        public BranchAndBound(IMaximizer maximizer) => 
            _maximizer = maximizer;

        public void Execute()
        {
            _queue = new Queue<Node>();

            var info = new Reader().Read();
            var node = new Node(info);
            _queue.Enqueue(node);

            do
            {
                var n = _queue.Dequeue();
                FillSucessors(n);


            } while (_queue.Count > 0);
        }

        private void FillSucessors(Node node)
        {
            var result = _maximizer.Execute(node.Info);
        }

        private bool IsResultOK(Entities.Result result)
        {
            if ((result.Z % 1) == 0) return false;
            return true;
        }
    }
}
