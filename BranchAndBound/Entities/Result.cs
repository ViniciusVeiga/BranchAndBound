using System;
using System.Collections.Generic;
using System.Text;

namespace Eletiva.BranchAndBound.Entities
{
    public sealed class Result
    {
        public decimal Z { get; set; }
        public IEnumerable<VariableResult> VariableResults { get; set; }

        public sealed class VariableResult
        {
            public Variable Variable { get; set; }
            public decimal Value { get; set; }
        }
    }

}
