using System;

namespace SocialNetworkApp
{
    // İki kullanıcı arasındaki bağlantı (Edge)
    public class Edge
    {
        public Node Source { get; set; } // Kaynak Düğüm (Kimden?)
        public Node Target { get; set; } // Hedef Düğüm (Kime?)
        public double Weight { get; set; } // Ağırlık (Maliyet)

        // Yapıcı Metot
        public Edge(Node source, Node target)
        {
            Source = source;
            Target = target;

            // Ağırlığı otomatik hesapla
            Weight = CalculateWeight();
        }

        // formüle göre ağırlık hesaplayan metot
        private double CalculateWeight()
        {
            // 1. Aktiflik farkının karesi
            double activenessDiff = Math.Pow(Source.Activeness - Target.Activeness, 2);

            // 2. Etkileşim farkının karesi
            double interactionDiff = Math.Pow(Source.Interaction - Target.Interaction, 2);

            // 3. Bağlantı sayısı farkının karesi
            double connectionDiff = Math.Pow(Source.ConnectionCount - Target.ConnectionCount, 2);

            // Formülün uygulanması
            double result = 1 + Math.Sqrt(activenessDiff + interactionDiff + connectionDiff);

            // Sonucu yuvarla
            return Math.Round(result, 2);
        }
    }
}