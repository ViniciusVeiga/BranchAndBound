using System;

namespace Eletiva.BranchAndBound.Entities
{
    public sealed class Rule
    {
        public Rule(Variable variable, decimal value)
        {
            Variable = variable;
            Value = value;
        }

        public Variable Variable { get; set; }
        public decimal Value { get; set; }
    }
}