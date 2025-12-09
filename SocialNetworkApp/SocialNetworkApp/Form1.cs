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
        private List<Node> dfsPath = null;
        private List<Node> djkPath = null;
        private List<Node> AStarPath = null;
        private Coloring coloring = new Coloring();

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

                // Coloring sınıfındaki Draw metodunu çağırıyoruz
                coloring.Draw(e.Graphics, graph, selectedNode, shortestPath, dfsPath, djkPath, AStarPath);
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

                // 1. Durum: SHIFT Basılı (BFS - Sarı Yol)
                if (Control.ModifierKeys == Keys.Shift && selectedNode != null && clickedNode != null)
                {
                    shortestPath = GraphAlgorithms.BFS_ShortestPath(graph, selectedNode, clickedNode);
                    dfsPath = null;
                    djkPath = null;
                    AStarPath = null;

                    if (shortestPath == null) MessageBox.Show("BFS ile bağlantı bulunamadı!");
                }
                // 2. Durum: ALT Basılı (DFS - Mor Yol)
               
                else if (Control.ModifierKeys == Keys.Alt && selectedNode != null && clickedNode != null)
                {
                    
                    dfsPath = GraphAlgorithms.DFS_FindPath(selectedNode, clickedNode);

                    shortestPath = null; // Diğer yolları temizle
                    djkPath=null;
                    AStarPath = null;

                    if (dfsPath == null) MessageBox.Show("DFS ile bağlantı bulunamadı!");
                }

                // 3. Durum : CTRL Basılı (Djikstra - Yeşil Yol)

                else if (Control.ModifierKeys == Keys.Control && selectedNode != null && clickedNode != null)
                {
                    djkPath = GraphAlgorithms.Dijkstra_ShortestPath(graph, selectedNode, clickedNode);

                    shortestPath = null;
                    dfsPath = null;   // Diğer yolları temizle
                    AStarPath = null;

                    if (djkPath == null) MessageBox.Show("DJK ile bağlantı bulunamadı!");
                }

                // 4. Durum : CTRL + SHİFT Basılı (A* - Kırmızı Yol)

                else if ((Control.ModifierKeys & (Keys.Control | Keys.Shift)) == (Keys.Control | Keys.Shift) && selectedNode != null && clickedNode != null)
                {
                    AStarPath = GraphAlgorithms.AStar_Path(graph, selectedNode, clickedNode);
                    shortestPath = null;
                    dfsPath = null;
                    djkPath = null;

                    if (AStarPath == null) MessageBox.Show("AStar ile bağlantı bulunamadı");
                }

                // 5. Durum: Hiçbir Tuş Yok 
                else
                {
                    shortestPath = null; // Tıklayınca eski çizimleri temizle
                    dfsPath = null;
                    djkPath = null;
                    AStarPath=null;

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
                this.Invalidate(); // Ekranı yenile
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