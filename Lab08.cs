using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace ASD
{
    public class Lab08 : MarshalByRefObject
    {
        public static int CoordsToNodes(int y, int x, int h, int w)
        {
            return y * w + x;
        }

        public static (int, int) NodesToCoords(int idx, int h, int w)
        {
            return (idx / w, idx % w);
        }

        /// <summary>Etap I</summary>
        /// <param name="P">Tablica która dla każdego pola zawiera informacje, ile maszyn moze lacznie wyjechac z tego pola</param>
        /// <param name="MachinePos">Tablica zawierajaca informacje o poczatkowym polozeniu maszyn</param>
        /// <returns>Pierwszy element kroki to liczba uratowanych maszyn, drugi to tablica indeksów tych maszyn</returns>
        public (int savedNum, int[] Saved) Stage1(int[,] P, (int row, int col)[] MachinePos)
        {
			int h = P.GetLength(0);
            int w = P.GetLength(1);

            int n = h * w;

            DiGraph<int> g = new DiGraph<int>(2 * n + 2);
            int s = 2 * n;
            int t = 2 * n + 1;

            for(int y = 0; y < h; y++)
            {
                for(int x = 0; x < w; x++)
                {
                    g.AddEdge(CoordsToNodes(y, x, h, w), CoordsToNodes(y, x, h, w) + n, P[y, x]);
                    // TOP EDGE CASE (y==0)
                    if(y == 0)
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, t, int.MaxValue);
                    }
                    else
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, CoordsToNodes(y - 1, x, h, w), int.MaxValue);
                    }

                    // LEFT EDGE CASE (x==0)
                    if(x != 0)
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, CoordsToNodes(y, x - 1, h, w), int.MaxValue);
                    }

                    // BOTTOM EDGE CASE (y==h-1)
                    if(y != h - 1)
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, CoordsToNodes(y + 1, x, h, w), int.MaxValue);
                    }

                    // RIGHT EDGE CASE (x==w-1)
                    if(x != w - 1)
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, CoordsToNodes(y, x + 1, h, w), int.MaxValue);
                    }
                }
            }

            for(int i = 0; i < MachinePos.Length; i++)
            {
                (int y, int x) = MachinePos[i];
                g.AddEdge(s, CoordsToNodes(y, x, h, w), 1);
            }

            var (flow, flowGraph) = Flows.FordFulkerson(g, s, t);

            int savedNum = 0;
            List<int> p = new List<int>();
            for(int i = 0; i<MachinePos.Length; i++)
            {
                var (y, x) = MachinePos[i];
                if(flowGraph.HasEdge(s, CoordsToNodes(y, x, h, w)) && flowGraph.GetEdgeWeight(s, CoordsToNodes(y, x, h, w) ) > 0)
                {
                    savedNum++;
                    p.Add(i);   
                }
            }

            return (savedNum, p.ToArray());
        }

        /// <summary>Etap II</summary>
        /// <param name="P">Tablica która dla każdego pola zawiera informacje, ile maszyn moze lacznie wyjechac z tego pola</param>
        /// <param name="MachinePos">Tablica zawierajaca informacje o poczatkowym polozeniu maszyn</param>
        /// <param name="MachineValue">Tablica zawierajaca informacje o wartosci maszyn</param>
        /// <param name="moveCost">Koszt jednego ruchu</param>
        /// <returns>Pierwszy element kroki to najwiekszy mozliwy zysk, drugi to tablica indeksow maszyn, ktorych wyprowadzenie maksymalizuje zysk</returns>
        public (int bestProfit, int[] Saved) Stage2(int[,] P, (int row, int col)[] MachinePos, int[] MachineValue, int moveCost)
        {
            int h = P.GetLength(0);
            int w = P.GetLength(1);

            int n = h * w;

            NetworkWithCosts<int, int> g = new NetworkWithCosts<int, int>(2 * n + 2);
            int s = 2 * n;
            int t = 2 * n + 1;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    g.AddEdge(CoordsToNodes(y, x, h, w), CoordsToNodes(y, x, h, w) + n, (P[y, x], moveCost));
                    // TOP EDGE CASE (y==0)
                    if (y == 0)
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, t, (int.MaxValue, 0));
                    }
                    else
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, CoordsToNodes(y - 1, x, h, w), (int.MaxValue, 0));
                    }

                    // LEFT EDGE CASE (x==0)
                    if (x != 0)
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, CoordsToNodes(y, x - 1, h, w), (int.MaxValue, 0));
                    }

                    // BOTTOM EDGE CASE (y==h-1)
                    if (y != h - 1)
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, CoordsToNodes(y + 1, x, h, w), (int.MaxValue, 0));
                    }

                    // RIGHT EDGE CASE (x==w-1)
                    if (x != w - 1)
                    {
                        g.AddEdge(CoordsToNodes(y, x, h, w) + n, CoordsToNodes(y, x + 1, h, w), (int.MaxValue, 0));
                    }
                }
            }

            for (int i = 0; i < MachinePos.Length; i++)
            {
                (int y, int x) = MachinePos[i];
                if((y+1)*moveCost < MachineValue[i])
                    g.AddEdge(s, CoordsToNodes(y, x, h, w), (1, -MachineValue[i]));
            }

            var (flowValue, flowCost, flowGraph) = Flows.MinCostMaxFlow(g, s, t);

            List<int> p = new List<int>();
            for (int i = 0; i < MachinePos.Length; i++)
            {
                var (y, x) = MachinePos[i];
                if (flowGraph.HasEdge(s, CoordsToNodes(y, x, h, w)) && flowGraph.GetEdgeWeight(s, CoordsToNodes(y, x, h, w)) > 0)
                {
                    p.Add(i);
                }
            }
            if(-flowCost < 0)
            {
                return (0, new int[0]);
            }

            return (-flowCost, p.ToArray());
        }
    }
}