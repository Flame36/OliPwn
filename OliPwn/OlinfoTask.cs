using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OliPwn
{
    public record OlinfoTask
    {
        public string Name { get; init; }
        public double TimeLimit { get; init; }
        public double MemoryLimit { get; init; }
        public string FileName { get; init; }
    }
}
