using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class BacktrackingData
    {
        public int longestSequenceLength;
        public List<int> longestCorrectSequence;
        public List<int> sequence;
        public bool[] usedVertices;
        public int[] colors;
    }
    public class Lab10 : MarshalByRefObject
    {
        /// <summary>
        /// Szukanie najdłuższego powtórzenia w zadanym kolorowaniu grafu.
        /// </summary>
        /// <param name="G">Graf prosty</param>
        /// <param name="color">Kolorowanie wierzchołków G (color[v] to kolor wierzchołka v)</param>
        /// <returns>Ścieżka, na której występuje powtórzenie (numery kolejnych wierzchołków)</returns>
        /// <remarks>W przypadku braku powtórzeń należy zwrócić null lub tablicę o długości 0</remarks>
        public int[] FindLongestRepetition(Graph G, int[] color)
        {
            BacktrackingData data  = new BacktrackingData();
            data.longestSequenceLength = 0;
            data.sequence = new List<int>();
            data.longestCorrectSequence = new List<int>();
            data.usedVertices = new bool[G.VertexCount];
            data.colors = color;
            Array.Fill(data.usedVertices, false);
            for(int i = 0; i < G.VertexCount; i++)
                FindLongestRepetitionUtils(G, i, ref data);
            return data.longestCorrectSequence.ToArray();
        }

        public bool isValid(int v, BacktrackingData data)
        {
            // {1, 2, 3, 4, 1, 2}
            // data.sequence.Count = 6
            // halfLength = 3
            // firstElement = 1
            // i = 3, 4
            // isApalindromic = t
            // j = 4, 5
            if (data.usedVertices[v] == true) return false;


            int halfLength = data.sequence.Count % 2 == 0 ? data.sequence.Count / 2 : (data.sequence.Count / 2) + 1;
            int verticesLeft = data.usedVertices.Length - data.sequence.Count;

            bool isApalindromic = false;
            int firstElement = data.sequence[0];

            int longestHalfLength = -1;
            for(int i = data.sequence.Count - 1;i>=halfLength; i--)
            {
                if(data.colors[data.sequence[i]] == data.colors[firstElement])
                {
                    isApalindromic = true;
                    for(int j = i; j<data.sequence.Count; j++)
                    {
                        if (data.colors[data.sequence[j]] != data.colors[data.sequence[j - i]])
                        {
                            isApalindromic = false;
                            break;
                        }
                    }
                    if (isApalindromic && 2*i > data.longestSequenceLength)
                    {
                        longestHalfLength = i;
                        break;
                    }
                }
            }
            if(isApalindromic)
            {
                return 2*longestHalfLength <= data.usedVertices.Length;
            }
            return verticesLeft >= data.sequence.Count;

        }

        public bool isExact(BacktrackingData data)
        {
            if (data.sequence.Count % 2 != 0) return false;
            int halfLength =  data.sequence.Count / 2;

            for (int i = 0; i < halfLength; i++)
            {
                if (data.colors[data.sequence[i]] != data.colors[data.sequence[i + halfLength]])
                {
                    return false;
                }
            }
            return true;
        }

        public void FindLongestRepetitionUtils(Graph G, int currentVertex, ref BacktrackingData data)
        {
            data.usedVertices[currentVertex] = true;
            data.sequence.Add(currentVertex);

            if (isExact(data) && data.sequence.Count > data.longestSequenceLength)
            {
                data.longestSequenceLength = data.sequence.Count;
                data.longestCorrectSequence = new List<int>(data.sequence);
            }

            foreach(var v in G.OutNeighbors(currentVertex))
            {
                if (isValid(v, data))
                {
                    FindLongestRepetitionUtils(G, v, ref data);
                }
            }

            data.usedVertices[currentVertex] = false;
            data.sequence.RemoveAt(data.sequence.Count - 1);
        }
    }

}


