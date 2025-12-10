using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SocialNetworkApp
{
    internal class Coloring
    {
        private const int NodeRadius = 35; // Düğüm yarıçapı

        // Ana çizim metodu
        public void Draw(Graphics g, Graph graph, Node selectedNode, Edge selectedEdge,
                         List<Node> shortestPath, List<Node> dfsPath, List<Node> djkPath, List<Node>AStarPath)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // --- 1. KATMAN: KENARLAR VE AĞIRLIKLAR (En altta) ---
            foreach (var edge in graph.Edges)
            {
                // Rengi ve Kalınlığı Belirle
                Color edgeColor;
                float edgeWidth;

                if (edge == selectedEdge)
                {
                    edgeColor = Color.DeepPink; // Seçiliyse Koyu Pembe
                    edgeWidth = 8;              // Ve daha kalın
                }
                else
                {
                    edgeColor = Color.Pink;     // Değilse normal pembe
                    edgeWidth = 5;
                }

                // Belirlenen renk ve kalınlık ile kalemi oluştur
                using (Pen pen = new Pen(edgeColor, edgeWidth))
                {
                    g.DrawLine(pen, edge.Source.Location, edge.Target.Location);
                }

                // Ağırlık yazısı (Değişmedi)
                Point midPoint = new Point(
                    (edge.Source.Location.X + edge.Target.Location.X) / 2,
                    (edge.Source.Location.Y + edge.Target.Location.Y) / 2);

                using (Font font = new Font("Arial", 12))
                {
                    g.DrawString(edge.Weight.ToString(), font, Brushes.Black, midPoint);
                }
            }

                // --- 2. KATMAN: RENKLİ YOLLAR (Ortada) ---

                // Sarı Yol (Shortest Path)
                DrawPath(g, shortestPath, Color.Gold);

            // DFS Yolu (Mor)
            DrawPath(g, dfsPath, Color.Purple);

            // Dijkstra Yolu (Yeşil)
            DrawPath(g, djkPath, Color.LimeGreen);

            // A* Yolu (Kırmızı)
            DrawPath(g, AStarPath, Color.Red);

            // --- 3. KATMAN: DÜĞÜMLER (En üstte) ---
            foreach (var node in graph.Nodes)
            {
                Brush brush = (node == selectedNode) ? Brushes.Red : Brushes.Blue;

                // Yuvarlağı çiz
                g.FillEllipse(brush, node.Location.X - NodeRadius, node.Location.Y - NodeRadius, NodeRadius * 2, NodeRadius * 2);

                // İsmi yaz
                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                {
                    g.DrawString(node.Name, font, Brushes.White, node.Location.X - NodeRadius / 2, node.Location.Y - NodeRadius / 2);
                }
            }
        }

        // Yolları çizen yardımcı metod
        private void DrawPath(Graphics g, List<Node> path, Color color)
        {
            if (path != null && path.Count > 1)
            {
                using (Pen pathPen = new Pen(color, 5))
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        g.DrawLine(pathPen, path[i].Location, path[i + 1].Location);
                    }
                }
            }
        }
    }
}
