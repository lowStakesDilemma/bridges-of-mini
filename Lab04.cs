using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ASD
{
    public class Lab04 : MarshalByRefObject
    {
        /// <summary>
        /// Etap 1 - Wyznaczenie liczby oraz listy zainfekowanych serwisów po upływie K dni.
        /// Algorytm analizuje propagację infekcji w grafie i zwraca wszystkie dotknięte nią serwisy.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Indeks początkowo zainfekowanego serwisu.</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage1(Graph G, int K, int s)
        {
            int n = G.VertexCount;
            int m = G.EdgeCount;

            Dictionary<int, int> infectedDates = [];
            Queue<int> q = new Queue<int> ();
            List<int> final = [];
            q.Enqueue(s);
            infectedDates[s] = 1;

            while (q.Count > 0)
            {
                int vertex = q.Dequeue();
                int time = infectedDates[vertex];
                if (!final.Contains(vertex))
                {
                    final.Add(vertex);
                }
                if (time == K)
                {
                    continue;
                }
                

                foreach(int neighbor in G.OutNeighbors(vertex))
                {
                    if (infectedDates.ContainsKey(neighbor))
                    {
                        if (infectedDates[neighbor] > time + 1)
                        {
                            infectedDates[neighbor] = time+1;
                            q.Enqueue(neighbor);
                        }
                    }
                    else
                    {
                        infectedDates[neighbor] = time+1;
                        q.Enqueue(neighbor);
                    }
                }
            }
            final.Sort();

            return (final.Count, final.ToArray());
        }

        /// <summary>
        /// Etap 2 - Wyznaczenie liczby oraz listy zainfekowanych serwisów przy uwzględnieniu wyłączeń.
        /// Algorytm analizuje propagację infekcji z możliwością wcześniejszego wyłączania serwisów.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Tablica początkowo zainfekowanych serwisów.</param>
        /// <param name="serviceTurnoffDay">Tablica zawierająca dzień, w którym dany serwis został wyłączony (K + 1 oznacza brak wyłączenia).</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage2(Graph G, int K, int[] s, int[] serviceTurnoffDay)
        {
            int n = G.VertexCount;
            int m = G.EdgeCount;
            int p = s.Length;

            int[] memory = new int[n];
            Array.Fill(memory, int.MaxValue);
            Queue<(int, int)> q = [];
            List<int> final = [];

            foreach(var sV in s)
            {
                q.Enqueue((sV, 1));
                memory[sV] = 1;
                final.Add(sV);
            }

            while (q.Count > 0)
            {
                var (vertex, time) = q.Dequeue();
                if (!final.Contains(vertex) && time < serviceTurnoffDay[vertex])
                {
                    final.Add(vertex);
                }
                if (time == K || time + 1 >= serviceTurnoffDay[vertex])
                {
                    continue;
                }


                foreach (int neighbor in G.OutNeighbors(vertex))
                {
                    if (memory[neighbor] > time + 1)
                    {
                        memory[neighbor] = time + 1;
                        q.Enqueue((neighbor, time + 1));
                    }
                }
            }
            final.Sort();

            return (final.Count, final.ToArray());
        }

        /// <summary>
        /// Etap 3 - Wyznaczenie liczby oraz listy zainfekowanych serwisów z możliwością ponownego włączenia wyłączonych serwisów.
        /// Algorytm analizuje propagację infekcji uwzględniając serwisy, które mogą być ponownie uruchamiane po określonym czasie.
        /// </summary>
        /// <param name="G">Graf reprezentujący infrastrukturę serwisów.</param>
        /// <param name="K">Liczba dni propagacji infekcji.</param>
        /// <param name="s">Tablica początkowo zainfekowanych serwisów.</param>
        /// <param name="serviceTurnoffDay">Tablica zawierająca dzień, w którym dany serwis został wyłączony (K + 1 oznacza brak wyłączenia).</param>
        /// <param name="serviceTurnonDay">Tablica zawierająca dzień, w którym dany serwis został ponownie włączony.</param>
        /// <returns>
        /// (int numberOfInfectedServices, int[] listOfInfectedServices) - 
        /// numberOfInfectedServices: liczba zainfekowanych serwisów,
        /// listOfInfectedServices: tablica zawierająca numery zainfekowanych serwisów w kolejności rosnącej.
        /// </returns>
        /// Jesli ServiceTurnoffDay[i] == ServiceTurnonDay[i], to serwis nie moze zostac zainfekowany w dniu i, a jesli juz byl zainfekowany to w dniu i nie infekuje
        /// Zainfekowany serwis infekuje kazdego dnia (o ile nie jest wylaczony)
        public (int numberOfInfectedServices, int[] listOfInfectedServices) Stage3(Graph G, int K, int[] s, int[] serviceTurnoffDay, int[] serviceTurnonDay)
        {
            int n = G.VertexCount;
            int m = G.EdgeCount;
            int p = s.Length;

            int[] memory = new int[n];
            Array.Fill(memory, int.MaxValue);
            Queue<(int, int)> q = [];
            List<int> final = [];

            foreach (var sV in s)
            {
                q.Enqueue((sV, 1));
                memory[sV] = 1;
                final.Add(sV);
            }

            int currDay = 1;

            while(currDay != K)
            {
            }

            //for (int i = 1; i < K; i++)
            //{
            //    foreach (var sV in dayArray[i])
            //    {
            //        foreach (var neighbor in G.OutNeighbors(sV))
            //        {
            //            if (isWorking(neighbor, i + 1, serviceTurnoffDay, serviceTurnonDay))
            //            {
            //                dayArray[i + 1].Add(neighbor);
            //            }
            //        }
            //    }
            //}
            //dayArray[K].Sort();
            //return (dayArray[K].Count, dayArray[K].ToArray());   

            //while (q.Count > 0)
            //{
            //    var (vertex, time) = q.Dequeue();
            //    if (!final.Contains(vertex) && (time < serviceTurnoffDay[vertex] || time > serviceTurnonDay[vertex]))
            //    {
            //        final.Add(vertex);
            //    }
            //    if (time == K)
            //    {
            //        continue;
            //    }
            //    if (time + 1 >= serviceTurnoffDay[vertex])
            //    {
            //        if (serviceTurnonDay[vertex] < K + 1)
            //        {
            //            q.Enqueue((vertex, serviceTurnonDay[vertex]));
            //        }
            //        continue;
            //    }


            //    foreach (int neighbor in G.OutNeighbors(vertex))
            //    {
                    
            //        q.Enqueue((neighbor, time + 1));
            //    }
            //}
            //final.Sort();

            //return (final.Count, final.ToArray());
        }

        public static bool isWorking(int vertex, int time, int[] serviceTurnoffDay, int[] serviceTurnonDay)
        {
            return serviceTurnoffDay[vertex] <= time || serviceTurnonDay[vertex] > time;
        }
    }
}
