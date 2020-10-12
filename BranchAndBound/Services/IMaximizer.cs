using Eletiva.BranchAndBound.Entities;
using System.Collections.Generic;

namespace Eletiva.BranchAndBound.Services
{
    public interface IMaximizer
    {
        Result Execute(Info info);
    }
}
