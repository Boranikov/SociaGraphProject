using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace SocialNetworkApp
{
    public partial class Form1 : Form
    {
        private Graph graph;
        private Node selectedNode;
        private Edge selectedEdge;
        private const int NodeRadius = 20;
        private int nextId = 1;
        private bool isDragging = false;
        private Point dragOffset;
        private List<Node> shortestPath = null;
        private List<Node> dfsPath = null;
        private List<Node> djkPath = null;
        private List<Node> AStarPath = null;


        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.DoubleBuffered = true;
            this.Size = new Size(1920, 1000);
            this.Text = "Sosyal Ağ Grafiği";
            createTestGraph(100, 0.1);
        }

        // ------------------------TEST ETME -----------------------------------// 
        private void createTestGraph(int nodeCount, double ConnectionProb)
        {
            Random random = new Random();
            graph = new Graph(); // Mevcut Grafiği sıfırla
            List<string> names = new List<string> {"Boran", "Bülent", "Yağız", "Ahmet", "Eren", "Melih", "Ensar", "Berk", "Nuh", "Emir",
            "Göksel", "Efe", "Mustafa", "Ömer", "Mert", "Ferda", "Ayhan", "Emirhan", "Can", "Onur",
            "Zeynep", "Kerem", "Doğu", "Zöhre", "Yelda", "Emine", "Nazlı", "Ferdasu", "İbrahim",
            "Deniz" }; // İsimler
            int minDistance = 200; // Düğümler arası mesafe

            // 1. Düğümleri oluştur

            for(int i =0; i < nodeCount; i++)
            {
                Point location;
                bool PositionFound = false;
                int attempts = 0;

                // Çakışma Önleme döngüsü

                do
                {
                    int x = random.Next(50, this.ClientSize.Width -50);
                    int y = random.Next(50, this.ClientSize.Height -50);
                    location = new Point(x, y);

                    // Üst üste binme Kontrolü
                    bool overlap = false;
                    foreach(var node in graph.Nodes)
                    {
                        double dist = Math.Sqrt(Math.Pow(location.X - node.Location.X, 2) + Math.Pow(location.Y - node.Location.Y, 2));
                        if (dist < minDistance)
                        {
                            overlap = true;
                            break;
                        }
                    }
                    if(!overlap) PositionFound = true;
                    attempts++;
                } while(!PositionFound && attempts < 100);

                if (!PositionFound) continue; // Yer bulamazsan bu düğümü atla

                string name = names.Count > i ? names[i] : $"User_{i + 1}";
                graph.AddNode(new Node(i + 1, name, location));
            }
            // 2. Kenarları Oluştur
            var nodeList = new List<Node>(graph.Nodes);
            for (int i = 0; i < nodeList.Count; i++)
            {
                for (int j = i + 1; j < nodeList.Count; j++)
                {
                    if (random.NextDouble() < ConnectionProb)
                    {
                        graph.AddEdge(nodeList[i], nodeList[j]);
                    }
                }
            }

            // ID sayacını güncelle (Elle ekleme yaparsan çakışmasın diye)
            nextId = nodeList.Count + 1;

            // Ekranı yenile
            this.Invalidate();
        }

        // ------------------------------TEST ETME -----------------------------------//

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

            // Coloring sınıfındaki Draw metodunu çağırıyoruz

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
        private Edge GetEdgeAt(Point p)
        {

            foreach (var edge in graph.Edges)
            {
                // Sanal bir çizim yolu oluştur
                using (GraphicsPath path = new GraphicsPath())
                {
                    // Bu yola bizim kenarı ekle
                    path.AddLine(edge.Source.Location, edge.Target.Location);

                    if (path.IsOutlineVisible(p, new Pen(Color.Black, 10)))
                    {
                        return edge;
                    }
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
                // DÜZELTME BURADA: ConnectionCount yerine Neighbors.Count yazıldı.
                MessageBox.Show($"Seçilen Kişi: {clickedNode.Name}\nBağlantılar: {clickedNode.Neighbors.Count}\nId: {clickedNode.Id}");
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
            
            Stopwatch stw = new Stopwatch();

            if (e.Button == MouseButtons.Left)
            {
                Node clickedNode = GetNodeAt(e.Location);

                // 1. Durum: SHIFT Basılı (BFS - Sarı Yol)
                if (Control.ModifierKeys == Keys.Shift && selectedNode != null && clickedNode != null)
                {
                    stw.Start();
                    shortestPath = GraphAlgorithms.BFS_ShortestPath(graph, selectedNode, clickedNode);
                    dfsPath = null;
                    djkPath = null;
                    AStarPath = null;
                    stw.Stop();
                    MessageBox.Show($"BFS Süre: {stw.Elapsed.TotalMilliseconds}");

                    if (shortestPath == null) MessageBox.Show("BFS ile bağlantı bulunamadı!");
                }
                // 2. Durum: ALT Basılı (DFS - Mor Yol)
                else if (Control.ModifierKeys == Keys.Alt && selectedNode != null && clickedNode != null)
                {
                    stw.Restart();
                    dfsPath = GraphAlgorithms.DFS_FindPath(selectedNode, clickedNode);
                    shortestPath = null;
                    djkPath = null;
                    AStarPath = null;
                    stw.Stop();
                    MessageBox.Show($"DFS Süre: {stw.Elapsed.TotalMilliseconds}");
                    if (dfsPath == null) MessageBox.Show("DFS ile bağlantı bulunamadı!");
                }
                // 3. Durum : CTRL Basılı (Djikstra - Yeşil Yol)
                else if (Control.ModifierKeys == Keys.Control && selectedNode != null && clickedNode != null)
                {
                    stw.Restart();
                    djkPath = GraphAlgorithms.Dijkstra_ShortestPath(graph, selectedNode, clickedNode);

                    shortestPath = null;
                    dfsPath = null;
                    AStarPath = null;
                    stw.Stop();
                    MessageBox.Show($"Djikstra Süre: {stw.Elapsed.TotalMilliseconds}");
                    if (djkPath == null) MessageBox.Show("DJK ile bağlantı bulunamadı!");
                }
                // 4. Durum : CTRL + SHİFT Basılı (A* - Kırmızı Yol)
                else if ((Control.ModifierKeys & (Keys.Control | Keys.Shift)) == (Keys.Control | Keys.Shift) && selectedNode != null && clickedNode != null)
                {
                    stw.Restart();
                    AStarPath = GraphAlgorithms.AStar_Path(graph, selectedNode, clickedNode);
                    shortestPath = null;
                    dfsPath = null;
                    djkPath = null;
                    stw.Stop();
                    MessageBox.Show($"A* Süre: {stw.Elapsed.TotalMilliseconds}");
                    if (AStarPath == null) MessageBox.Show("AStar ile bağlantı bulunamadı");
                }
                // 5. Durum: Hiçbir Tuş Yok (Sürükleme veya Seçim)
                else
                {
                    shortestPath = null;
                    dfsPath = null;
                    djkPath = null;
                    AStarPath = null;

                    if (clickedNode != null)
                    {
                        isDragging = true;
                        selectedNode = clickedNode;
                        selectedEdge = null;
                        dragOffset = new Point(e.Location.X - clickedNode.Location.X, e.Location.Y - clickedNode.Location.Y);
                    }
                    else
                    {
                        Edge clickedEdge = GetEdgeAt(e.Location); //Edge seçme

                        if (clickedEdge != null)
                        {
                            selectedEdge = clickedEdge;
                            selectedNode = null;
                        }
                        else
                        {
                            selectedEdge = null;
                            selectedNode = null;
                        }
                        isDragging = false;
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
        protected override void OnKeyDown(KeyEventArgs e) // Silme İşlemi
        {
            base.OnKeyDown(e);
            if(e.KeyCode == Keys.Delete && selectedNode != null) // Delete tuşu ile silme
            {
                graph.RemoveNode(selectedNode); // Node Silme
                e.Handled = true; // Başka tuşlarla karışmasın
                this.Invalidate(); // Ekranı yeniden çiz
            }
            else if(e.KeyCode == Keys.Delete && selectedEdge != null)
            {
                graph.RemoveEdge(selectedEdge);
                e.Handled = true; // Başka tuşlarla karışmasın
                this.Invalidate(); // Ekranı yeniden çiz
            }
        }
    }
}