using Eletiva.BranchAndBound;
using Eletiva.Simplex;
using System;

namespace Eletive.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var simplex = new Simplex();
            var branchAndBound = new BranchAndBound(simplex);
            branchAndBound.Execute();
        }
    }
}
