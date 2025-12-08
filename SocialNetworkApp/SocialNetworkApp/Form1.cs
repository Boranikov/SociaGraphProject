using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocialNetworkApp
{
    public partial class Form1 : Form
    {
        private Graph graph;
        private Node selectedNode; 
        private const int NodeRadius = 20;
        private int nextId = 5;
        private bool isDragging = false;
        private Point dragOffset;
        private List<Node> shortestPath = null;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Size = new Size(800, 600);
            this.Text = "Sosyal Ağ Grafiği";
            createTestGraph(); 
        }

        private void createTestGraph() 
        {
            graph = new Graph();
            // Test düğümleri oluşturma
            Node nodeA = new Node(1, "Alice", new Point(100, 100));
            Node nodeB = new Node(2, "Bob", new Point(300, 100));
            Node nodeC = new Node(3, "Charlie", new Point(200, 300));
            Node nodeD = new Node(4, "Diana", new Point(500, 200));

            graph.AddNode(nodeA);
            graph.AddNode(nodeB);
            graph.AddNode(nodeC);
            graph.AddNode(nodeD);

            // Test kenarları oluşturma
            graph.AddEdge(nodeA, nodeB);
            graph.AddEdge(nodeA, nodeC);
            graph.AddEdge(nodeB, nodeC);
            graph.AddEdge(nodeB, nodeD);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // --- SARI YOL ÇİZİMİ ---
            if (shortestPath != null && shortestPath.Count > 1)
            {
                using (Pen pathPen = new Pen(Color.Gold, 5))
                {
                    for (int i = 0; i < shortestPath.Count - 1; i++)
                    {
                        g.DrawLine(pathPen, shortestPath[i].Location, shortestPath[i + 1].Location);
                    }
                }
            }

            // Kenarları çiz
            foreach (var edge in graph.Edges)
            {
                Pen pen = new Pen(Color.Gray, 2);
                g.DrawLine(pen, edge.Source.Location, edge.Target.Location);

                // Ağırlık etiketini çiz
                Point midPoint = new Point(
                    (edge.Source.Location.X + edge.Target.Location.X) / 2,
                    (edge.Source.Location.Y + edge.Target.Location.Y) / 2);

                g.DrawString(edge.Weight.ToString(), this.Font, Brushes.Black, midPoint);
            }

            // Düğümleri çiz
            foreach (var node in graph.Nodes)
            {
                Brush brush = (node == selectedNode) ? Brushes.Red : Brushes.Blue;
                g.FillEllipse(brush, node.Location.X - NodeRadius, node.Location.Y - NodeRadius, NodeRadius * 2, NodeRadius * 2);
                g.DrawString(node.Name, this.Font, Brushes.White, node.Location.X - NodeRadius / 2, node.Location.Y - NodeRadius / 2);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Right)
            {
                Node clickedNode = GetNodeAt(e.Location);
                if (selectedNode != null && clickedNode != null && selectedNode != clickedNode)
                {
                    graph.AddEdge(selectedNode, clickedNode); // Kenar ekle
                }
            }
            this.Invalidate(); // Ekranı yeniden çiz
        }

        private Node GetNodeAt(Point p)
        {
            foreach (var node in graph.Nodes)
            {
                double distance = Math.Sqrt(Math.Pow(p.X - node.Location.X, 2) + Math.Pow(p.Y - node.Location.Y, 2));
                if (distance <= NodeRadius)
                {
                    return node;
                }
            }
            return null;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            Node clickedNode = GetNodeAt(e.Location);

            if (selectedNode != null) // Var olan düğüm seçildiyse onun bilgilerini göster
            {
                MessageBox.Show($"Seçilen Kişi: {clickedNode.Name}\nBağlantılar: {clickedNode.ConnectionCount}\nId: {clickedNode.Id}");
            }
            else // Yeni düğüm ekle
            {
                int currentId = nextId++;
                Node newNode = new Node(currentId, $"User{currentId}", e.Location);
                graph.AddNode(newNode);
                this.Invalidate(); // Ekranı yeniden çiz
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                Node clickedNode = GetNodeAt(e.Location);

                // SHIFT TUŞU KONTROLÜ
                if (Control.ModifierKeys == Keys.Shift && selectedNode != null && clickedNode != null)
                {
                    shortestPath = GraphAlgorithms.BFS_ShortestPath(graph, selectedNode, clickedNode);
                    if (shortestPath == null) MessageBox.Show("Bağlantı bulunamadı!");
                }
                else
                {
                    shortestPath = null; // Eski yolu temizle
                    if (clickedNode != null)
                    {
                        isDragging = true;
                        selectedNode = clickedNode;
                        dragOffset = new Point(e.Location.X - clickedNode.Location.X, e.Location.Y - clickedNode.Location.Y);
                    }
                    else
                    {
                        isDragging = false;
                        selectedNode = null;
                    }
                }
                this.Invalidate();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging && selectedNode != null)
            {
                Point newLocation = new Point(e.X - dragOffset.X, e.Y - dragOffset.Y);
                selectedNode.Location = newLocation;
                this.Invalidate(); // Ekranı yeniden çiz
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (isDragging)
            {
                isDragging = false; // Sürüklemeyi bitir
            }
        }
    }
}