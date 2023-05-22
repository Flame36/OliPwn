using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OliPwn
{
    public record SubmissionResult
    {
        public string Outcome { get; init; }
        public string Text { get; init; }
        public double Time { get; init; }
        public double Memory { get; init; }
    }
}
