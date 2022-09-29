using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public class graphs
    {
        /// <summary>
        /// Etap 1: Rozmiar najliczniejszej krainy w zadanym grafie (2.5p)
        /// 
        /// Przez krainę rozumiemy maksymalny zbiór wierzchołków, z których
        /// każde dwa należą do jakiegoś cyklu (równoważnie: najliczniejszy
        /// zbiór wierzchołków G indukujący podgraf 2-spójny wierzchołkowo).
        /// 
        /// Uwaga: Z powyższej definicji wynika, że zbiór pusty jest krainą, 
        /// a zbiór jednoelementowy nie.
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <returns>Rozmiar największej bańki</returns>

        public int MaxProvinceSize(Graph G)
        {
            int n = G.VertexCount, count = 0, max = 0;
            var visited = new int[n];
            var low = new int[n];
            var parent = new int[n];
            var area = new Stack<Edge>();

            for (int i = 0; i < n; i++)
                visited[i] = low[i] = parent[i] = -1;

            for (int i = 0; i < n; i++)
            {
                if (visited[i] == -1)
                {
                    rec(i);
                }

                if (area.Count > 1)
                {
                    var Dir = new Dictionary<int, int>();

                    while (area.Count > 0)
                    {
                        var c = area.Pop();
                        if (!Dir.ContainsKey(c.From))
                            Dir.Add(c.From, c.From);
                        if (!Dir.ContainsKey(c.To))
                            Dir.Add(c.To, c.To);
                    }

                    max = Math.Max(max, Dir.Count);
                }
                else if (area.Count == 1) 
                    area.Pop();
            }

            return max;

            void rec(int curr)
            {
                visited[curr] = low[curr] = ++count;
                int children = 0;

                foreach (int v in G.OutNeighbors(curr))
                {
                    if (visited[v] == -1)
                    {
                        parent[v] = curr;
                        area.Push(new Edge(curr, v));
                        children++;

                        rec(v);

                        low[curr] = Math.Min(low[curr], low[v]);

                        if ((visited[curr] == 1 && children > 1) || (visited[curr] > 1 && low[v] >= visited[curr])) // curr to punkt artykulacji
                        {
                            var c = area.Pop();
                            if (c.From != curr || c.To != v)
                            {
                                var Dir = new Dictionary<int, int>();
                                while (c.From != curr || c.To != v)
                                {
                                    if (!Dir.ContainsKey(c.From)) 
                                        Dir.Add(c.From, c.From);
                                    if (!Dir.ContainsKey(c.To)) 
                                        Dir.Add(c.To, c.To);
                                    c = area.Pop();
                                }
                                if (!Dir.ContainsKey(c.From))
                                    Dir.Add(c.From, c.From);
                                if (!Dir.ContainsKey(c.To)) 
                                    Dir.Add(c.To, c.To);

                                max = Math.Max(max, Dir.Count);
                            }
                        }
                    }
                    else if (v != parent[curr] && visited[v] < visited[curr])
                    {
                        low[curr] = Math.Min(low[curr], visited[v]);
                        area.Push(new Edge(curr, v));
                    }
                }
            }
        }

        /// <summary>
        /// Etap 2: Wierzchołek znajdujący się w największej liczbie krain (2.5p)
        /// 
        /// Funcja zwraca wierzchołek znajdujący się w największej liczbie krain.
        /// 
        /// W przypadku remisu należy zwrócić wierzchołek o mniejszym numerze.
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        public int VertexInMostProvinces(Graph G)
        {
            int n = G.VertexCount, count = 0;
            var visited = new int[n];
            var low = new int[n];
            var parent = new int[n];
            var max = new int[n];
            var area = new Stack<Edge>();

            for (int i = 0; i < n; i++)
                visited[i] = low[i] = parent[i] = -1;

            for (int i = 0; i < n; i++)
            {
                if (visited[i] == -1)
                    rec(i);

                var Dir = new Dictionary<int, int>();
                if (area.Count > 1)
                    while (area.Count > 0)
                    {
                        var c = area.Pop();
                        if (!Dir.ContainsKey(c.From))
                        {
                            Dir.Add(c.From, c.From);
                            max[c.From]++;
                        }
                        if (!Dir.ContainsKey(c.To))
                        {
                            Dir.Add(c.To, c.To);
                            max[c.To]++;
                        }
                    }
                else if (area.Count == 1) 
                    area.Pop();
            }

            return Array.IndexOf(max, max.Max());

            void rec(int curr)
            {
                visited[curr] = low[curr] = ++count;
                int children = 0;

                foreach (int v in G.OutNeighbors(curr))
                {
                    if (visited[v] == -1)
                    {
                        parent[v] = curr;
                        area.Push(new Edge(curr, v));
                        children++;

                        rec(v);

                        low[curr] = Math.Min(low[curr], low[v]);

                        if ((visited[curr] == 1 && children > 1) || (visited[curr] > 1 && low[v] >= visited[curr])) // curr to punkt artykulacji
                        {
                            var c = area.Pop();
                            if (c.From != curr || c.To != v)
                            {
                                var Dir = new Dictionary<int, int>();

                                while (c.From != curr || c.To != v)
                                {
                                    if (!Dir.ContainsKey(c.From))
                                    {
                                        Dir.Add(c.From, c.From);
                                        max[c.From]++;
                                    }
                                    if (!Dir.ContainsKey(c.To))
                                    {
                                        Dir.Add(c.To, c.To);
                                        max[c.To]++;
                                    }
                                    c = area.Pop();
                                }

                                if (!Dir.ContainsKey(c.From))
                                {
                                    Dir.Add(c.From, c.From);
                                    max[c.From]++;
                                }
                                if (!Dir.ContainsKey(c.To))
                                {
                                    Dir.Add(c.To, c.To);
                                    max[c.To]++;
                                }
                            }
                        }
                    }
                    else if (v != parent[curr] && visited[v] < visited[curr])
                    {
                        low[curr] = Math.Min(low[curr], visited[v]);
                        area.Push(new Edge(curr, v));
                    }
                }
            }
        }

    }
}