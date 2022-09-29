using System;
using ASD.Graphs;
using ASD;
using System.Collections.Generic;

namespace ASD
{

    public class graphsDFS
    {
        /// <summary>
        /// Etap 1 - szukanie trasy z miasta start_v do miasta end_v, startując w dniu day
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Indeks wierzchołka odpowiadającego miastu startowemu</param>
        /// <param name="end_v">Indeks wierzchołka odpowiadającego miastu docelowemu</param>
        /// <param name="day">Dzień startu (w tym dniu należy wyruszyć z miasta startowego)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        public (bool result, int[] route) FindRoute(DiGraph<int> g, int start_v, int end_v, int day, int days_number)
        {
            DiGraph ng = new DiGraph(g.VertexCount * days_number, g.Representation);
            int[] route2 = new int[ng.VertexCount];

            for (int i = 0; i < ng.VertexCount; i++)
                route2[i] = -1;

            route2[start_v * days_number + day] = start_v * days_number + day;

            foreach (var e in g.DFS().SearchAll())
                ng.AddEdge(e.From * days_number + e.weight, e.To * days_number + (e.weight + 1) % days_number);

            foreach (var e in ng.DFS().SearchFrom(start_v * days_number + day))
            {
                if (route2[e.To] == -1)
                    route2[e.To] = e.From;
                if (e.To / days_number == end_v)
                {
                    int x = e.To;
                    List<int> list = new List<int>();

                    while (x != start_v * days_number + day)
                    {
                        list.Add(x / days_number);
                        x = route2[x];
                    }

                    list.Add(x / days_number);
                    list.Reverse();
                    int[] route = new int[list.Count];
                    for (int i = 0; i < list.Count; i++)
                        route[i] = list[i];

                    return (true, route);
                }
            }

            return (false, null);
        }
        /// <summary>
        /// Etap 2 - szukanie trasy z jednego z miast z tablicy start_v do jednego z miast z tablicy end_v (startować można w dowolnym dniu)
        /// </summary>
        /// <param name="g">Ważony graf skierowany będący mapą</param>
        /// <param name="start_v">Tablica z indeksami wierzchołków startowych (trasę trzeba zacząć w jednym z nich)</param>
        /// <param name="end_v">Tablica z indeksami wierzchołków docelowych (trasę trzeba zakończyć w jednym z nich)</param>
        /// <param name="days_number">Liczba dni uwzględnionych w rozkładzie (tzn. wagi krawędzi są z przedziału [0, days_number-1])</param>
        /// <returns>(result, route) - result ma wartość true gdy podróż jest możliwa, wpp. false, 
        /// route to tablica z indeksami kolejno odwiedzanych miast (pierwszy indeks to indeks miasta startowego, ostatni to indeks miasta docelowego),
        /// jeżeli result == false to route ustawiamy na null</returns>
        public (bool result, int[] route) FindRouteSets(DiGraph<int> g, int[] start_v, int[] end_v, int days_number)
        {
            DiGraph ng = new DiGraph(g.VertexCount * days_number + 2, g.Representation);
            int[] route2 = new int[ng.VertexCount];
            int start = ng.VertexCount - 2, end = ng.VertexCount - 1;

            for (int i = 0; i < ng.VertexCount; i++)
                route2[i] = -1;

            route2[start] = start;

            foreach (var e in g.DFS().SearchAll())
                ng.AddEdge(e.From * days_number + e.weight, e.To * days_number + (e.weight + 1) % days_number);

            for (int i = 0; i < start_v.Length; i++)
                for (int j = 0; j < days_number; j++)
                    ng.AddEdge(start, start_v[i] * days_number + j);

            for (int i = 0; i < end_v.Length; i++)
                for (int j = 0; j < days_number; j++)
                    ng.AddEdge(end_v[i] * days_number + j, end);

            foreach (var e in ng.DFS().SearchFrom(start))
            {
                if (route2[e.To] == -1)
                    route2[e.To] = e.From;
                if (e.To == end)
                {
                    int x = route2[e.To];
                    List<int> list = new List<int>();

                    while (x != start)
                    {
                        list.Add(x / days_number);
                        x = route2[x];
                    }

                    list.Reverse();
                    int[] route = new int[list.Count];
                    for (int i = 0; i < list.Count; i++)
                        route[i] = list[i];

                    return (true, route);
                }
            }

            return (false, null);
        }
    }
}
