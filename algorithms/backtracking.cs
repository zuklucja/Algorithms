using System;
using System.Linq;
using System.Collections.Generic;
using ASD.Graphs;

namespace Lab10
{
    public class DeliveryPlanner
    {

        /// <param name="railway">Graf reprezentujący sieć kolejową</param>
        /// <param name="eggDemand">Zapotrzebowanie na jajka na poszczególnyhc stacjach. Zerowy element tej tablicy zawsze jest 0</param>
        /// <param name="truckCapacity">Pojemność wagonu na jajka</param>
        /// <param name="tankEngineRange">Zasięg parowozu</param>
        /// <param name="isRefuelStation">na danym indeksie true, jeśli na danej stacji można uzupelnić węgiel i wodę</param>
        /// <param name="anySolution">Czy znaleźć jakiekolwiek rozwiązanie (true, etap 1), czy najkrótsze (false, etap 2)</param>
        /// <returns>Informację czy istnieje trasa oraz tablicę reprezentującą kolejne wierzchołki w trasie (pierwszy i ostatni element tablicy musi być 0). W przypadku, gdy zwracany jest false, wartość tego pola nie jest sprawdzana, może być null.</returns>
        public (bool routeExists, int[] route) PlanDelivery(Graph<int> railway, int[] eggDemand, int truckCapacity, int tankEngineRange, bool[] isRefuelStation, bool anySolution)
        {
            int n = railway.VertexCount;
            int count = 1;
            int currcapacity = 0;
            int currrange = 0;
            Stack<int> path = new Stack<int>();
            path.Push(0);
            int[] bestpath = null;
            int time = 0;
            int besttime = int.MaxValue;
            bool[] visited = new bool[n];

            DeliveryRec(0);

            if (bestpath != null)
                return (true, bestpath);

            return (false, null);

            int DeliveryRec(int k)
            {
                if (isRefuelStation[k])
                    currrange = 0;

                if (k == 0)
                    currcapacity = 0;

                if (count == n && railway.HasEdge(k, 0))
                {
                    int weight = railway.GetEdgeWeight(k, 0);
                    if (currrange + weight <= tankEngineRange)
                    {
                        path.Push(0);
                        if (anySolution)
                        {
                            bestpath = path.ToArray();
                            return 1;
                        }
                        if (time + weight < besttime)
                        {
                            bestpath = path.ToArray();
                            besttime = time + weight;
                        }
                        path.Pop();
                    }
                    else return -1;
                }

                foreach(var i in railway.OutNeighbors(k))
                {
                    if (!visited[i])
                    {
                        int weight = railway.GetEdgeWeight(k, i);

                        if (currcapacity + eggDemand[i] <= truckCapacity && currrange + weight <= tankEngineRange)
                        {
                            if (!anySolution && time + weight > besttime)
                                continue;
                            if (i != 0)
                            {
                                count++;
                                visited[i] = true;
                            }
                            int oldrange = currrange, oldcap = currcapacity;
                            currcapacity = currcapacity + eggDemand[i];
                            currrange = currrange + weight;
                            time += weight;
                            path.Push(i);

                            if (DeliveryRec(i) == 1) 
                                return 1;

                            path.Pop();
                            time -= weight;
                            currcapacity = oldcap;
                            currrange = oldrange;
                            if (i != 0)
                            {
                                count--;
                                visited[i] = false;
                            }
                        }
                    }
                }

                return 0;
            }
        }
    }
}
