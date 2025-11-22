using System;
using System.Collections.Generic;
using System.Drawing; 

namespace SocialNetworkApp
{
    // (Düğüm/Kullanıcı) Sınıfı
    public class Node
    {
        // Benzersiz kimlik
        public int Id { get; set; }

        // Kullanıcı Adı 
        public string Name { get; set; }

        // Ağırlık hesabı için gereken özellikler
        public double Activeness { get; set; }    // Özellik 1: Aktiflik
        public double Interaction { get; set; }   // Özellik 2: Etkileşim
        public int ConnectionCount { get; set; }  // Özellik 3: Bağlantı Sayısı (Bunu otomatik de hesaplatabiliriz)

        // Görsel özellikler (Ekranda nerede duracak?)
        public Point Location { get; set; }
        public Color Color { get; set; } = Color.Blue; // Varsayılan renk mavi

        // Komşuları tutacak liste
        public List<Node> Neighbors { get; set; }

        // Yapıcı Metot (Constructor)
        public Node(int id, string name, Point location)
        {
            Id = id;
            Name = name;
            Location = location;
            Neighbors = new List<Node>();

            // Varsayılan rastgele değerler atama test sırasında kolaylık
            Random rnd = new Random();
            Activeness = Math.Round(rnd.NextDouble(), 2); 
            Interaction = rnd.Next(1, 20); 
        }
    }
}