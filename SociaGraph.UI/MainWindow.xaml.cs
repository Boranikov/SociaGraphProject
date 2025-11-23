using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SociaGraph.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 1. Düğümü Oluştur
            Node node1 = new Node { Id = 1, Label = "Göksel", X = 100, Y = 100, Activity = 0.8, Interaction = 12 };
            DrawNode(node1);

            // 2. Düğümü Oluştur
            Node node2 = new Node { Id = 2, Label = "Boran", X = 350, Y = 200, Activity = 0.5, Interaction = 8 };
            DrawNode(node2);

            // Bu iki düğüm arasına Kenar (Edge) çiz
            Edge edge1 = new Edge(node1, node2, 5.5); // Ağırlık 5.5 olsun
            DrawEdge(edge1);

        }

        private void DrawNode(Node dataNode)
        {
            // 1. Düğüm (Daire)
            Ellipse visualNode = new Ellipse()
            {
                Width = 50,
                Height = 50,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Tag = dataNode // Veriyi görselin içine gizliyoruz
            };

            // Konumu Node nesnesinden alıyoruz
            Canvas.SetLeft(visualNode, dataNode.X);
            Canvas.SetTop(visualNode, dataNode.Y);

            // Tıklama Olayını Ekle
            visualNode.MouseLeftButtonDown += Node_MouseLeftButtonDown;

            // 2. Düğümün Yazısı (Label)
            TextBlock label = new TextBlock()
            {
                Text = dataNode.Label,
                FontSize = 14,
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                IsHitTestVisible = false // Yazıya tıklayınca daire algılansın diye bunu kapatıyoruz
            };

            // Yazıyı dairenin ortasına/altına hizalama
            Canvas.SetLeft(label, dataNode.X + 10);
            Canvas.SetTop(label, dataNode.Y + 15);

            // 3. Ekrana Ekleme (Sadece bir kez!)
            GraphCanvas.Children.Add(visualNode);
            GraphCanvas.Children.Add(label);
        }
        private void DrawEdge(Edge edge)
        {
            // 1. Çizgiyi (Line) oluşturuyoruz
            Line visualEdge = new Line()
            {
                // Çizgi düğümün (50px) tam ortasından çıksın diye +25 ekliyoruz
                X1 = edge.Source.X + 25,
                Y1 = edge.Source.Y + 25,
                X2 = edge.Target.X + 25,
                Y2 = edge.Target.Y + 25,
                Stroke = Brushes.Gray,   // Rengi gri
                StrokeThickness = 2      // Kalınlığı 2
            };

            // ÖNEMLİ: Çizgiyi düğümlerin ALTINDA kalsın diye en alta (0. sıraya) ekliyoruz
            GraphCanvas.Children.Insert(0, visualEdge);

            // 2. Ağırlığı (Weight) çizginin ortasına yazıyoruz
            TextBlock weightLabel = new TextBlock()
            {
                Text = edge.Weight.ToString("0.0"), // Sayıyı metne çevir
                Foreground = Brushes.Red,           // Rengi kırmızı
                FontWeight = FontWeights.Bold,      // Kalın yazı
                Background = Brushes.White          // Okunsun diye arkası beyaz
            };

            // Yazıyı çizginin tam ortasına hesaplayıp koyuyoruz
            double midX = (visualEdge.X1 + visualEdge.X2) / 2;
            double midY = (visualEdge.Y1 + visualEdge.Y2) / 2;

            Canvas.SetLeft(weightLabel, midX);
            Canvas.SetTop(weightLabel, midY);

            GraphCanvas.Children.Add(weightLabel);
        }

        private void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse clickedShape = sender as Ellipse;
            if (clickedShape != null)
            {
                // Görselin içindeki veriyi geri al
                Node data = clickedShape.Tag as Node;

                // Seçildiğini belli et (Renk değişimi)
                clickedShape.Fill = Brushes.Orange;

                // Bilgileri göster
                MessageBox.Show($"ID: {data.Id}\nİsim: {data.Label}\nAktiflik: {data.Activity}\nEtkileşim: {data.Interaction}", "Düğüm Bilgisi");
            }
        }
    }
}