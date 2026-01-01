using System.Collections.Generic;
using System.Drawing; // Color için (System.Drawing.Common gerekir)

namespace SocialNetworkApp
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Point Location { get; set; }

        // --- GÖRSELLEÞTÝRME RENGÝ ---
        public Color NodeColor { get; set; } = Color.White;

        // --- KOMÞULAR (Baðlantý Sayýsý artýk Neighbors.Count ile bulunacak) ---
        public List<Node> Neighbors { get; set; } = new List<Node>();

        // --- ÝSTERLERE GÖRE YENÝ ÖZELLÝKLER ---
        public double ActivityScore { get; set; } // Özellik I (Aktiflik)
        public int InteractionCount { get; set; } // Özellik II (Etkileþim Sayýsý) 
        // -------------------------------------

        public Node(int id, string name, Point location)
        {
            Id = id;
            Name = name;
            Location = location;

            // Varsayýlan deðerler
            ActivityScore = 0.5;
            InteractionCount = 10;
        }
    }
}