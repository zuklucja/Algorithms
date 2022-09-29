using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class ShortestPaths
    {
        /// <summary>
        /// Etap 1 i 2 - szukanie trasy w nieplynacej rzece
        /// </summary>
        /// <param name="w"> Odległość między brzegami rzeki</param>
        /// <param name="l"> Długość fragmentu rzeki </param>
        /// <param name="lilie"> Opis lilii na rzece </param>
        /// <param name="sila"> Siła żabki - maksymalny kwadrat długości jednego skoku </param>
        /// <param name="start"> Początkowa pozycja w metrach od lewej strony </param>
        /// <returns> (int total, (int x, int y)[] route) - total - suma sił koniecznych do wszystkich skoków, route -
        /// lista par opisujących skoki. Opis jednego skoku (x,y) to dystans w osi x i dystans w osi y, jaki skok pokonuje</returns>
        public (int total, (int, int)[] route) FindRoute(int w, int l, int[,] lilie, int sila, int start)
        {
            DiGraph<int> G = new DiGraph<int>(w * l + 2);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < l; j++)
                {
                    int u = i * l + j;
                    if (lilie[i, j] == 1)
                    {
                        if (sila >= Math.Pow(i + 1, 2) + Math.Pow(j - start, 2))
                        {
                            G.AddEdge(w * l, u, (int)(Math.Pow(i + 1, 2) + Math.Pow(j - start, 2)));
                        }
                        if (sila >= Math.Pow(w - i, 2))
                        {
                            G.AddEdge(u, w * l + 1, (int)(Math.Pow(w - i, 2)));
                        }

                        for (int m = 0; m < w; m++)
                        {
                            for (int n = 0; n < l; n++)
                            {
                                int f = (int)(Math.Pow(m - i, 2) + Math.Pow(n - j, 2));
                                if (i == m && j == n) 
                                    continue;
                                if (lilie[m, n] == 1 && sila >= f)
                                {
                                    int v = m * l + n;
                                    G.AddEdge(u, v, f);
                                }
                            }
                        }
                    }
                }
                if (sila >= Math.Pow(w + 1, 2))
                    G.AddEdge(w * l, w * l + 1, (int)Math.Pow(w + 1, 2));
            }

            var p = Paths.Dijkstra(G, w * l);
            if (p.Reachable(w * l, w * l + 1))
            {
                int total = p.GetDistance(w * l, w * l + 1);
                var routet = p.GetPath(w * l, w * l + 1);
                (int, int)[] route = new (int, int)[routet.Length - 1];
                (int, int) prev = (0, 0);

                for (int i = 0; i < routet.Length; i++)
                {
                    if (i == 0)
                    {
                        prev = (-1, start);
                        continue;
                    }
                    else if (i == routet.Length - 1)
                    {
                        route[i - 1] = (w - prev.Item1, 0);
                        continue;
                    }

                    (int, int) q = ((routet[i]) / l, routet[i] % l);
                    route[i - 1] = (q.Item1 - prev.Item1, q.Item2 - prev.Item2);
                    prev = q;
                }

                return (total, route);
            }

            return (0, null);
        }

        public int mod(int i, int m)
        {
            return (i % m + m) % m;
        }
        /// <summary>
        /// Etap 3 i 4 - szukanie trasy w nieplynacej rzece
        /// </summary>
        /// <param name="w"> Odległość między brzegami rzeki</param>
        /// <param name="l"> Długość fragmentu rzeki </param>
        /// <param name="lilie"> Opis lilii na rzece </param>
        /// <param name="sila"> Siła żabki - maksymalny kwadrat długości jednego skoku </param>
        /// <param name="start"> Początkowa pozycja w metrach od lewej strony </param>
        /// <param name="max_skok"> Maksymalna ilość skoków </param>
        /// <param name="v"> Prędkość rzeki </param>
        /// <returns> (int total, (int x, int y)[] route) - total - suma sił koniecznych do wszystkich skoków, route -
        /// lista par opisujących skoki. Opis jednego skoku (x,y) to dystans w osi x i dystans w osi y, jaki skok pokonuje</returns>
        public (int total, (int, int)[] route) FindRouteFlowing(int w, int l, int[,] lilie, int sila, int start, int max_skok, int v)
        {
            int size = w * l + 1;
            DiGraph<int> G = new DiGraph<int>(size * (max_skok - 1) + 2);
            int START = size * (max_skok - 1), END = START + 1;

            if (sila >= Math.Pow(w + 1, 2))
                G.AddEdge(START, END, (int)Math.Pow(w + 1, 2));

            for (int mm = 0; mm < max_skok - 1; mm++)
            {
                int postSTART = size * (mm + 1) - 1; // wierzchołek START na poziomie mm
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < l; j++)
                    {
                        if (lilie[i, mod(j + (l - v) * (mm + 1), l)] == 1)
                        {
                            int u = i * l + j + mm * size;
                            if (sila >= Math.Pow(i + 1, 2) + Math.Pow(j - start, 2))
                            {
                                G.AddEdge(START, u, (int)(Math.Pow(i + 1, 2) + Math.Pow(j - start, 2)));
                            }
                        }
                        if (mm != 0 && lilie[i, mod(j + (l - v) * mm, l)] == 1)
                        {
                            int u = i * l + j + (mm - 1) * size;
                            for (int m = 0; m < w; m++)
                            {
                                for (int n = 0; n < l; n++)
                                {
                                    int vv = m * l + n + mm * size;
                                    if (i == m && j == n)
                                    {
                                        if (lilie[m, mod(n + (l - v) * (mm + 1), l)] == 1)
                                            G.AddEdge(u, vv, 0);
                                        continue;
                                    }

                                    int f = (int)(Math.Pow(m - i, 2) + Math.Pow(n - j, 2));
                                    if (lilie[m, mod(n + (l - v) * (mm + 1), l)] == 1 && sila >= f)
                                    {
                                        G.AddEdge(u, vv, f);
                                    }
                                }
                            }
                        }
                        if (lilie[i, mod(j + (l - v) * (mm + 1), l)] == 1 && sila >= Math.Pow(w - i, 2))
                        {
                            int u = i * l + j + mm * size;
                            G.AddEdge(u, END, (int)(Math.Pow(w - i, 2)));
                        }
                    }
                }
                G.AddEdge(START, postSTART, 0);
                START = postSTART;
            }

            START = size * (max_skok - 1);
            var p = Paths.Dijkstra(G, START);
            if (p.Reachable(START, END))
            {
                int total = p.GetDistance(START, END);
                var routet = p.GetPath(START, END);
                (int, int)[] route = new (int, int)[routet.Length - 1];
                (int, int) prev = (0, 0);

                for (int i = 0; i < routet.Length; i++)
                {
                    if (i == 0)
                    {
                        prev = (-1, start);
                        continue;
                    }
                    else if (i == routet.Length - 1)
                    {
                        route[i - 1] = (w - prev.Item1, 0);
                        continue;
                    }

                    int sizes = routet[i] - routet[i] / size * size, jj = sizes % l, ii = (sizes - jj) / l;
                    (int, int) q;

                    if (routet[i] + 1 % size == 0)
                        q = (-1, start);
                    else 
                        q = (ii, jj);

                    route[i - 1] = (q.Item1 - prev.Item1, q.Item2 - prev.Item2);
                    prev = q;
                }

                return (total, route);
            }

            return (0, null);
        }
    }
}
