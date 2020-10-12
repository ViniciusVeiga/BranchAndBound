using System.Collections.Generic;

namespace Eletiva.BranchAndBound.Entities
{
    public sealed class Restriction
    {
        private readonly List<VariableValue> variableValues = new List<VariableValue>();

        public Restriction(decimal value) =>
            Value = value;

        public decimal Value { get; private set; }
        public IReadOnlyCollection<VariableValue> VariableValues => variableValues;

        public void AddVariable(decimal value, Variable variable)
        {
            var variableWeight = new VariableValue(variable, value);
            variableValues.Add(variableWeight);
        }
    }
}
