using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharon
{
    class PIController
    {
        public PIController()
        {
        }

        public PIController(double p, double i, double min, double max)
        {
            P = p;
            I = i;
            Min = min;
            Max = max;
        }

        public double P
        {
            get;
            set;
        }

        public double I
        {
            get;
            set;
        }

        public double Max
        {
            get;
            set;
        }

        public double Min
        {
            get;
            set;
        }

        private double _i, _c;

        public double Intergreted
        {
            get
            {
                return _i;
            }
            set
            {
                if(value > Max)
                    value = Max;
                if(value < Min)
                    value = Min;
                _i = value;
            }
        }

        public double Current
        {
            get
            {
                return _c;
            }
            private set
            {
                if(value > Max)
                    value = Max;
                if(value < Min)
                    value = Min;
                _c = value;
            }
        }

        public virtual double MoveNext(double input)
        {
            Intergreted += I * input;
            var proported = P * input;
            Current = Intergreted + proported;
            return Current;
        }

        public void Reset()
        {
            Current = 0;
            Intergreted = 0;
        }
    }
}
