using System.Windows;
using SocialNetworkApp; // Arkadaşının projesini buraya ekle

namespace SociaGraph.UI
{
    public partial class MainWindow : Window
    {
        // 1. GRAF NESNESİNİ BURADA TANIMLIYORUZ
        public Graph myGraph;

        public MainWindow()
        {
            InitializeComponent();

            // 2. Program açılınca boş bir graf oluşturuyoruz
            myGraph = new Graph();
        }

        // KAYDET BUTONU
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            FileManager fm = new FileManager();
            // Mevcut grafiği dosyaya gönder
            fm.SaveGraph(myGraph);
        }

        // YÜKLE BUTONU
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            FileManager fm = new FileManager();
            // Dosyadan gelen grafiği al
            Graph loadedGraph = fm.LoadGraph();

            if (loadedGraph != null)
            {
                // Hafızadaki grafiği yenisiyle değiştir
                myGraph = loadedGraph;

                // Kontrol etmek için ekrana mesaj basalım
                MessageBox.Show($"Başarıyla yüklendi! Toplam Düğüm: {myGraph.Nodes.Count}");

                // İLERİDE BURAYA ÇİZİMİ YENİLEME KODU GELECEK
            }
        }
    }
}