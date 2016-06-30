using Sharon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharonWpf
{
    class FeedbackController2 : PIController
    {
        public FeedbackController2(double p, double i, double value, double min, double max)
            : base(p, i, min, max)
        {
            Value = value;
        }

        public FeedbackController2()
        {
        }

        public FeedbackController2(double p, double i, double value)
            : this(p, i, value, double.NegativeInfinity, double.PositiveInfinity)
        {
        }

        public override double MoveNext(double input)
        {
            input = input - Current;
            if(input > Value)
                input = Value;
            else if(input < -Value)
                input = -Value;
            return base.MoveNext(input);
        }

        public double Value
        {
            get; set;
        }
    }
}
