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

            // TEST VERİSİ: Boran backend'i bağlayana kadar bununla test et.
            Node testNode = new Node
            {
                Id = 1,
                Label = "Göksel",
                X = 150,
                Y = 100,
                Activity = 0.8,
                Interaction = 12
            };

            DrawNode(testNode); // Artık sayı değil, NESNE gönderiyoruz.
        }

        private void DrawNode(Node dataNode)
        {
            // 1. Düğümün Kendisi (Daire)
            Ellipse visualNode = new Ellipse()
            {
                Width = 50,
                Height = 50,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Tag = dataNode // ÖNEMLİ: Veriyi görselin içine gizliyoruz (PDF Madde 4.1 OOP kuralı)
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