using ASD.Graphs;
using ASD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class Lab06 : MarshalByRefObject
    {
        /// <summary>Etap I</summary>
        /// <param name="G">Graf opisujący połączenia szlakami turystycznymi z podanym czasem przejścia krawędzi w wadze.</param>
        /// <param name="waitTime">Czas oczekiwania Studenta-Podróżnika w danym wierzchołku.</param>
        /// <param name="s">Wierzchołek startowy (początek trasy).</param>
        /// <returns>Pierwszy element krotki to wierzchołek końcowy szukanej trasy. Drugi element to długość trasy w minutach. Trzeci element to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (zawierająca zarówno wierzchołek początkowy, jak i końcowy).</returns>
        public (int t, int l, int[] path) Stage1(DiGraph<int> G, int[] waitTime, int s)
        {


            int[] dist = new int[G.VertexCount];
            int[] prev = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++)
            {
                dist[i] = int.MaxValue;
                prev[i] = -1;
            }

            dist[s] = 0;
            prev[s] = s;

            Queue<(int vertex, int time)> q = new Queue<(int vertex, int time)>();
            q.Enqueue((s, 0));
            int maxCost = -1;
            int maxIdx = -1;
            while (q.Count > 0)
            {
                var (vertex, time) = q.Dequeue();

                foreach(var v in G.OutNeighbors(vertex))
                {
                    int cost = dist[vertex] + G.GetEdgeWeight(vertex, v) + waitTime[v];
                    if(cost < dist[v])
                    {
                        dist[v] = cost;
                        prev[v] = vertex;
                        q.Enqueue((v, cost));
                    }
                }
            }

            for (int i = 0; i < G.VertexCount; i++)
            {
                if (dist[i] != int.MaxValue && dist[i] - waitTime[i] > maxCost)
                {
                    maxCost = dist[i] - waitTime[i];
                    maxIdx = i;
                }
            }
            if(maxIdx == -1)
            {
                maxIdx = s;
                maxCost = 0;
            }

            List<int> path = new List<int>();
            int currentVertex = maxIdx;

            while (prev[currentVertex] != currentVertex)
            {
                path.Add(currentVertex);
                currentVertex = prev[currentVertex];
            }
            path.Add(currentVertex);
            path.Reverse();
            return(maxIdx, maxCost, path.ToArray());
        }

        /// <summary>Etap II</summary>
        /// <param name="G">Graf opisujący połączenia szlakami turystycznymi z podanym czasem przejścia krawędzi w wadze.</param>
        /// <param name="C">Graf opisujący koszty przejścia krawędziami w grafie G.</param>
        /// <param name="waitTime">Czas oczekiwania Studenta-Podróżnika w danym wierzchołku.</param>
        /// <param name="s">Wierzchołek startowy (początek trasy).</param>
        /// <param name="t">Wierzchołek końcowy (koniec trasy).</param>
        /// <returns>Pierwszy element krotki to długość trasy w minutach. Drugi element to koszt przebycia trasy w złotych. Trzeci element to droga będąca rozwiązaniem: sekwencja odwiedzanych wierzchołków (zawierająca zarówno wierzchołek początkowy, jak i końcowy). Jeśli szukana trasa nie istnieje, funkcja zwraca `null`.</returns>
        public (int l, int c, int[] path)? Stage2(DiGraph<int> G, Graph<int> C, int[] waitTime, int s, int t)
        {
            DiGraph<int> DiC = new DiGraph<int>(G.VertexCount);
            foreach(var edge in C.DFS().SearchAll())
            {
                if(G.HasEdge(edge.From, edge.To))
                {
                    DiC.AddEdge(edge.From, edge.To, edge.Weight);
                }
            }


            var pinfo = Paths.Dijkstra(DiC, s);
            if(!pinfo.Reachable(s, t))
            {
                return null;
            }
            int minCost = pinfo.GetDistance(s, t);

            int[] dist = new int[G.VertexCount];
            int[] prev = new int[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++)
            {
                dist[i] = int.MaxValue;
                prev[i] = -1;
            }

            dist[s] = 0;
            prev[s] = s;

            Queue<(int vertex, int time, int money)> q = new Queue<(int vertex, int time, int money)>();
            q.Enqueue((s, 0, 0));
            while (q.Count > 0)
            {
                var (vertex, time, money) = q.Dequeue();

                if(money > minCost || pinfo.GetDistance(s, vertex) < money)
                {
                    continue;
                }

                foreach (var v in G.OutNeighbors(vertex))
                {
                    int cost = time + G.GetEdgeWeight(vertex, v) + waitTime[v];
                    int moneyCost = money + DiC.GetEdgeWeight(vertex, v);
                    if (moneyCost <= pinfo.GetDistance(s, v) && cost < dist[v])
                    {
                        dist[v] = cost;
                        prev[v] = vertex;
                        q.Enqueue((v, cost, moneyCost));
                    }
                }
            }

            if (dist[t] == int.MaxValue)
            {
                return null;
            }

            List<int> path = new List<int>();
            int currentVertex = t;

            while (prev[currentVertex] != currentVertex)
            {
                path.Add(currentVertex);
                currentVertex = prev[currentVertex];
            }
            path.Add(currentVertex);
            path.Reverse();
            return ((s==t) ? 0 : dist[t] - waitTime[t], minCost, path.ToArray());
        }
    }
}