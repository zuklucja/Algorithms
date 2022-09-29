using System;
using System.Linq;
using System.Collections.Generic;

namespace ASD
{
    public class SheepHeard
    {
        /// <param name="dogs">Tablica opisująca pozycje psow</param>
        /// <param name="shed">Pozycja sciany budynku gospodarczego</param>
        /// <returns>Wierzchołki wielokatku w którym owce są bezpieczne</returns>
        public (double x, double y)[] SafePolygon((double x, double y)[] dogs, (float A, float B, float C) shed)
        {
            if (dogs.Length > 1)
            {
                double A = -shed.B;
                double B = shed.A;
                var l = new List<(double x, double y)>();

                foreach (var i in dogs)
                {
                    var C = -(A * i.x + B * i.y);
                    (double x, double y) p;
                    var W = shed.A * B - A * shed.B;
                    var Wx = shed.B * C - shed.C * B;
                    var Wy = shed.C * A - shed.A * C;
                    p.x = Wx / W;
                    p.y = Wy / W;
                    l.Add(p);
                }

                var f = new List<(double x, double y)>();
                f.Add(l.Min());
                f.Add(l.Max());
                f.AddRange(dogs);
                return ConvexHull(f);
            }
            return null;
        }
        private int Cross((double, double) o, (double, double) a, (double, double) b)
        {
            double value = (a.Item1 - o.Item1) * (b.Item2 - o.Item2) - (a.Item2 - o.Item2) * (b.Item1 - o.Item1);
            return Math.Abs(value) < 1e-10 ? 0 : value < 0 ? -1 : 1;
        }
        public (double, double)[] ConvexHull(List<(double x, double y)> Po)
        {
            int minindex = 0;
            (double x, double y) p0 = (double.MaxValue, double.MaxValue);

            for (int i = 0; i < Po.Count; i++)
            {
                if (Po[i].y < p0.y || (Po[i].y == p0.y && Po[i].x < p0.x))
                {
                    p0 = Po[i];
                    minindex = i;
                }
            }

            Po.RemoveAt(minindex);

            Po.Sort(((double x, double y) o1, (double x, double y) o2) =>
            {
                var ret = Cross(p0, o1, o2);
                if (ret == 0)
                {
                    return Math.Sign(Math.Pow(o2.x - p0.x, 2) + Math.Pow(o2.y - p0.y, 2) - (Math.Pow(o1.x - p0.x, 2) + Math.Pow(o1.y - p0.y, 2)));
                }
                return ret;
            });

            var P = new List<(double x, double y)>();
            P.Add(p0);
            Po.Reverse();
            P.AddRange(Po);

            var S = new Stack<(double x, double y)>();
            S.Push(P[0]);
            if (P.Count > 1)
                S.Push(P[1]);
            for (int k = 2; k < P.Count; k++)
            {
                var x = S.Pop();

                while (S.Count >= 1 && Cross(S.Peek(), x, P[k]) <= 0)
                {
                    x = S.Pop();
                }
                S.Push(x);
                S.Push(P[k]);
            }

            return S.Reverse().ToArray();
        }

        /// <param name="sheeps">Tablica opisującapozycje owiec</param>
        /// <param name="dogs">Tablica opisującapozycje psow</param>
        /// <param name="shed">Pozycja sciany budynku gospodarczego</param>
        /// <returns>Liczba bezpiecznych owiec</returns>
        public int CheckCoverage((double x, double y)[] sheeps, (double x, double y)[] dogs, (float A, float B, float C) shed)
        {
            var poly = SafePolygon(dogs, shed);
            if (poly is null) return 0;

            int n = 0;
            foreach (var i in sheeps)
            {
                int start = 1;
                int end = poly.Length - 1;

                while (end - start != 1)
                {
                    var ret = Cross(poly[0], i, poly[(end + start) / 2]);
                    if (ret >= 0)
                    {
                        end = (end + start) / 2;
                    }
                    else if (ret < 0)
                    {
                        start = (end + start) / 2;
                    }
                }

                var P1 = TriangleField(poly[0], poly[start], i);
                var P2 = TriangleField(poly[0], poly[end], i);
                var P3 = TriangleField(poly[start], poly[end], i);
                var P = TriangleField(poly[0], poly[end], poly[start]);

                if (Math.Abs(P - P1 - P2 - P3) < 1e-10)
                    n++;
            }

            return n;
        }

        private double TriangleField((double x, double y) p1, (double x, double y) p2, (double x, double y) p3)
        {
            return 0.5 * Math.Abs((p1.x * p2.y - p1.x * p3.y) + (p2.x * p3.y - p2.x * p1.y) + (p3.x * p1.y - p3.x * p2.y));
        }
    }

}