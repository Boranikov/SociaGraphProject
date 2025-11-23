using System;

namespace SociaGraph.UI
{
    public class Edge
    {
        public Node Source { get; set; } // Başlangıç Düğümü (Nereden)
        public Node Target { get; set; } // Bitiş Düğümü (Nereye)
        public double Weight { get; set; } // Ağırlık (PDF'te istenen maliyet)

        public Edge(Node source, Node target, double weight = 1.0)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }
    }
}