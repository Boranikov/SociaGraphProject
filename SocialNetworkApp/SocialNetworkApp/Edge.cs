using System;
using System.Drawing;

namespace SocialNetworkApp
{
    public class Edge
    {
        public Node Source { get; set; }
        public Node Target { get; set; }
        public double Weight { get; set; } // Formülden çýkan sonuç

        // Görselleþtirme
        public Color EdgeColor { get; set; } = Color.Gray;
        public int Thickness { get; set; } = 2;

        public Edge(Node source, Node target)
        {
            Source = source;
            Target = target;
            // Aðýrlýðý formüle göre hesapla
            Weight = CalculateWeight();
        }

        // --- DÝNAMÝK AÐIRLIK FORMÜLÜ ---
        public double CalculateWeight()
        {
            // 1. Özellik Farklarý (Mutlak Deðer)
            double diffActivity = Math.Abs(Source.ActivityScore - Target.ActivityScore);
            double diffInteraction = Math.Abs(Source.InteractionCount - Target.InteractionCount);

            // Baðlantý sayýsý o anki komþu sayýsýdýr
            double diffConnection = Math.Abs(Source.Neighbors.Count - Target.Neighbors.Count);

            // 2. Formülün Paydasý
            // Payda = (1 + FarkAktif) * (2 + FarkEtkilesim) * (2 + FarkBaglanti)^2
            double denominator = (1 + diffActivity) * (2 + diffInteraction) * Math.Pow((2 + diffConnection), 2);

            double result = 1.0 / denominator;

            return result;
        }
    }
}