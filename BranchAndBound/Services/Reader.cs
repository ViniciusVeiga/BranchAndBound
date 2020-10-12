using Eletiva.BranchAndBound.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eletiva.BranchAndBound.Services
{
    public class Reader
    {
        private readonly List<Variable> variables = new List<Variable>();
        private string _file;

        public Info Read()
        {
            //_file = File.ReadAllText("Files/exemplo.txt");
            var info = new Info();
            var variableA = new Variable("a");
            var variableB = new Variable("b");
            info.AddVariable(variableA);
            info.AddVariable(variableB);
            info.AddRule(variableA, 3);
            info.AddRule(variableB, 8);
            var restriction1 = new Restriction(7);
            restriction1.AddVariable(3, variableA);
            restriction1.AddVariable(1, variableB);
            info.AddRestrition(restriction1);
            var restriction2 = new Restriction(5);
            restriction2.AddVariable(1, variableA);
            restriction2.AddVariable(2, variableB);
            info.AddRestrition(restriction2);
            return info;
        }
    }
}
