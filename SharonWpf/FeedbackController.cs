using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharon
{
    class FeedbackController : PIController
    {
        public FeedbackController(double p, double i, double min, double max)
            : base(p, i, min, max)
        {
        }

        public FeedbackController()
        {
        }

        public FeedbackController(double p, double i)
            : base(p, i, double.NegativeInfinity, double.PositiveInfinity)
        {
        }

        public override double MoveNext(double input)
        {
            return base.MoveNext(input - Current);
        }
    }
}
