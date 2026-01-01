using SocialNetworkApp; // Node, Graph, Edge sınıfları burada
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
        // 1. GRAF NESNESİ
        public Graph myGraph;
        private Node selectedNode = null;

        private bool isDragging = false; // Şu an sürüklüyor muyuz?
        private Node draggedNode = null; // Hangi düğümü sürüklüyoruz?

        public MainWindow()
        {
            InitializeComponent();
            // Program açılınca boş bir graf oluştur
            myGraph = new Graph();
        }

        // --- 1. KAYDET BUTONU ---
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

        // --- 2. YÜKLE BUTONU ---
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

                    // Yüklenen grafiği renklendir ve çiz
                    GraphAlgorithms.WelshPowellColor(myGraph);
                    DrawGraph();

                    MessageBox.Show($"Başarıyla yüklendi! Toplam Düğüm: {myGraph.Nodes.Count}");
                }
            }
        }

        // --- 3. ANALİZ BUTONU (TOP 5) ---
        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (myGraph == null || myGraph.Nodes.Count == 0) return;

            // 1. SAYAÇ BAŞLAT (Performans Ölçümü İçin)
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // 2. BAĞLANTILARI GÜNCELLE (Hata olmasın diye)
            foreach (var node in myGraph.Nodes) node.Neighbors.Clear();
            foreach (var edge in myGraph.Edges)
            {
                if (!edge.Source.Neighbors.Contains(edge.Target)) edge.Source.Neighbors.Add(edge.Target);
                if (!edge.Target.Neighbors.Contains(edge.Source)) edge.Target.Neighbors.Add(edge.Source);
            }

            // 3. ALGORİTMAYI ÇALIŞTIR
            var top5List = GraphAlgorithms.GetTopInfluencers(myGraph, 5);

            // 4. SAYAÇ DURDUR
            sw.Stop();

            // 5. SONUÇLARI SOL PANELE YAZDIR
            lblPerformance.Text = $"{sw.ElapsedMilliseconds} ms"; // Süreyi yaz

            lstAlgorithmResults.Items.Clear(); // Listeyi temizle
            lstAlgorithmResults.Items.Add("--- 🏆 EN ETKİLİ 5 KULLANICI ---");

            int sira = 1;
            foreach (var node in top5List)
            {
                // DÜZELTME: node.Interaction -> node.InteractionCount
                string bilgi = $"{sira}. {node.Name} (Bağ: {node.Neighbors.Count}, Puan: {node.InteractionCount})";
                lstAlgorithmResults.Items.Add(bilgi);
                sira++;
            }
        }

        // --- 4. RENKLENDİR BUTONU ---
        private void btnColoring_Click(object sender, RoutedEventArgs e)
        {
            if (myGraph == null || myGraph.Nodes.Count == 0) return;

            // ADIM 1: KOMŞULUKLARI GÜNCELLE
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

            // ADIM 2: ALGORİTMAYI ÇALIŞTIR
            GraphAlgorithms.WelshPowellColor(myGraph);

            // ADIM 3: TEKRAR ÇİZ
            DrawGraph();
        }

        // --- 5. ÇİZİM FONKSİYONU (CANVAS) ---
        private void DrawGraph()
        {
            mainCanvas.Children.Clear(); // Tahtayı temizle

            if (myGraph == null) return;

            // A) KENARLARI VE AĞIRLIKLARI ÇİZ
            foreach (var edge in myGraph.Edges)
            {
                // 1. Çizgiyi Çiz
                Line line = new Line();
                line.X1 = edge.Source.Location.X;
                line.Y1 = edge.Source.Location.Y;
                line.X2 = edge.Target.Location.X;
                line.Y2 = edge.Target.Location.Y;

                // Rengi ve Kalınlığı Edge nesnesinden al
                var drawingColor = edge.EdgeColor;
                System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(
                    drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);

                line.Stroke = new SolidColorBrush(wpfColor);
                line.StrokeThickness = edge.Thickness;

                mainCanvas.Children.Add(line);

                // 2. Ağırlığı Yazdır
                double midX = (line.X1 + line.X2) / 2;
                double midY = (line.Y1 + line.Y2) / 2;

                TextBlock txtWeight = new TextBlock();
                // Formül çok küçük sayı ürettiği için virgülden sonra 3 hane gösterelim
                txtWeight.Text = edge.Weight.ToString("0.000");
                txtWeight.Foreground = Brushes.DarkSlateGray;
                txtWeight.Background = Brushes.White;
                txtWeight.FontSize = 10;
                txtWeight.Padding = new Thickness(2);

                Canvas.SetLeft(txtWeight, midX - 10);
                Canvas.SetTop(txtWeight, midY - 10);
                mainCanvas.Children.Add(txtWeight);
            }

            // B) DÜĞÜMLERİ ÇİZ
            foreach (var node in myGraph.Nodes)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 40;
                ellipse.Height = 40;

                var drawingColor = node.NodeColor;
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

        // --- 6. SOL TIKLAMA OLAYI (GÜNCELLENMİŞ) ---
        private void mainCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(mainCanvas);
            Node clickedNode = null;

            // 1. Tıklanan düğümü bul
            foreach (var node in myGraph.Nodes)
            {
                double distance = Math.Sqrt(Math.Pow(node.Location.X - p.X, 2) + Math.Pow(node.Location.Y - p.Y, 2));
                if (distance < 25)
                {
                    clickedNode = node;
                    break;
                }
            }

            // --- A) TAŞIMA MODU ---
            if (chkMoveMode.IsChecked == true)
            {
                if (clickedNode != null)
                {
                    isDragging = true;
                    draggedNode = clickedNode;
                }
                return;
            }

            // --- B) ALGORİTMA KISAYOLLARI ---
            if (clickedNode != null && selectedNode != null && clickedNode != selectedNode &&
                (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift ||
                 System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control ||
                 System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Alt))
            {
                List<Node> path = null;
                System.Drawing.Color pathColor = System.Drawing.Color.Gray;
                string algoName = "";

                try
                {
                    if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
                    {
                        path = GraphAlgorithms.Dijkstra_ShortestPath(myGraph, selectedNode, clickedNode);
                        pathColor = System.Drawing.Color.LimeGreen;
                        algoName = "Dijkstra";
                    }
                    else if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
                    {
                        path = GraphAlgorithms.BFS_ShortestPath(myGraph, selectedNode, clickedNode);
                        pathColor = System.Drawing.Color.OrangeRed;
                        algoName = "BFS";
                    }
                    else if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Alt)
                    {
                        path = GraphAlgorithms.AStar_Path(myGraph, selectedNode, clickedNode);
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

                        selectedNode = null; // İşlem bitince seçimi temizle
                    }
                    else
                    {
                        MessageBox.Show($"{algoName} ile yol bulunamadı!", "Sonuç", MessageBoxButton.OK, MessageBoxImage.Warning);
                        DrawGraph();
                        selectedNode = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                    selectedNode = null;
                }
                return;
            }

            // --- C) NORMAL İŞLEMLER ---
            if (clickedNode != null)
            {
                // 1. SOL PANELİ GÜNCELLE
                lblNodeId.Text = clickedNode.Id.ToString();
                lblNodeName.Text = clickedNode.Name;

                // İSTERLERE UYGUN GÜNCELLEMELER
                lblNodeActivity.Text = clickedNode.ActivityScore.ToString("0.00");
                lblNodeInteraction.Text = clickedNode.InteractionCount.ToString();
                lblNodeScore.Text = clickedNode.InteractionCount.ToString(); // Puan olarak Etkileşim sayısını gösteriyoruz
                lblNodeDegree.Text = clickedNode.Neighbors.Count.ToString();

                lstNeighbors.Items.Clear();
                foreach (var neighbor in clickedNode.Neighbors) lstNeighbors.Items.Add($"➡ {neighbor.Name}");

                // 2. BAĞLANTI MANTIĞI
                if (selectedNode == null)
                {
                    selectedNode = clickedNode;
                    selectedNode.NodeColor = System.Drawing.Color.Red;
                }
                else if (selectedNode == clickedNode)
                {
                    selectedNode = null;
                    GraphAlgorithms.WelshPowellColor(myGraph);
                }
                else
                {
                    Edge newEdge = new Edge(selectedNode, clickedNode);
                    myGraph.Edges.Add(newEdge);

                    if (!selectedNode.Neighbors.Contains(clickedNode)) selectedNode.Neighbors.Add(clickedNode);
                    if (!clickedNode.Neighbors.Contains(selectedNode)) clickedNode.Neighbors.Add(selectedNode);

                    GraphAlgorithms.WelshPowellColor(myGraph);
                    selectedNode = null;
                }
            }
            else
            {
                // BOŞLUĞA TIKLANDI -> YENİ DÜĞÜM EKLE
                int newId = myGraph.Nodes.Count + 1;
                Node newNode = new Node(newId, "User " + newId, new System.Drawing.Point((int)p.X, (int)p.Y));

                // İSTER 4.3: RASTGELE ÖZELLİKLER
                Random rnd = new Random();
                newNode.ActivityScore = Math.Round(rnd.NextDouble(), 2);
                newNode.InteractionCount = rnd.Next(0, 100);

                newNode.NodeColor = System.Drawing.Color.Blue;
                myGraph.Nodes.Add(newNode);
                GraphAlgorithms.WelshPowellColor(myGraph);

                if (selectedNode != null) selectedNode = null;
            }

            DrawGraph();
        }

        // --- SAĞ TIK İLE SİLME ---
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

                // Silinince komşulukları da güncellemek gerekir
                foreach (var node in myGraph.Nodes) node.Neighbors.Remove(clickedNode);

                GraphAlgorithms.WelshPowellColor(myGraph);
                DrawGraph();
            }
        }

        // --- SÜRÜKLEME ---
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
                    // Welsh-Powell rengine geri dönsün diye yeniden boyayabiliriz
                    GraphAlgorithms.WelshPowellColor(myGraph);
                    draggedNode = null;
                }
                DrawGraph();
            }
        }

        // --- YOLU GÖRSELLEŞTİRME ---
        private void VisualizePath(List<Node> path, System.Drawing.Color color)
        {
            // 1. DÜĞÜM RENKLERİNİ SIFIRLA VE YENİDEN HESAPLA
            foreach (var node in myGraph.Nodes) node.NodeColor = System.Drawing.Color.White;

            if (myGraph.Nodes.Count > 0)
            {
                foreach (var node in myGraph.Nodes) node.Neighbors.Clear();
                foreach (var edge in myGraph.Edges)
                {
                    if (!edge.Source.Neighbors.Contains(edge.Target)) edge.Source.Neighbors.Add(edge.Target);
                    if (!edge.Target.Neighbors.Contains(edge.Source)) edge.Target.Neighbors.Add(edge.Source);
                }
                GraphAlgorithms.WelshPowellColor(myGraph);
            }

            // 2. ÇİZGİLERİ SIFIRLA
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

            // 3. SADECE YOL ÇİZGİLERİNİ BOYA
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
        // --- RASTGELE GRAF OLUŞTURUCU (İster 4.5 İçin) ---
        private void btnRandom_Click(object sender, RoutedEventArgs e)
        {
            // 1. Önce Grafiği Temizle
            myGraph = new Graph();
            selectedNode = null;

            Random rnd = new Random();
            int nodeCount = 50; // Düğüm sayısı
            int minDistance = 60; // İki düğüm arasındaki minimum mesafe (Çap 40px ise 60px iyidir)

            // 2. DÜĞÜMLERİ OLUŞTUR
            for (int i = 1; i <= nodeCount; i++)
            {
                int x = 0;
                int y = 0;
                bool cakismaVar = true;
                int denemeSayisi = 0;

                // Güvenli bir yer bulana kadar döngü kur (Max 1000 deneme)
                while (cakismaVar && denemeSayisi < 1000)
                {
                    // Rastgele koordinat üret
                    x = rnd.Next(50, (int)mainCanvas.ActualWidth - 50);
                    y = rnd.Next(50, (int)mainCanvas.ActualHeight - 50);

                    cakismaVar = false;

                    // Mevcut düğümlerle mesafe kontrolü yap
                    foreach (var existingNode in myGraph.Nodes)
                    {
                        double dx = existingNode.Location.X - x;
                        double dy = existingNode.Location.Y - y;
                        double distance = Math.Sqrt(dx * dx + dy * dy); // Pisagor

                        // Eğer çok yakınsa, çakışma var demektir
                        if (distance < minDistance)
                        {
                            cakismaVar = true;
                            break; // Diğerlerine bakmaya gerek yok, yeni sayı üret
                        }
                    }
                    denemeSayisi++;
                }

                // Eğer 1000 denemede yer bulamadıysa (ekran dolduysa) o düğümü pas geçebiliriz
                // Ama genellikle 50 düğüm için yer bulur.
                if (!cakismaVar)
                {
                    Node newNode = new Node(i, "User " + i, new System.Drawing.Point(x, y));

                    // İsterlere uygun rastgele veriler
                    newNode.ActivityScore = Math.Round(rnd.NextDouble(), 2);
                    newNode.InteractionCount = rnd.Next(10, 500);
                    newNode.NodeColor = System.Drawing.Color.Blue;

                    myGraph.AddNode(newNode);
                }
            }

            // 3. RASTGELE BAĞLANTILAR (KENARLAR) KUR
            foreach (var node in myGraph.Nodes)
            {
                int baglantiSayisi = rnd.Next(1, 4);

                for (int k = 0; k < baglantiSayisi; k++)
                {
                    if (myGraph.Nodes.Count > 1) // Hata önlemek için kontrol
                    {
                        Node targetNode = myGraph.Nodes[rnd.Next(0, myGraph.Nodes.Count)];

                        if (targetNode != node)
                        {
                            myGraph.AddEdge(node, targetNode);
                        }
                    }
                }
            }

            // 4. SON RÖTUŞLAR
            GraphAlgorithms.WelshPowellColor(myGraph);
            DrawGraph();

            MessageBox.Show($"{myGraph.Nodes.Count} düğümlü, çakışmasız ağ oluşturuldu! 🚀");
        }
    }
}