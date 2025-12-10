using System;
using System.Drawing;

namespace SocialNetworkApp
{
    // Ýki kullanýcý arasýndaki baðlantý (Edge)
    public class Edge
    {
        public Node Source { get; set; } // Kaynak Düðüm (Kimden?)
        public Node Target { get; set; } // Hedef Düðüm (Kime?)
        public double Weight { get; set; } // Aðýrlýk (Maliyet)

        // Yapýcý Metot
        public Edge(Node source, Node target)
        {
            Source = source;
            Target = target;

            // Aðýrlýðý otomatik hesapla
            Weight = CalculateWeight();
        }

        // formüle göre aðýrlýk hesaplayan metot
        private double CalculateWeight()
        {
            // 1. Aktiflik farkýnýn karesi
            double activenessDiff = Math.Pow(Source.Activeness - Target.Activeness, 2);

            // 2. Etkileþim farkýnýn karesi
            double interactionDiff = Math.Pow(Source.Interaction - Target.Interaction, 2);

            // 3. Baðlantý sayýsý farkýnýn karesi
            double connectionDiff = Math.Pow(Source.ConnectionCount - Target.ConnectionCount, 2);

            // Formülün uygulanmasý
            double result = 1 + Math.Sqrt(activenessDiff + interactionDiff + connectionDiff);

            // Sonucu yuvarla
            return Math.Round(result, 2);
        }
    }
}