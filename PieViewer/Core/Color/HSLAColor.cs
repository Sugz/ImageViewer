using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PieViewer.Core.Color
{
    public struct HSLAColor
    {
        private double _h = 0;
        private double _s = 1;
        private double _l = 0;
        private double _a = 1;

        public double H 
        { 
            get => _h;
            set => _h = Math.Min(360, Math.Max(0, value));
        }
        public double S
        {
            get => _s;
            set => _s = Math.Min(1, Math.Max(0, value));
        }
        public double L
        {
            get => _l;
            set => _l = Math.Min(1, Math.Max(0, value));
        }

        public double A
        {
            get => _a;
            set => _a = Math.Min(1, Math.Max(0, value));
        }

        public HSLAColor()
        {
        }

        public HSLAColor(double h, double s = 1d, double l = 0.5d, double a = 1d)
        {
            H = h;
            S = s;
            L = l;
            A = a;
        }
    }
}
