using System;
using System.Collections.Generic;
using System.Text;

namespace Eletiva.BranchAndBound.Entities
{
    public class VariableValue
    {
        public VariableValue(Variable variable, decimal value)
        {
            Variable = variable;
            Value = value;
        }

        public Variable Variable { get; private set; }
        public decimal Value { get; private set; }
    }
}
