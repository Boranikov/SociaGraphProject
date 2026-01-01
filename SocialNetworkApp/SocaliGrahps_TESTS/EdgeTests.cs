using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetworkApp;
using System.Drawing;
using System.Collections.Generic;

namespace SocaliGrahps_TESTS
{
    [TestClass]
    public class EdgeTests
    {
        [TestMethod]
        public void Constructor_ShouldSetPropertiesCorrectly() //Constructor doğru çalışıyor mu
        {
            // Hazırlık
            Node Node1 = new Node(1, "A", new Point(0, 0));
            Node Node2 = new Node(2, "B", new Point(0, 0));

            Node1.Activeness = 0.5;
            Node2.Activeness = 0.5;

            Node1.Interaction = 10;
            Node2.Interaction = 14;

            Node1.ConnectionCount = 5;
            Node2.ConnectionCount = 2;


            // Eylem
            Edge edge = new Edge(Node1, Node2);

            // Doğrulama
            Assert.AreEqual(6.00, edge.Weight, "Ağırlık Hesaplaması yanlış");

        }
        [TestMethod]
        public void Edge_Should_Return1_NodesIden() //Seçilen nodellar aynı ise edge olmamalı ver 1 döndürmeli
        {
            // Hazırlık
            Node Node1 = new Node(1, "A", new Point(0, 0));
            Node Node2 = new Node(1, "A", new Point(0, 0));

            Node1.Activeness = 0.5;
            Node2.Activeness = 0.5;

            Node1.Interaction = 10;
            Node2.Interaction = 10;

            Node1.ConnectionCount = 5;
            Node2.ConnectionCount = 5;

            // Eylem
            Edge edge = new Edge(Node1, Node2);

            // Doğrulama
            Assert.AreEqual(1.00, edge.Weight, "Aynı Node Seçildi");

        }
        [TestMethod]
        public void SourceandTarget() // Source ve Target elemanları doğru atanıyor mu
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
        public void Weight() //Ağırlık hesaplama doğru yapılıyor mu
        {
            Node Node1 = new Node(1, "A", new Point(0, 0));
            Node Node2 = new Node(2, "B", new Point(2, 2));

            Node1.Activeness = 1;
            Node2.Activeness = 2;

            Node1.Interaction = 3;
            Node2.Interaction = 5;

            Node1.ConnectionCount = 4;
            Node2.ConnectionCount = 6;

            // Eylem
            Edge edge = new Edge(Node2 , Node1);

            // Doğrulama
            Assert.AreEqual(edge.Weight, 4, "Ağırlık Yanlış Hesaplandı");
        }
    }
}
