using System;
using System.Collections.Generic;
using System.Drawing; 

namespace SocialNetworkApp
{
    // (Düðüm/Kullanýcý) Sýnýfý
    public class Node
    {
        public Color NodeColor { get; set; } = Color.White; // Varsayýlan renk beyaz olsun
        // Benzersiz kimlik
        public int Id { get; set; }

        // Kullanýcý Adý 
        public string Name { get; set; }

        // Aðýrlýk hesabý için gereken özellikler
        public double Activeness { get; set; }    // Özellik 1: Aktiflik
        public double Interaction { get; set; }   // Özellik 2: Etkileþim
        public int ConnectionCount { get; set; }  // Özellik 3: Baðlantý Sayýsý (Bunu otomatik de hesaplatabiliriz)

        // Görsel özellikler (Ekranda nerede duracak?)
        public Point Location { get; set; }

        // Komþularý tutacak liste
        public List<Node> Neighbors { get; set; }
        private static Random rnd = new Random();

        // Yapýcý Metot (Constructor)
        public Node(int id, string name, Point location)
        {
            Id = id;
            Name = name;
            Location = location;
            Neighbors = new List<Node>();

            // Varsayýlan rastgele deðerler atama test sýrasýnda kolaylýk
            Activeness = Math.Round(rnd.NextDouble(), 2); 
            Interaction = rnd.Next(1, 20); 
        }

    }
}