using Microsoft.Win32; // WPF dosya pencereleri için bu gereklidir
using Newtonsoft.Json;
using System.IO;
using System.Windows; // MessageBox için bu gereklidir
using SocialNetworkApp; // Arkadaşının Graph sınıfını görmek için

namespace SociaGraph.UI
{
    public class FileManager
    {
        // --- KAYDETME (SAVE) ---
        public void SaveGraph(Graph graph)
        {
            // WPF'de SaveFileDialog 
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Dosyası|*.json";
            saveFileDialog.Title = "Grafiği Kaydet";
            saveFileDialog.FileName = "GraphData.json";

            // WPF'de ShowDialog() sonucu "true/false" döner 
            if (saveFileDialog.ShowDialog() == true)
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Formatting = Formatting.Indented
                };

                string json = JsonConvert.SerializeObject(graph, settings);
                File.WriteAllText(saveFileDialog.FileName, json);

                // MessageBox kullanımı
                MessageBox.Show("Kayıt Başarılı!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // --- YÜKLEME (LOAD) ---
        public Graph LoadGraph()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Dosyası|*.json";
            openFileDialog.Title = "Grafik Yükle";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);

                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    };

                    Graph loadedGraph = JsonConvert.DeserializeObject<Graph>(json, settings);

                    MessageBox.Show("Grafik Yüklendi!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                    return loadedGraph;
                }
            }
            return null;
        }
    }
}