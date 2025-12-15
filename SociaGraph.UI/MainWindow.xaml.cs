using SocialNetworkApp; // Arkadaşının projesi
using System;
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
            FileManager fm = new FileManager();
            fm.SaveGraph(myGraph);
        }

        // --- 2. YÜKLE BUTONU ---
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            FileManager fm = new FileManager();
            Graph loadedGraph = fm.LoadGraph();

            if (loadedGraph != null)
            {
                myGraph = loadedGraph;
                DrawGraph(); // Yüklenince hemen çiz
                MessageBox.Show($"Başarıyla yüklendi! Toplam Düğüm: {myGraph.Nodes.Count}");
            }
        }

        // --- 3. ANALİZ BUTONU (TOP 5) ---
        private void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (myGraph == null || myGraph.Nodes.Count == 0)
            {
                MessageBox.Show("Analiz edilecek veri yok!", "Hata", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var top5List = GraphAlgorithms.GetTopInfluencers(myGraph, 5);

            string mesaj = "🏆 EN POPÜLER 5 KULLANICI 🏆\n\n";
            int sira = 1;
            foreach (var node in top5List)
            {
                mesaj += $"{sira}. {node.Name}\n   └ Bağlantı: {node.ConnectionCount}\n   └ Puan: {node.Interaction}\n\n";
                sira++;
            }
            MessageBox.Show(mesaj, "Analiz Sonucu", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // --- 4. RENKLENDİR BUTONU ---
        private void btnColoring_Click(object sender, RoutedEventArgs e)
        {
            if (myGraph == null || myGraph.Nodes.Count == 0) return;

            // ADIM 1: KOMŞULUKLARI GÜNCELLE (Bunu yapmazsak hepsi kırmızı olur!)
            // Önce herkesin hafızasını temizle
            foreach (var node in myGraph.Nodes)
            {
                node.Neighbors.Clear(); // Varsa eski komşu bilgilerini sil
                node.NodeColor = System.Drawing.Color.White; // Rengini sıfırla
            }

            // Şimdi Çizgiler (Edges) listesine bakarak kim kiminle komşu tekrar yaz
            foreach (var edge in myGraph.Edges)
            {
                // Source'un komşusu Target'tır
                if (!edge.Source.Neighbors.Contains(edge.Target))
                    edge.Source.Neighbors.Add(edge.Target);

                // Target'ın komşusu Source'tur (Çift yönlü)
                if (!edge.Target.Neighbors.Contains(edge.Source))
                    edge.Target.Neighbors.Add(edge.Source);
            }

            // ADIM 2: ALGORİTMAYI ÇALIŞTIR
            // Artık herkes komşusunu tanıyor, algoritma doğru çalışacak
            GraphAlgorithms.WelshPowellColor(myGraph);

            // ADIM 3: TEKRAR ÇİZ
            DrawGraph();

            MessageBox.Show("Renklendirme Başarılı! Herkes komşusundan farklı renkte olmalı.", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // --- 5. ÇİZİM FONKSİYONU (CANVAS) ---
        private void DrawGraph()
        {
            mainCanvas.Children.Clear(); // Tahtayı temizle

            if (myGraph == null) return;

            // A) KENARLARI ÇİZ
            foreach (var edge in myGraph.Edges)
            {
                Line line = new Line();
                line.X1 = edge.Source.Location.X;
                line.Y1 = edge.Source.Location.Y;
                line.X2 = edge.Target.Location.X;
                line.Y2 = edge.Target.Location.Y;
                line.Stroke = Brushes.Gray;
                line.StrokeThickness = 2;
                mainCanvas.Children.Add(line);
            }

            // B) DÜĞÜMLERİ ÇİZ
            foreach (var node in myGraph.Nodes)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 40;
                ellipse.Height = 40;

                // Rengi dönüştür (System.Drawing -> WPF)
                var drawingColor = node.NodeColor;
                Color wpfColor = Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
                ellipse.Fill = new SolidColorBrush(wpfColor);

                ellipse.Stroke = Brushes.Black;
                ellipse.StrokeThickness = 1;

                // Konumu ayarla (Ortalamak için -20)
                Canvas.SetLeft(ellipse, node.Location.X - 20);
                Canvas.SetTop(ellipse, node.Location.Y - 20);
                mainCanvas.Children.Add(ellipse);

                // İsmi yaz
                TextBlock txt = new TextBlock();
                txt.Text = node.Id.ToString();
                txt.Foreground = Brushes.Black;
                txt.FontWeight = FontWeights.Bold;

                Canvas.SetLeft(txt, node.Location.X - 5);
                Canvas.SetTop(txt, node.Location.Y - 10);
                mainCanvas.Children.Add(txt);
            }
        }

        // --- 6. BOŞ TIKLAMA OLAYI (Hata vermemesi için) ---
        private void mainCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(mainCanvas);
            Node clickedNode = null;

            // Tıklanan düğümü bul
            foreach (var node in myGraph.Nodes)
            {
                double distance = Math.Sqrt(Math.Pow(node.Location.X - p.X, 2) + Math.Pow(node.Location.Y - p.Y, 2));
                if (distance < 25)
                {
                    clickedNode = node;
                    break;
                }
            }

            // --- MOD KONTROLÜ ---
            // Eğer "Taşıma Modu" kutucuğu işaretliyse:
            if (chkMoveMode.IsChecked == true)
            {
                if (clickedNode != null)
                {
                    isDragging = true;
                    draggedNode = clickedNode;
                    clickedNode.NodeColor = System.Drawing.Color.Orange; // Taşındığını belli et
                }
                return; // Fonksiyonu burada kes, aşağıya (bağlantı koduna) inmesin!
            }

            // --- NORMAL BAĞLANTI MODU (Eski kodların burası) ---
            if (clickedNode != null)
            {
                // ... (Senin mevcut bağlantı kurma kodların burada kalacak) ...
                // KOD TEKRARI OLMASIN DİYE KISALTTIM, SEN ESKİ KODUNU KORU
                // Sadece en başa if (chkMoveMode.IsChecked == true) bloğunu ekle yeter.

                // Örnek:
                if (selectedNode == null)
                {
                    selectedNode = clickedNode;
                    selectedNode.NodeColor = System.Drawing.Color.Red;
                }
                else if (selectedNode != clickedNode)
                {
                    // Bağlantı kur...
                    Edge newEdge = new Edge(selectedNode, clickedNode);
                    myGraph.Edges.Add(newEdge);
                    selectedNode.NodeColor = System.Drawing.Color.Blue;
                    clickedNode.NodeColor = System.Drawing.Color.Blue;
                    selectedNode = null;
                }
            }
            else
            {
                // Boşluğa tıklayınca Node ekleme kodu...
                int newId = myGraph.Nodes.Count + 1;
                // ...
                Node newNode = new Node(newId, "User " + newId, new System.Drawing.Point((int)p.X, (int)p.Y));
                newNode.NodeColor = System.Drawing.Color.Blue;
                myGraph.Nodes.Add(newNode);
            }

            DrawGraph();
        }
        // --- SAĞ TIK İLE SİLME (Eksik Olan Fonksiyon) ---
        private void mainCanvas_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(mainCanvas);

            // 1. Tıklanan yerde bir düğüm var mı?
            Node clickedNode = null;
            foreach (var node in myGraph.Nodes)
            {
                double distance = Math.Sqrt(Math.Pow(node.Location.X - p.X, 2) + Math.Pow(node.Location.Y - p.Y, 2));
                if (distance < 25) // Yarıçapın içindeyse
                {
                    clickedNode = node;
                    break;
                }
            }

            // 2. Eğer bir düğüme denk geldiysek SİL
            if (clickedNode != null)
            {
                // Önce bu düğüme bağlı olan TÜM kenarları (Edge) temizle
                myGraph.Edges.RemoveAll(edge => edge.Source == clickedNode || edge.Target == clickedNode);

                // Sonra düğümün kendisini sil
                myGraph.Nodes.Remove(clickedNode);

                // Eğer sildiğimiz düğüm o an "Seçili" olan ise, seçimi iptal et
                if (selectedNode == clickedNode) selectedNode = null;

                // Ekranı güncelle
                DrawGraph();
            }
        }
        // --- FARE HAREKET EDİNCE (SÜRÜKLEME) ---
        private void mainCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // GÜVENLİK KONTROLÜ: 
            // Eğer sürükleme var sanıyoruz ama aslında Sol Tuş basılı değilse -> DURDUR!
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Released)
            {
                isDragging = false;
                if (draggedNode != null)
                {
                    draggedNode.NodeColor = System.Drawing.Color.Blue;
                    draggedNode = null;
                }
                return;
            }

            // Normal Sürükleme Kodu
            if (isDragging && draggedNode != null)
            {
                System.Windows.Point p = e.GetPosition(mainCanvas);

                // Düğümü farenin ortasına getirmek için biraz ofset verelim mi? 
                // Yoksa tam uca mı yapışsın? Şimdilik direkt atayalım:
                draggedNode.Location = new System.Drawing.Point((int)p.X, (int)p.Y);

                DrawGraph();
            }
        }

        // --- FARE TUŞU BIRAKILINCA (BİTİR) ---
        private void mainCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                if (draggedNode != null)
                {
                    draggedNode.NodeColor = System.Drawing.Color.Blue; // Rengini normale döndür (Mavi)
                    draggedNode = null;
                }
                DrawGraph();
            }
        }
    }
}