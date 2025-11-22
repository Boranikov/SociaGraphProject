using System.Collections.Generic;
using System.Linq;

namespace SocialNetworkApp
{
    public class Graph
    {
        public List<Node> Nodes { get; set; }
        public List<Edge> Edges { get; set; }

        public Graph()
        {
            Nodes = new List<Node>();
            Edges = new List<Edge>();
        }

        // --- EKLEME İŞLEMLERİ ---

        public void AddNode(Node node)
        {
            Nodes.Add(node);
        }

        public void AddEdge(Node source, Node target)
        {
            bool exists = Edges.Any(e =>
                (e.Source == source && e.Target == target) ||
                (e.Source == target && e.Target == source));

            if (!exists)
            {
                Edge newEdge = new Edge(source, target);
                Edges.Add(newEdge);

                source.Neighbors.Add(target);
                target.Neighbors.Add(source);

                source.ConnectionCount++;
                target.ConnectionCount++;
            }
        }

        // --- SİLME İŞLEMLERİ  ---

        public void RemoveNode(Node nodeToRemove)
        {
            if (nodeToRemove == null) return;

            // 1. Önce bu düğüme bağlı olan tüm kenarlar silinmeli
            // Listeyi tersten dönüyoruz ki silerken index kaymamalı
            for (int i = Edges.Count - 1; i >= 0; i--)
            {
                var edge = Edges[i];
                if (edge.Source == nodeToRemove || edge.Target == nodeToRemove)
                {
                    // Kenarı sil
                    RemoveEdge(edge);
                }
            }

            // 2. Düğümü listeden sil
            Nodes.Remove(nodeToRemove);
        }

        public void RemoveEdge(Edge edgeToRemove)
        {
            if (edgeToRemove == null) return;

            // Komşuluk listelerinden birbirlerini silsinler
            edgeToRemove.Source.Neighbors.Remove(edgeToRemove.Target);
            edgeToRemove.Target.Neighbors.Remove(edgeToRemove.Source);

            // Bağlantı sayılarını düşür
            edgeToRemove.Source.ConnectionCount--;
            edgeToRemove.Target.ConnectionCount--;

            // Ana listeden sil
            Edges.Remove(edgeToRemove);
        }
    }
}