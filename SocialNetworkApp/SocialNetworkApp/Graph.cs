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

        // --- EKLEME ÝÞLEMLERÝ ---
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

                // Sadece listeye ekliyoruz, sayý (Count) otomatik artýyor.
                source.Neighbors.Add(target);
                target.Neighbors.Add(source);

                // ESKÝDEN BURADA ConnectionCount++ VARDI, ARTIK YOK.
            }
        }

        // --- SÝLME ÝÞLEMLERÝ ---
        public void RemoveNode(Node nodeToRemove)
        {
            if (nodeToRemove == null) return;

            // Önce baðlý kenarlarý sil
            for (int i = Edges.Count - 1; i >= 0; i--)
            {
                var edge = Edges[i];
                if (edge.Source == nodeToRemove || edge.Target == nodeToRemove)
                {
                    RemoveEdge(edge);
                }
            }
            Nodes.Remove(nodeToRemove);
        }

        public void RemoveEdge(Edge edgeToRemove)
        {
            if (edgeToRemove == null) return;

            edgeToRemove.Source.Neighbors.Remove(edgeToRemove.Target);
            edgeToRemove.Target.Neighbors.Remove(edgeToRemove.Source);

            Edges.Remove(edgeToRemove);
        }
    }
}