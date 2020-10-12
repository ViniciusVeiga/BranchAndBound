using System;
using System.Collections.Generic;
using System.Text;

namespace Eletiva.BranchAndBound.Entities
{
    public sealed class Variable : Entity
    {
        public Variable(string description) => 
            Description = description;

        public string Description { get; private set; }
    }
}
