using SocialNetworkApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SociaGraph.UI
{
    public partial class MainWindow : Window
    {
        public Graph myGraph;
        private Node selectedNode = null;
        private Node previousSelectedNode = null;

        private bool isDragging = false;
        private Node draggedNode = null;

        public MainWindow()
        {
            InitializeComponent();
            myGraph = new Graph();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "SosyalAg_Verisi";
            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON Files (*.json)|*.json";

            if (dlg.ShowDialog() == true)
            {
                FileManager.SaveGraph(myGraph, dlg.FileName);
                MessageBox.Show("Graf başarıyla kaydedildi! 💾");
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON Files (*.json)|*.json";

            if (dlg.ShowDialog() == true)
            {
                Graph loadedGraph = FileManager.LoadGraph(dlg.FileName);

                if (loadedGraph != null)
                {
                    myGraph = loadedGraph;
                    Coloring.WelshPowellColor(myGraph);
                    DrawGraph();
                    MessageBox.Show($"Başarıyla yüklendi! Toplam Düğüm: {myGraph.Nodes.Count}");
                }
            }
        }

        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (myGraph == null || myGraph.Nodes.Count == 0) return;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            foreach (var node in myGraph.Nodes) node.Neighbors.Clear();
            foreach (var edge in myGraph.Edges)
            {
                if (!edge.Source.Neighbors.Contains(edge.Target)) edge.Source.Neighbors.Add(edge.Target);
                if (!edge.Target.Neighbors.Contains(edge.Source)) edge.Target.Neighbors.Add(edge.Source);
            }

            var top5List = GraphAlgorithms.GetTopInfluencers(myGraph, 5);
            int componentCount = GraphAlgorithms.CalculateConnectedComponents(myGraph);

            sw.Stop();

            lblPerformance.Text = $"{sw.ElapsedMilliseconds} ms";

            lstAlgorithmResults.Items.Clear();
            lstAlgorithmResults.Items.Add("--- 🏆 EN ETKİLİ 5 KULLANICI ---");
            lstAlgorithmResults.Items.Add($"Ayrık Topluluk: {componentCount}");
            lstAlgorithmResults.Items.Add("-----------------------------");

            int sira = 1;
            foreach (var node in top5List)
            {
                string bilgi = $"{sira}. {node.Name} (Bağ: {node.Neighbors.Count}, Puan: {node.InteractionCount})";
                lstAlgorithmResults.Items.Add(bilgi);
                sira++;
            }
        }

        private void btnColoring_Click(object sender, RoutedEventArgs e)
        {
            if (myGraph == null || myGraph.Nodes.Count == 0) return;

            foreach (var node in myGraph.Nodes)
            {
                node.Neighbors.Clear();
                node.NodeColor = System.Drawing.Color.White;
            }

            foreach (var edge in myGraph.Edges)
            {
                if (!edge.Source.Neighbors.Contains(edge.Target)) edge.Source.Neighbors.Add(edge.Target);
                if (!edge.Target.Neighbors.Contains(edge.Source)) edge.Target.Neighbors.Add(edge.Source);
            }

            Coloring.WelshPowellColor(myGraph);
            DrawGraph();
        }

        private void DrawGraph()
        {
            mainCanvas.Children.Clear();

            if (myGraph == null) return;

            foreach (var edge in myGraph.Edges)
            {
                Line line = new Line();
                line.X1 = edge.Source.Location.X;
                line.Y1 = edge.Source.Location.Y;
                line.X2 = edge.Target.Location.X;
                line.Y2 = edge.Target.Location.Y;

                var drawingColor = edge.EdgeColor;
                System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(
                    drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);

                line.Stroke = new SolidColorBrush(wpfColor);
                line.StrokeThickness = edge.Thickness;

                mainCanvas.Children.Add(line);

                double midX = (line.X1 + line.X2) / 2;
                double midY = (line.Y1 + line.Y2) / 2;

                TextBlock txtWeight = new TextBlock();
                txtWeight.Text = edge.Weight.ToString("0.0000");
                txtWeight.Foreground = Brushes.DarkSlateGray;
                txtWeight.Background = Brushes.White;
                txtWeight.FontSize = 10;
                txtWeight.Padding = new Thickness(2);

                Canvas.SetLeft(txtWeight, midX - 10);
                Canvas.SetTop(txtWeight, midY - 10);
                mainCanvas.Children.Add(txtWeight);
            }

            foreach (var node in myGraph.Nodes)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 40;
                ellipse.Height = 40;

                var drawingColor = node.NodeColor;
                if (node == selectedNode) drawingColor = System.Drawing.Color.HotPink;

                System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(
                    drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
                ellipse.Fill = new SolidColorBrush(wpfColor);

                ellipse.Stroke = Brushes.Black;
                ellipse.StrokeThickness = 1;

                Canvas.SetLeft(ellipse, node.Location.X - 20);
                Canvas.SetTop(ellipse, node.Location.Y - 20);
                mainCanvas.Children.Add(ellipse);

                TextBlock txt = new TextBlock();
                txt.Text = node.Id.ToString();
                txt.Foreground = Brushes.Black;
                txt.FontWeight = FontWeights.Bold;

                Canvas.SetLeft(txt, node.Location.X - 5);
                Canvas.SetTop(txt, node.Location.Y - 10);
                mainCanvas.Children.Add(txt);
            }
        }

        private void mainCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(mainCanvas);
            Node clickedNode = null;

            foreach (var node in myGraph.Nodes)
            {
                double distance = Math.Sqrt(Math.Pow(node.Location.X - p.X, 2) + Math.Pow(node.Location.Y - p.Y, 2));
                if (distance < 25)
                {
                    clickedNode = node;
                    break;
                }
            }

            if (e.ClickCount == 2)
            {
                if (clickedNode != null)
                {
                    if (previousSelectedNode != null && previousSelectedNode != clickedNode)
                    {
                        Edge newEdge = new Edge(previousSelectedNode, clickedNode);
                        myGraph.Edges.Add(newEdge);

                        if (!previousSelectedNode.Neighbors.Contains(clickedNode)) previousSelectedNode.Neighbors.Add(clickedNode);
                        if (!clickedNode.Neighbors.Contains(previousSelectedNode)) clickedNode.Neighbors.Add(previousSelectedNode);

                        Coloring.WelshPowellColor(myGraph);
                        previousSelectedNode = null;
                    }
                }
                else
                {
                    int newId = myGraph.Nodes.Count + 1;
                    Node newNode = new Node(newId, "User " + newId, new System.Drawing.Point((int)p.X, (int)p.Y));

                    Random rnd = new Random();
                    newNode.ActivityScore = Math.Round(rnd.NextDouble(), 2);
                    newNode.InteractionCount = rnd.Next(0, 100);
                    newNode.NodeColor = System.Drawing.Color.Blue;

                    myGraph.Nodes.Add(newNode);
                    Coloring.WelshPowellColor(myGraph);
                }
            }
            else if (e.ClickCount == 1)
            {
                if (chkMoveMode.IsChecked == true && clickedNode != null)
                {
                    isDragging = true;
                    draggedNode = clickedNode;
                    return;
                }

                if (clickedNode != null)
                {
                    // CTRL + SHIFT veya diğer tuş kombinasyonları kontrolü
                    if (selectedNode != null && selectedNode != clickedNode &&
                        (System.Windows.Input.Keyboard.Modifiers == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift) ||
                         System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift ||
                         System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control ||
                         System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Alt))
                    {
                        RunAlgorithm(clickedNode);
                        return;
                    }

                    lblNodeId.Text = clickedNode.Id.ToString();
                    lblNodeName.Text = clickedNode.Name;
                    lblNodeActivity.Text = clickedNode.ActivityScore.ToString("0.00");
                    lblNodeInteraction.Text = clickedNode.InteractionCount.ToString();
                    lblNodeScore.Text = clickedNode.InteractionCount.ToString();
                    lblNodeDegree.Text = clickedNode.Neighbors.Count.ToString();
                    lstNeighbors.Items.Clear();
                    foreach (var neighbor in clickedNode.Neighbors) lstNeighbors.Items.Add($"➡ {neighbor.Name}");

                    previousSelectedNode = selectedNode;
                    selectedNode = clickedNode;
                }
                else
                {
                    selectedNode = null;
                    previousSelectedNode = null;
                }
            }

            DrawGraph();
        }

        private void RunAlgorithm(Node targetNode)
        {
            List<Node> path = null;
            System.Drawing.Color pathColor = System.Drawing.Color.Gray;
            string algoName = "";

            try
            {
                // 1. ÖNCELİK: DFS (CTRL + SHIFT)
                if (System.Windows.Input.Keyboard.Modifiers == (System.Windows.Input.ModifierKeys.Control | System.Windows.Input.ModifierKeys.Shift))
                {
                    path = GraphAlgorithms.DFS_FindPath(selectedNode, targetNode);
                    pathColor = System.Drawing.Color.Magenta;
                    algoName = "DFS";
                }
                // 2. DIJKSTRA (SHIFT)
                else if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
                {
                    path = GraphAlgorithms.Dijkstra_ShortestPath(myGraph, selectedNode, targetNode);
                    pathColor = System.Drawing.Color.LimeGreen;
                    algoName = "Dijkstra";
                }
                // 3. BFS (CTRL)
                else if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
                {
                    path = GraphAlgorithms.BFS_ShortestPath(myGraph, selectedNode, targetNode);
                    pathColor = System.Drawing.Color.OrangeRed;
                    algoName = "BFS";
                }
                // 4. A* (ALT)
                else if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Alt)
                {
                    path = GraphAlgorithms.AStar_Path(myGraph, selectedNode, targetNode);
                    pathColor = System.Drawing.Color.DeepSkyBlue;
                    algoName = "A*";
                }

                if (path != null && path.Count > 0)
                {
                    VisualizePath(path, pathColor);
                    lstAlgorithmResults.Items.Clear();
                    lstAlgorithmResults.Items.Add($"--- {algoName} ---");
                    lstAlgorithmResults.Items.Add($"Adım: {path.Count - 1}");
                    foreach (var n in path) lstAlgorithmResults.Items.Add($"⬇ {n.Name}");
                }
                else
                {
                    MessageBox.Show($"{algoName} ile yol bulunamadı!", "Sonuç", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DrawGraph();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void mainCanvas_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(mainCanvas);
            Node clickedNode = null;
            foreach (var node in myGraph.Nodes)
            {
                double distance = Math.Sqrt(Math.Pow(node.Location.X - p.X, 2) + Math.Pow(node.Location.Y - p.Y, 2));
                if (distance < 25)
                {
                    clickedNode = node;
                    break;
                }
            }

            if (clickedNode != null)
            {
                myGraph.Edges.RemoveAll(edge => edge.Source == clickedNode || edge.Target == clickedNode);
                myGraph.Nodes.Remove(clickedNode);
                if (selectedNode == clickedNode) selectedNode = null;

                foreach (var node in myGraph.Nodes) node.Neighbors.Remove(clickedNode);

                Coloring.WelshPowellColor(myGraph);
                DrawGraph();
            }
        }

        private void mainCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Released)
            {
                isDragging = false;
                draggedNode = null;
                return;
            }

            if (isDragging && draggedNode != null)
            {
                System.Windows.Point p = e.GetPosition(mainCanvas);
                draggedNode.Location = new System.Drawing.Point((int)p.X, (int)p.Y);
                DrawGraph();
            }
        }

        private void mainCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                if (draggedNode != null)
                {
                    Coloring.WelshPowellColor(myGraph);
                    draggedNode = null;
                }
                DrawGraph();
            }
        }

        private void VisualizePath(List<Node> path, System.Drawing.Color color)
        {
            foreach (var node in myGraph.Nodes) node.NodeColor = System.Drawing.Color.White;

            if (myGraph.Nodes.Count > 0)
            {
                foreach (var node in myGraph.Nodes) node.Neighbors.Clear();
                foreach (var edge in myGraph.Edges)
                {
                    if (!edge.Source.Neighbors.Contains(edge.Target)) edge.Source.Neighbors.Add(edge.Target);
                    if (!edge.Target.Neighbors.Contains(edge.Source)) edge.Target.Neighbors.Add(edge.Source);
                }
                Coloring.WelshPowellColor(myGraph);
            }

            foreach (var edge in myGraph.Edges)
            {
                edge.Thickness = 2;
                edge.EdgeColor = System.Drawing.Color.Gray;
            }

            if (path == null || path.Count == 0)
            {
                DrawGraph();
                return;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                Node n1 = path[i];
                Node n2 = path[i + 1];

                var edgeToColor = myGraph.Edges.FirstOrDefault(edge =>
                    (edge.Source == n1 && edge.Target == n2) ||
                    (edge.Source == n2 && edge.Target == n1));

                if (edgeToColor != null)
                {
                    edgeToColor.EdgeColor = color;
                    edgeToColor.Thickness = 5;
                }
            }
            DrawGraph();
        }

        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            myGraph = new Graph();
            selectedNode = null;
            previousSelectedNode = null;

            Random rnd = new Random();
            int nodeCount = 50;
            int minDistance = 60;

            for (int i = 1; i <= nodeCount; i++)
            {
                int x = 0;
                int y = 0;
                bool cakismaVar = true;
                int denemeSayisi = 0;

                while (cakismaVar && denemeSayisi < 1000)
                {
                    x = rnd.Next(50, (int)mainCanvas.ActualWidth - 50);
                    y = rnd.Next(50, (int)mainCanvas.ActualHeight - 50);

                    cakismaVar = false;

                    foreach (var existingNode in myGraph.Nodes)
                    {
                        double dx = existingNode.Location.X - x;
                        double dy = existingNode.Location.Y - y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance < minDistance)
                        {
                            cakismaVar = true;
                            break;
                        }
                    }
                    denemeSayisi++;
                }

                if (!cakismaVar)
                {
                    Node newNode = new Node(i, "User " + i, new System.Drawing.Point(x, y));

                    newNode.ActivityScore = Math.Round(rnd.NextDouble(), 2);
                    newNode.InteractionCount = rnd.Next(5, 50);
                    newNode.NodeColor = System.Drawing.Color.Blue;

                    myGraph.AddNode(newNode);
                }
            }

            foreach (var node in myGraph.Nodes)
            {
                int baglantiSayisi = rnd.Next(1, 4);

                for (int k = 0; k < baglantiSayisi; k++)
                {
                    if (myGraph.Nodes.Count > 1)
                    {
                        Node targetNode = myGraph.Nodes[rnd.Next(0, myGraph.Nodes.Count)];

                        if (targetNode != node)
                        {
                            myGraph.AddEdge(node, targetNode);
                        }
                    }
                }
            }

            Coloring.WelshPowellColor(myGraph);
            DrawGraph();

            MessageBox.Show($"{myGraph.Nodes.Count} düğümlü, çakışmasız ağ oluşturuldu! 🚀");
        }
    }
}