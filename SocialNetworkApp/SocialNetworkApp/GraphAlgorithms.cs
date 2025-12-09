using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace SocialNetworkApp
{
    internal class GraphAlgorithms
    {
        // ----------------------------- BFS --------------------------------------//
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
        // ----------------------------- BFS BİTTİ --------------------------------------//
        // ----------------------------- DFS --------------------------------------//
        public static List<Node> DFS_FindPath(Node start, Node end) 
        {
            if (start == null || end == null) return null;
            var previous = new Dictionary<Node, Node>(); //Hangi düğüme nerden geldik
            var stack = new Stack<Node>(); //DFS için yığın
            var visited = new HashSet<Node>(); //Ziyaret edilen düğümler
            stack.Push(start); //Başlangıç düğümünü yığına ekle
            while (stack.Count > 0) 
            {
                var current = stack.Pop(); //stacktan en son eklenen düğüm çıkar
                if (!visited.Contains(current)) 
                {
                    visited.Add(current);
                    if (current == end) break; //Hedefe ulaşıldıysa döngüyü kır
                    foreach (var neighbor in current.Neighbors) 
                    {
                        if (!visited.Contains(neighbor))
                        {
                            if (!previous.ContainsKey(neighbor))
                            {
                                previous[neighbor] = current;
                                stack.Push(neighbor);
                            }
                        }
                    }
                }
            }
            // Yolu geriye doğru oluştur
            if (!previous.ContainsKey(end) && start != end) return null;

            var path = new List<Node>();
            var curr = end;
            while (curr != null && previous.ContainsKey(curr))
            {
                path.Add(curr);
                curr = previous[curr];
            }
            path.Add(start);
            path.Reverse();

            return path;
        }
        // ----------------------------- DFS BİTTİ --------------------------------------//
        // ----------------------------- Dijkstra --------------------------------------//
        public static List<Node> Dijkstra_ShortestPath(Graph graph, Node start, Node end)
        {
            var distance = new Dictionary<Node, double>(); 
            var previous = new Dictionary<Node, Node>();
            var nodesToVisit = new List<Node>();
            // Başlangıç her nokta sonsuz mesafe, başlangıç 0
            foreach (var node in graph.Nodes)
            {
                distance[node] = double.MaxValue;
                nodesToVisit.Add(node);
            }
            distance[start] = 0;
            while (nodesToVisit.Count > 0)
            {
                // Mesafesi en küçük olan düğümü seç
                nodesToVisit.Sort((x,y) =>  distance[x].CompareTo(distance[y]));
                var current = nodesToVisit[0];
                nodesToVisit.RemoveAt(0);
                // Hedef ulaşıldıysa veya kalanlar sonsuz uzaklıksa dur
                if (current == end) break;
                if (distance[current] == double.MaxValue) break;
                // Komşuları ve kenar ağırlıklarını bul
                foreach ( var edge in graph.Edges)
                {
                    Node neighbor = null;
                    if (edge.Source == current) neighbor = edge.Target;
                    else if (edge.Target == current) neighbor = edge.Source;
                    if (neighbor != null && nodesToVisit.Contains(neighbor))
                    {
                        double newDist = distance[current] + edge.Weight; // Mevcut yol + Kenar Ağırlığı

                        // Daha kısa bir yol bulduysak güncelle
                        if (newDist < distance[neighbor])
                        {
                            distance[neighbor] = newDist;
                            previous[neighbor] = current;
                        }
                    }
                }
            }
            // Yolu geriye doğru oluştur
            if (!previous.ContainsKey(end) && start != end) return null;

            var path = new List<Node>();
            var curr = end;
            while (curr != null && previous.ContainsKey(curr))
            {
                path.Add(curr);
                curr = previous[curr];
            }
            path.Add(start);
            path.Reverse();

            return path;
        }
        // ----------------------------- Dijkstra BİTTİ --------------------------------------//
        // ----------------------------- A* --------------------------------------//
        public static List<Node> AStar_Path(Graph graph, Node start ,Node end)
        {
            // A* gerekli listeler
            var openSet = new List<Node>();
            var previous = new Dictionary<Node, Node >();
            // gScore: Başlangıçtan buraya kadar olan gerçek maliyet
            var gScore = new Dictionary<Node, double>();
            // fScore: gScore + Heuristic (Tahmini toplam maliyet)
            var fScore = new Dictionary<Node, double>();
            // Tüm düğümler için başlangıç değerini sonsuz yap
            foreach (var node in graph.Nodes)
            {
                gScore[node] = double.MaxValue;
                fScore[node] = double.MaxValue;
            }
            // Başlangıç ayarı 
            gScore[start] = 0;
            fScore[start] = Heuristic(start, end);
            openSet.Add(start);
            while (openSet.Count > 0)
            {
                // fSroce en düşük seç
                openSet.Sort((x,y) => fScore[x].CompareTo(fScore[y]));
                var current = openSet[0];
                // Hedef ulaşıldı mı ?
                if (current == end)
                {
                    return RePath(previous, start, end);
                }
                openSet.RemoveAt(0);

                // Komşuları gezme

                foreach (var edge in graph.Edges)
                {
                    Node neighbor = null;
                    if (edge.Source == current)
                    {
                        neighbor = edge.Target;
                    }
                    else if (edge.Target == current)
                    {
                        neighbor = edge.Source;
                    }

                    if (neighbor != null)
                    {
                        // Yeni maliyeti hesaplama
                        double score = gScore[current] + edge.Weight;
                        if (score < gScore[neighbor])
                        {
                            previous[neighbor]=current;
                            gScore[neighbor] = score;

                            // Gerçek maliyet + Kuş uçuşu kalan mesafe
                            fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);

                            if (!openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                            }
                        }
                    }
                }

            }
            return null; // Yol yoksa
        }
        private static double Heuristic(Node a, Node b)
        {
            // Euclidean (Pisagor Teoremi): İki nokta arasındaki kuş uçuşu mesafe
            // Formül: Kök içinde ((x1-x2)^2 + (y1-y2)^2)

            double deltaX = a.Location.X - b.Location.X;
            double deltaY = a.Location.Y - b.Location.Y;

            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
        }

        // --- Yolu Geriye Doğru Oluşturma ---
        private static List<Node> RePath(Dictionary<Node, Node> previous, Node start, Node end)
        {
            var path = new List<Node>();
            var curr = end;
            while (curr != null && previous.ContainsKey(curr))
            {
                path.Add(curr);
                curr = previous[curr];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
        // ----------------------------- A* BİTTİ --------------------------------------//
    }
}
