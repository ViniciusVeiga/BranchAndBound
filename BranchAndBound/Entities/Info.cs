using System.Collections.Generic;

namespace Eletiva.BranchAndBound.Entities
{
    public sealed class Info
    {
        private readonly List<Variable> _variables = new List<Variable>();
        private readonly List<Rule> _rules = new List<Rule>();
        private readonly List<Restriction> _restriction = new List<Restriction>();

        public IReadOnlyCollection<Variable> Variables => _variables;
        public IReadOnlyCollection<Rule> Rules => _rules;
        public IReadOnlyCollection<Restriction> Restrictions => _restriction;

        public void AddRule(Variable variable, decimal value)
        {
            var rule = new Rule(variable, value);
            _rules.Add(rule);
        }

        public void AddRestrition(Restriction restriction) => 
            _restriction.Add(restriction);

        public void AddVariable(Variable variable) =>
            _variables.Add(variable);
    }
}
