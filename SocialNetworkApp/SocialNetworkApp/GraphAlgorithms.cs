using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetworkApp
{
    internal class GraphAlgorithms
    {
        public static List<Node> BFS_ShortestPath(Graph graph, Node start, Node end)
        {
            if (start == null || end == null) return null;
            var previous = new Dictionary<Node, Node>(); //Hangi düğüme nerden geldik
            var queue = new Queue<Node>(); //BFS için kuyruk
            var visited = new HashSet<Node>(); //Ziyaret edilen düğümler

            queue.Enqueue(start); //Başlangıç düğümünü kuyruğa ekle
            visited.Add(start); //Başlangıç düğümünü ziyaret edilenlere ekle
            while (queue.Count > 0)
            {
                var current = queue.Dequeue(); //Kuyruktan düğüm çıkar
                if (current == end) break; //Hedefe ulaşıldıysa döngüyü kır
                foreach (var neighbor in current.Neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor); //Komşuyu ziyaret edilenlere ekle
                        previous[neighbor] = current; //Komşunun önceki düğümünü ayarla
                        queue.Enqueue(neighbor); //Komşuyu kuyruğa ekle
                    }
                }
            }
            if (!previous.ContainsKey(end) && start != end)
            {
                return null; //Hedefe ulaşılamadıysa null döndür
            }
            var path = new List<Node>();
            var curr = end;
            while (curr != null && previous.ContainsKey(curr))
            {
                path.Add(curr);
                curr = previous[curr];
            }
            path.Add(start); //Başlangıç düğümünü ekle
            path.Reverse(); //Yolu ters çevir
            return path; //Yolu döndür
        }
    }
}
