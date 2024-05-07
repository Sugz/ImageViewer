using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BenchmarkApp
{
    [MemoryDiagnoser]
    public class Benchmark
    {
        Random rnd = new Random();


        private int Size = 150;
        private int _halfSize;
        private int _halfSizeSquared;
        private int _innerRadius;
        private int _innerRadiusSquared;
        double _angle = 0;
        private Point _center;
        readonly double sqrt3 = Math.Sqrt(3);


        private Point[] triangle = new Point[3] {
            new Point(75, 17.4),
            new Point(124.8, 104.1),
            new Point(25.3, 104.1),
        };

        private Point[] points;

        public Benchmark()
        {
            _halfSize = Size / 2;
            _halfSizeSquared = _halfSize * _halfSize;
            _innerRadius = _halfSize - 15;
            _innerRadiusSquared = _innerRadius * _innerRadius;
            _center = new(_halfSize, _halfSize);


            points = Enumerable.Range(0, 100).Select(x => new Point(rnd.Next(150), rnd.Next(150))).ToArray();
        }




        private static bool IsPointInHueCircle(Point point, Point center, int halfSizeSquared, int innerRadiusSquared)
        {
            double distanceSquared = (center - point).LengthSquared;
            return distanceSquared <= halfSizeSquared && distanceSquared >= innerRadiusSquared;
        }

        private static bool IsPointInHueCircle2(Point point, Point center, int halfSizeSquared, int innerRadiusSquared)
        {
            double distanceSquared = (center - point).LengthSquared;
            if (distanceSquared > halfSizeSquared)
                return false;
            if (distanceSquared >= innerRadiusSquared)
                return true;
            return false;
        }

        private bool PointInTriangle(Point p, Point p0, Point p1, Point p2)
        {
            var s = (p0.X - p2.X) * (p.Y - p2.Y) - (p0.Y - p2.Y) * (p.X - p2.X);
            var t = (p1.X - p0.X) * (p.Y - p0.Y) - (p1.Y - p0.Y) * (p.X - p0.X);

            if ((s < 0) != (t < 0) && s != 0 && t != 0)
                return false;

            var d = (p2.X - p1.X) * (p.Y - p1.Y) - (p2.Y - p1.Y) * (p.X - p1.X);
            return d == 0 || (d < 0) == (s + t <= 0);
        }

        private bool PointInTriangle2(Point p)
        {
            var x1 = (p.X - _halfSize) * 1.0 / _innerRadius;
            var y1 = (p.Y - _halfSize) * 1.0 / _innerRadius;

            if (2 * y1 > 1) return false;
            if (sqrt3 * x1 + (-1) * y1 > 1) return false;
            if (-sqrt3 * x1 + (-1) * y1 > 1) return false;

            return true;
        }

        private bool PointInTriangle3(Point p)
        {
            var x1 = (p.X - _halfSize) * 1.0 / _innerRadius;
            var y1 = (p.Y - _halfSize) * 1.0 / _innerRadius;

            return !(2 * y1 > 1 || sqrt3 * x1 + (-1) * y1 > 1 || -sqrt3 * x1 + (-1) * y1 > 1);
        }


        private double GetAngle(Point p, int halfSize)
        {
            var angle = Math.Atan2(p.Y - halfSize, p.X - halfSize) + Math.PI / 2;
            if (angle < 0) 
                angle += 2 * Math.PI;
            return angle;
        }




        //[Benchmark]
        //public void GetAngle()
        //{
        //    Point p = new(75, 5);
        //    if (IsPointInHueCircle(p, _center, _halfSizeSquared, _innerRadiusSquared))
        //        _ = GetAngle(p, _halfSize);
        //}

        //[Benchmark]
        //public void GetAngle2()
        //{
        //    Point p = new(75, 5);
        //    if (IsPointInHueCircle2(p, _center, _halfSizeSquared, _innerRadiusSquared))
        //        _ = GetAngle(p, _halfSize);
        //}




        //[Benchmark]
        //public void Triangle1()
        //{
        //    foreach(Point p in points)
        //    {
        //        _ = PointInTriangle(p, triangle[0], triangle[1], triangle[2]);
        //    }
        //}

        [Benchmark]
        public void Triangle2()
        {
            foreach (Point p in points)
            {
                _ = PointInTriangle2(p);
            }
        }

        [Benchmark]
        public void Triangle3()
        {
            foreach (Point p in points)
            {
                _ = PointInTriangle3(p);
            }
        }
    }
}
