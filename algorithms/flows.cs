using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD_lab08
{
    [Serializable]
    public struct Cat
    {
        /// <summary>
        /// Zawiera identyfikatory osób, które kot zaakceptuje
        /// </summary>
        public int[] AcceptablePeople { get; }

        public Cat(int[] acceptablePeople)
        {
            AcceptablePeople = acceptablePeople;
        }
    }

    [Serializable]
    public struct Person
    {
        /// <summary>
        /// Maksymalna liczba kotów, którymi zajmie się opiekun
        /// </summary>
        public int MaxCats { get; }

        /// <summary>
        /// Kwoty, które osoba życzy sobie za opiekę nad kotami (catId -> int)
        /// </summary>
        public int[] Salaries { get; }

        public Person(int maxCats, int[] salaries)
        {
            MaxCats = maxCats;
            Salaries = salaries;
        }
    }

    public class Cats
    {
        /// <summary>
        /// Zadanie pierwsze, w którym nie bierzemy pod uwagę pieniędzy jakie nam przyjdzie zapłacić opiekunom
        /// </summary>
        /// <param name="cats">Tablica zawierające nasze koty</param>
        /// <param name="people">Tablica zawierająca dostępnych opiekunów</param>
        /// <returns>
        /// isPossible: wartość logiczna oznaczająca, czy przypisanie jest możliwe, 
        /// assignment: przypisanie kotów do opiekunów (personId -> [catId])
        /// </returns>
        public (bool isPossible, int[][] assignment) StageOne(Cat[] cats, Person[] people)
        {
            int nc = cats.Length, np = people.Length;
            DiGraph<int> G = new DiGraph<int>(2 + nc + np);
            int start = nc + np, end = start + 1;

            for (int j = 0; j < np; j++)
            {
                for (int i = 0; i < nc; i++)
                {
                    if (j==0) 
                        G.AddEdge(start, i, 1);
                    if (cats[i].AcceptablePeople.Contains(j))
                        G.AddEdge(i, j + nc, 1);
                }

                G.AddEdge(j + nc, end, people[j].MaxCats);
            }

            var (flowVal, flow) = Flows.FordFulkerson(G, start, end);
            if (flowVal != nc)
                return (false, null);

            int[][] assignment = new int[np][];
            for (int i = 0; i < np; i++)
            {
                if (flow.HasEdge(nc + i, end))
                {
                    assignment[i] = new int[flow.GetEdgeWeight(nc + i, end)];
                    int k = 0;
                    for (int j = 0; j < nc; j++)
                        if (flow.HasEdge(j, nc + i))
                            assignment[i][k++] = j;
                }
                else
                    assignment[i] = new int[0];
            }

            return (true, assignment);
        }

        /// <summary>
        /// Zadanie drugie, w którym bierzemy pod uwagę kwoty jakie nam przyjdzie zapłacić
        /// </summary>
        /// <param name="cats">Tablica zawierające nasze koty</param>
        /// <param name="people">Tablica zawierająca dostępnych opiekunów</param>
        /// <returns>
        /// isPossible: wartość logiczna oznaczająca, czy przypisanie jest możliwe,
        /// assignment: przypisanie kotów do opiekunów (personId -> [catId]),
        /// minCost: minimalna suma pieniędzy do zapłacenia opiekunom za opiekę nad wszystkimi kotami
        /// </returns>
        public (bool isPossible, int[][] assignment, int minCost) StageTwo(Cat[] cats, Person[] people)
        {
            int nc = cats.Length, np = people.Length;
            NetworkWithCosts<int, int> G = new NetworkWithCosts<int, int>(2 + nc + np);
            int start = nc + np, end = start + 1;

            for (int i=0;i<nc;i++)
            {
                G.AddEdge(i, end, 1, 0);

                foreach(var j in cats[i].AcceptablePeople)
                {
                    G.AddEdge(start, j + nc, people[j].MaxCats, 0);
                    G.AddEdge(j + nc, i, 1, people[j].Salaries[i]);
                }
            }

            var (flowVal, flowCost, flow) = Flows.MinCostMaxFlow(G, start, end);
            if (flowVal != nc)
                return (false, null, int.MaxValue);

            int[][] assignment = new int[np][];
            for (int i = 0; i < np; i++)
            {
                if (flow.HasEdge(start, nc+i))
                {
                    int k = 0;
                    assignment[i] = new int[flow.GetEdgeWeight(start, nc+i)];
                    for (int j = 0; j < nc; j++)
                        if (flow.HasEdge(nc + i,j))
                            assignment[i][k++] = j;
                }
                else
                    assignment[i] = new int[0];

            }

            return (true, assignment, flowCost);
        }
    }
}
