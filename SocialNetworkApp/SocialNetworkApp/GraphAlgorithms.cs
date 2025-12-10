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
            var previous = new Dictionary<Node, Node>(); //Hangi düðüme nerden geldik
            var queue = new Queue<Node>(); //BFS için kuyruk
            var visited = new HashSet<Node>(); //Ziyaret edilen düðümler

            queue.Enqueue(start); //Baþlangýç düðümünü kuyruða ekle
            visited.Add(start); //Baþlangýç düðümünü ziyaret edilenlere ekle
            while (queue.Count > 0)
            {
                var current = queue.Dequeue(); //Kuyruktan düðüm çýkar
                if (current == end) break; //Hedefe ulaþýldýysa döngüyü kýr
                foreach (var neighbor in current.Neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor); //Komþuyu ziyaret edilenlere ekle
                        previous[neighbor] = current; //Komþunun önceki düðümünü ayarla
                        queue.Enqueue(neighbor); //Komþuyu kuyruða ekle
                    }
                }
            }
            if (!previous.ContainsKey(end) && start != end)
            {
                return null; //Hedefe ulaþýlamadýysa null döndür
            }
            var path = new List<Node>();
            var curr = end;
            while (curr != null && previous.ContainsKey(curr))
            {
                path.Add(curr);
                curr = previous[curr];
            }
            path.Add(start); //Baþlangýç düðümünü ekle
            path.Reverse(); //Yolu ters çevir
            return path; //Yolu döndür
        }
        // ----------------------------- BFS BÝTTÝ --------------------------------------//
        // ----------------------------- DFS --------------------------------------//
        public static List<Node> DFS_FindPath(Node start, Node end) 
        {
            if (start == null || end == null) return null;
            var previous = new Dictionary<Node, Node>(); //Hangi düðüme nerden geldik
            var stack = new Stack<Node>(); //DFS için yýðýn
            var visited = new HashSet<Node>(); //Ziyaret edilen düðümler
            stack.Push(start); //Baþlangýç düðümünü yýðýna ekle
            while (stack.Count > 0) 
            {
                var current = stack.Pop(); //stacktan en son eklenen düðüm çýkar
                if (!visited.Contains(current)) 
                {
                    visited.Add(current);
                    if (current == end) break; //Hedefe ulaþýldýysa döngüyü kýr
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
            // Yolu geriye doðru oluþtur
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
        // ----------------------------- DFS BÝTTÝ --------------------------------------//
        // ----------------------------- Dijkstra --------------------------------------//
        public static List<Node> Dijkstra_ShortestPath(Graph graph, Node start, Node end)
        {
            var distance = new Dictionary<Node, double>(); 
            var previous = new Dictionary<Node, Node>();
            var nodesToVisit = new List<Node>();
            // Baþlangýç her nokta sonsuz mesafe, baþlangýç 0
            foreach (var node in graph.Nodes)
            {
                distance[node] = double.MaxValue;
                nodesToVisit.Add(node);
            }
            distance[start] = 0;
            while (nodesToVisit.Count > 0)
            {
                // Mesafesi en küçük olan düðümü seç
                nodesToVisit.Sort((x,y) =>  distance[x].CompareTo(distance[y]));
                var current = nodesToVisit[0];
                nodesToVisit.RemoveAt(0);
                // Hedef ulaþýldýysa veya kalanlar sonsuz uzaklýksa dur
                if (current == end) break;
                if (distance[current] == double.MaxValue) break;
                // Komþularý ve kenar aðýrlýklarýný bul
                foreach ( var edge in graph.Edges)
                {
                    Node neighbor = null;
                    if (edge.Source == current) neighbor = edge.Target;
                    else if (edge.Target == current) neighbor = edge.Source;
                    if (neighbor != null && nodesToVisit.Contains(neighbor))
                    {
                        double newDist = distance[current] + edge.Weight; // Mevcut yol + Kenar Aðýrlýðý

                        // Daha kýsa bir yol bulduysak güncelle
                        if (newDist < distance[neighbor])
                        {
                            distance[neighbor] = newDist;
                            previous[neighbor] = current;
                        }
                    }
                }
            }
            // Yolu geriye doðru oluþtur
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
        // ----------------------------- Dijkstra BÝTTÝ --------------------------------------//
        // ----------------------------- A* --------------------------------------//
        public static List<Node> AStar_Path(Graph graph, Node start ,Node end)
        {
            // A* gerekli listeler
            var openSet = new List<Node>();
            var previous = new Dictionary<Node, Node >();
            // gScore: Baþlangýçtan buraya kadar olan gerçek maliyet
            var gScore = new Dictionary<Node, double>();
            // fScore: gScore + Heuristic (Tahmini toplam maliyet)
            var fScore = new Dictionary<Node, double>();
            // Tüm düðümler için baþlangýç deðerini sonsuz yap
            foreach (var node in graph.Nodes)
            {
                gScore[node] = double.MaxValue;
                fScore[node] = double.MaxValue;
            }
            // Baþlangýç ayarý 
            gScore[start] = 0;
            fScore[start] = Heuristic(start, end);
            openSet.Add(start);
            while (openSet.Count > 0)
            {
                // fSroce en düþük seç
                openSet.Sort((x,y) => fScore[x].CompareTo(fScore[y]));
                var current = openSet[0];
                // Hedef ulaþýldý mý ?
                if (current == end)
                {
                    return RePath(previous, start, end);
                }
                openSet.RemoveAt(0);

                // Komþularý gezme

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

                            // Gerçek maliyet + Kuþ uçuþu kalan mesafe
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
            // Euclidean (Pisagor Teoremi): Ýki nokta arasýndaki kuþ uçuþu mesafe
            // Formül: Kök içinde ((x1-x2)^2 + (y1-y2)^2)

            double deltaX = a.Location.X - b.Location.X;
            double deltaY = a.Location.Y - b.Location.Y;

            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
        }

        // --- Yolu Geriye Doðru Oluþturma ---
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
        // ----------------------------- A* BÝTTÝ --------------------------------------//
    }
}
