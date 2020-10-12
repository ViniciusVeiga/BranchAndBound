using Eletiva.BranchAndBound.Services;
using System;

namespace Eletiva.BranchAndBound
{
    public sealed class BranchAndBound
    {
        private readonly IMaximizer _maximizer;

        public BranchAndBound(IMaximizer maximizer) => 
            _maximizer = maximizer;

        public void Execute()
        {
            var info = new Reader().Read();

            while(true)
            {
                var result = _maximizer.Execute(info);
                if (IsResultOK(result)) break;
            }
        }

        private bool IsResultOK(Entities.Result result)
        {
            if ((result.Z % 1) == 0) return false;
            return true;
        }
    }
}
