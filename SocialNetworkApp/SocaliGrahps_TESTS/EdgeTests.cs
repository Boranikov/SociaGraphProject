using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetworkApp;
using System.Drawing;
using System.Collections.Generic;

namespace SocaliGrahps_TESTS
{
    [TestClass]
    public class EdgeTests
    {
        // Yardımcı Metot: Komşu sayısını simüle etmek için listeye sahte node ekler
        private void AddDummyNeighbors(Node node, int count)
        {
            if (node.Neighbors == null) node.Neighbors = new List<Node>();

            for (int i = 0; i < count; i++)
            {
                node.Neighbors.Add(new Node(999 + i, "Dummy", new Point(0, 0)));
            }
        }

        [TestMethod]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Hazırlık
            Node Node1 = new Node(1, "A", new Point(0, 0));
            Node Node2 = new Node(2, "B", new Point(0, 0));

            // İsimler güncellendi
            Node1.ActivityScore = 0.5;
            Node2.ActivityScore = 0.5; // Fark: 0

            Node1.InteractionCount = 10;
            Node2.InteractionCount = 14; // Fark: 4

            // Komşu sayılarını ayarlamak için listeye eleman ekliyoruz
            AddDummyNeighbors(Node1, 5);
            AddDummyNeighbors(Node2, 2); // Fark: 3

            // Eylem
            Edge edge = new Edge(Node1, Node2);

            // Doğrulama (Double karşılaştırmada 0.0001 sapma payı veriyoruz)
            Assert.AreEqual(0.006666, edge.Weight, 0.0001, "Ağırlık Hesaplaması yanlış");
        }

        [TestMethod]
        public void Edge_Should_Check_Identical_Nodes()
        {
            // Hazırlık
            Node Node1 = new Node(1, "A", new Point(0, 0));
            Node Node2 = new Node(1, "A", new Point(0, 0));

            Node1.ActivityScore = 0.5;
            Node2.ActivityScore = 0.5; // Fark: 0

            Node1.InteractionCount = 10;
            Node2.InteractionCount = 10; // Fark: 0

            AddDummyNeighbors(Node1, 5);
            AddDummyNeighbors(Node2, 5); // Fark: 0

            // Eylem
            Edge edge = new Edge(Node1, Node2);

            // Doğrulama
            Assert.AreEqual(0.125, edge.Weight, 0.0001, "Aynı Node Seçildiğinde formül hatalı");
        }

        [TestMethod]
        public void SourceandTarget()
        {
            // Hazırlık
            Node Node1 = new Node(1, "A", new Point(0, 0));
            Node Node2 = new Node(2, "B", new Point(10, 10));

            // Eylem
            Edge edge = new Edge(Node1, Node2);

            // Doğrulama
            Assert.AreEqual(Node1, edge.Source);
            Assert.AreEqual(Node2, edge.Target);
        }

        [TestMethod]
        public void Weight_Calculation_Test()
        {
            Node Node1 = new Node(1, "A", new Point(0, 0));
            Node Node2 = new Node(2, "B", new Point(2, 2));

            Node1.ActivityScore = 1;
            Node2.ActivityScore = 2; // Fark: 1

            Node1.InteractionCount = 3;
            Node2.InteractionCount = 5; // Fark: 2

            AddDummyNeighbors(Node1, 4);
            AddDummyNeighbors(Node2, 6); // Fark: 2

            // Eylem
            Edge edge = new Edge(Node2, Node1);

            // Doğrulama
            Assert.AreEqual(0.0078125, edge.Weight, 0.00001, "Ağırlık Yanlış Hesaplandı");
        }
    }
}