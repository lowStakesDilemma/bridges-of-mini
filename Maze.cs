using ASD.Graphs;
using System;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Maze : MarshalByRefObject
    {

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            path = "";
            int n = maze.GetLength(0);
            int m = maze.GetLength(1);
            int s, f;
            s = f = -1;
            DiGraph<int> g = new DiGraph<int>(maze.Length);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (!withDynamite && maze[i, j] == 'X')
                    {
                        continue;
                    }
                    if (maze[i, j] == 'S')
                    {
                        s = IdxToGraph(n, m, i, j);
                    }
                    if (maze[i, j] == 'E')
                    {
                        f = IdxToGraph(n, m, i, j);
                    }
                    if (i + 1 < n)
                    {
                        if (maze[i + 1, j] != 'X')
                        {
                            g.AddEdge(IdxToGraph(n, m, i, j), IdxToGraph(n, m, i + 1, j), 1);
                            if (maze[i, j] == 'X')
                            {
                                g.AddEdge(IdxToGraph(n, m, i+1, j), IdxToGraph(n, m, i, j), t);
                            }
                            else
                            {
                                g.AddEdge(IdxToGraph(n, m, i + 1, j), IdxToGraph(n, m, i, j), 1);
                            }
                        }
                        else if (withDynamite)
                        {
                            g.AddEdge(IdxToGraph(n, m, i, j), IdxToGraph(n, m, i + 1, j), t);
                            if (maze[i, j] == 'X')
                            {
                                g.AddEdge(IdxToGraph(n, m, i + 1, j), IdxToGraph(n, m, i, j), t);
                            }
                            else
                            {
                                g.AddEdge(IdxToGraph(n, m, i + 1, j), IdxToGraph(n, m, i, j), 1);
                            }
                        }
                    }
                    if (j + 1 < m)
                    {
                        if (maze[i, j + 1] != 'X')
                        {
                            g.AddEdge(IdxToGraph(n, m, i, j), IdxToGraph(n, m, i, j + 1), 1);
                            if (maze[i, j] == 'X')
                            {
                                g.AddEdge(IdxToGraph(n, m, i, j+1), IdxToGraph(n, m, i, j), t);
                            }
                            else
                            {
                                g.AddEdge(IdxToGraph(n, m, i, j+1), IdxToGraph(n, m, i, j), 1);
                            }
                        }
                        else if (withDynamite)
                        {
                            g.AddEdge(IdxToGraph(n, m, i, j), IdxToGraph(n, m, i, j + 1), t);
                            if (maze[i, j] == 'X')
                            {
                                g.AddEdge(IdxToGraph(n, m, i, j + 1), IdxToGraph(n, m, i, j), t);
                            }
                            else
                            {
                                g.AddEdge(IdxToGraph(n, m, i, j + 1), IdxToGraph(n, m, i, j), 1);
                            }
                        }
                    }
                }
            }
            if(s == -1 || f == -1)
            {
                throw new Exception("Program did not find the start or end in the maze.");
            }
            var pinfo = Paths.Dijkstra<int>(g, s);
            if (pinfo.Reachable(s, f))
            {
                path = "";
                var gpath = pinfo.GetPath(s, f);
                var (yp, xp) = GraphToIdx(n, m, s);
                for(int i = 1; i<gpath.Length; i++)
                {
                    var (y, x) = GraphToIdx(n, m, gpath[i]);
                    if(x > xp)
                    {
                        path += "E";
                    }
                    else if (x < xp)
                    {
                        path += "W";
                    }
                    else if(y < yp)
                    {
                        path += "N";
                    }
                    else
                    {
                        path += "S";
                    }
                    (xp, yp) = (x, y);  
                }
                return pinfo.GetDistance(s, f);
            }
            return -1;
        }

        public static int IdxToGraph(int n, int m, int i, int j)
        {
            return i * m + j;
        }

        public static (int, int) GraphToIdx(int n, int m, int idx)
        {
            int i = idx / m;
            int j = idx % m;
            return (i, j);
        }

        public static int IdxToGraphDynamite(int n, int m, int k, int i, int j, int l)
        {
            return l * n * m + i * m + j;
        }

        public static (int, int, int) GraphToIdxDynamite(int n, int m, int k, int idx)
        {
            int j = idx % m;
            idx -= j;
            idx /= m;
            int i = idx % n;
            int l = idx / n;
            return (i, j, l);
        }

        /// <summary>
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            path = "";
            int n = maze.GetLength(0);
            int m = maze.GetLength(1);
            int s, f;
            s = f = -1;
            DiGraph<int> g = new DiGraph<int>(maze.Length * (k+1));
            for (int l = 0; l < k + 1; l++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (maze[i, j] == 'S')
                        {
                            s = IdxToGraph(n, m, i, j);
                        }
                        if (maze[i, j] == 'E')
                        {
                            f = IdxToGraph(n, m, i, j);
                        }
                        if (i + 1 < n)
                        {
                            if (maze[i + 1, j] != 'X')
                            {
                                g.AddEdge(IdxToGraphDynamite(n, m, k, i, j, l), IdxToGraphDynamite(n, m, k, i + 1, j, l), 1);
                                if (maze[i, j] != 'X')
                                    g.AddEdge(IdxToGraphDynamite(n, m, k, i, j, l), IdxToGraphDynamite(n, m, k, i + 1, j, l), 1);
                            }
                            else if(l + 1 < k + 1)
                            {
                                g.AddEdge(IdxToGraphDynamite(n, m, k, i, j, l), IdxToGraphDynamite(n, m, k, i + 1, j, l + 1), t);
                            }
                        }
                        if (j + 1 < m)
                        {
                            if (maze[i, j + 1] != 'X')
                            {
                                g.AddEdge(IdxToGraphDynamite(n, m, k, i, j, l), IdxToGraphDynamite(n, m, k, i, j + 1, l), 1);
                                if (maze[i, j] == 'X')
                                {
                                    g.AddEdge(IdxToGraphDynamite(n, m, k, i, j + 1, l), IdxToGraphDynamite(n, m, k, i, j, l), 1);
                                }
                                else if(l + 1 < k + 1)
                                {
                                    g.AddEdge(IdxToGraphDynamite(n, m, k, i, j, l), IdxToGraphDynamite(n, m, k, i, j + 1, l + 1), t);
                                }
                            }
                        }
                    }
                }
            }
            if (s == -1 || f == -1)
            {
                throw new Exception("Program did not find the start or end in the maze.");
            }
            var pinfo = Paths.Dijkstra<int>(g, s);
            int minDist = int.MaxValue;
            int minL = -1;
            for(int l = 0; l < k+1; l++)
            {
                if(pinfo.Reachable(s, f + l * n * m))
                {
                    int currDist = pinfo.GetDistance(s, f + l * n * m);
                    if(currDist < minDist)
                    {
                        minDist = currDist;
                        minL = l;
                    }
                }
            }
            if(minL == -1)
            {
                return -1;
            }
            path = "";
            var gpath = pinfo.GetPath(s, f + minL * n * m);
            var (yp, xp, lp) = GraphToIdxDynamite(n, m, k, s);
            for (int i = 1; i < gpath.Length; i++)
            {
                var (y, x, l) = GraphToIdxDynamite(n, m, k, gpath[i]);
                if (x > xp)
                {
                    path += "E";
                }
                else if (x < xp)
                {
                    path += "W";
                }
                else if (y < yp)
                {
                    path += "N";
                }
                else
                {
                    path += "S";
                }
                (xp, yp) = (x, y);
            }
            return pinfo.GetDistance(s, f + minL * n * m);
        }
    }
}