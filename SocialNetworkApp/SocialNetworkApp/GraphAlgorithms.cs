using System;
using System.Collections.Generic;
using System.Linq;

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

        // 2. WELSH-POWELL RENKLENDÝRME
        public static void WelshPowellColor(Graph graph)
        {
            var sortedNodes = graph.Nodes.OrderByDescending(n => n.Neighbors.Count).ToList();
            var colors = new List<System.Drawing.Color>
            {
                System.Drawing.Color.Red, System.Drawing.Color.Blue, System.Drawing.Color.Green,
                System.Drawing.Color.Yellow, System.Drawing.Color.Orange, System.Drawing.Color.Purple,
                System.Drawing.Color.Cyan, System.Drawing.Color.Magenta, System.Drawing.Color.Brown,
                System.Drawing.Color.Pink, System.Drawing.Color.Lime, System.Drawing.Color.Teal
            };

            int colorIndex = 0;
            while (sortedNodes.Count > 0)
            {
                var currentColor = colors[colorIndex % colors.Count];
                var coloredInThisRound = new List<Node>();

                var firstNode = sortedNodes[0];
                firstNode.NodeColor = currentColor;
                coloredInThisRound.Add(firstNode);
                sortedNodes.RemoveAt(0);

                for (int i = 0; i < sortedNodes.Count; i++)
                {
                    var node = sortedNodes[i];
                    bool isNeighbor = false;
                    foreach (var coloredNode in coloredInThisRound)
                    {
                        if (node.Neighbors.Contains(coloredNode))
                        {
                            isNeighbor = true;
                            break;
                        }
                    }

                    if (!isNeighbor)
                    {
                        node.NodeColor = currentColor;
                        coloredInThisRound.Add(node);
                        sortedNodes.RemoveAt(i);
                        i--;
                    }
                }
                colorIndex++;
            }
        }

        // --- DIJKSTRA ---
        public static List<Node> Dijkstra_ShortestPath(Graph graph, Node start, Node end)
        {
            var distances = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();
            var unvisited = new List<Node>();

            foreach (var node in graph.Nodes)
            {
                distances[node] = double.MaxValue;
                previous[node] = null;
                unvisited.Add(node);
            }

            distances[start] = 0;

            while (unvisited.Count > 0)
            {
                unvisited.Sort((a, b) => distances[a].CompareTo(distances[b]));
                var current = unvisited[0];
                unvisited.RemoveAt(0);

                if (current == end) break;
                if (distances[current] == double.MaxValue) break;

                // Kenarlarý bul (Aðýrlýk için)
                foreach (var neighbor in current.Neighbors)
                {
                    if (!unvisited.Contains(neighbor)) continue;

                    var edge = graph.Edges.FirstOrDefault(e =>
                        (e.Source == current && e.Target == neighbor) ||
                        (e.Source == neighbor && e.Target == current));

                    if (edge != null)
                    {
                        double alt = distances[current] + edge.Weight;
                        if (alt < distances[neighbor])
                        {
                            distances[neighbor] = alt;
                            previous[neighbor] = current;
                        }
                    }
                }
            }

            return ReconstructPath(previous, end);
        }

        // --- BFS ---
        public static List<Node> BFS_ShortestPath(Graph graph, Node start, Node end)
        {
            var previous = new Dictionary<Node, Node>();
            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == end) break;

                foreach (var neighbor in current.Neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        previous[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return ReconstructPath(previous, end);
        }

        // --- A* (A-STAR) ---
        public static List<Node> AStar_Path(Graph graph, Node start, Node end)
        {
            var gScore = new Dictionary<Node, double>();
            var fScore = new Dictionary<Node, double>();
            var previous = new Dictionary<Node, Node>();
            var openSet = new List<Node> { start };

            foreach (var node in graph.Nodes)
            {
                gScore[node] = double.MaxValue;
                fScore[node] = double.MaxValue;
            }

            gScore[start] = 0;
            fScore[start] = Heuristic(start, end);

            while (openSet.Count > 0)
            {
                openSet.Sort((a, b) => fScore[a].CompareTo(fScore[b]));
                var current = openSet[0];

                if (current == end) return ReconstructPath(previous, end);

                openSet.RemoveAt(0);

                foreach (var neighbor in current.Neighbors)
                {
                    var edge = graph.Edges.FirstOrDefault(e =>
                        (e.Source == current && e.Target == neighbor) ||
                        (e.Source == neighbor && e.Target == current));

                    if (edge == null) continue;

                    double tentative_gScore = gScore[current] + edge.Weight;

                    if (tentative_gScore < gScore[neighbor])
                    {
                        previous[neighbor] = current;
                        gScore[neighbor] = tentative_gScore;
                        fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);

                        if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private static double Heuristic(Node a, Node b)
        {
            // Öklid mesafesi (Kuþ uçuþu)
            return Math.Sqrt(Math.Pow(a.Location.X - b.Location.X, 2) + Math.Pow(a.Location.Y - b.Location.Y, 2));
        }

        private static List<Node> ReconstructPath(Dictionary<Node, Node> previous, Node end)
        {
            if (!previous.ContainsKey(end) && previous.Count > 0) return null; // Yol yoksa

            var path = new List<Node>();
            var current = end;
            while (current != null)
            {
                path.Add(current);
                if (previous.ContainsKey(current))
                    current = previous[current];
                else
                    current = null;
            }
            path.Reverse();
            return path.Count > 1 ? path : null;
        }
        // --- DFS (Derinlik Öncelikli Arama) ---
        public static List<Node> DFS_FindPath(Node start, Node end)
        {
            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();
            var parentMap = new Dictionary<Node, Node>(); // Yolu geri izlemek için

            stack.Push(start);
            visited.Add(start);
            parentMap[start] = null;

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current == end) return ReconstructPath(parentMap, end);

                foreach (var neighbor in current.Neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        parentMap[neighbor] = current;
                        stack.Push(neighbor);
                    }
                }
            }
            return null; // Yol bulunamadý
        }
        // --- BAÐLI BÝLEÞEN SAYISI (Connected Components) ---
        // Ýster 3.2: Ayrýk topluluklarýn tespiti
        public static int CalculateConnectedComponents(Graph graph)
        {
            if (graph.Nodes.Count == 0) return 0;

            HashSet<Node> visited = new HashSet<Node>();
            int componentCount = 0;

            foreach (var node in graph.Nodes)
            {
                // Eðer bu düðüme daha önce hiç uðramadýysak, yeni bir "ada" bulduk demektir.
                if (!visited.Contains(node))
                {
                    componentCount++;
                    // Bu adadaki herkesi ziyaret edildi olarak iþaretle (BFS ile)
                    MarkComponent(node, visited);
                }
            }
            return componentCount;
        }

        // Yardýmcý Fonksiyon (Sadece bu adayý gezmek için)
        private static void MarkComponent(Node startNode, HashSet<Node> visited)
        {
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbor in current.Neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
    }

}