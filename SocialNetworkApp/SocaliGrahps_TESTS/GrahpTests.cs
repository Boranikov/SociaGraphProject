using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetworkApp;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace SocaliGrahps_TESTS
{
    [TestClass]
    public class GraphTests // İsim düzeltildi: Grahp -> Graph
    {
        private Graph graph;
        private Node NodeA;
        private Node NodeB;
        private Node NodeC;

        // Her Testten Önce Her şeyi sıfırlar
        [TestInitialize]
        public void Setup()
        {
            graph = new Graph();
            // Node constructor yapısı projenin son haline uygun
            NodeA = new Node(1, "A", new Point(0, 0));
            NodeB = new Node(2, "B", new Point(1, 1));
            NodeC = new Node(3, "C", new Point(1, 1));
        }

        [TestMethod]
        public void AddNodeTest()
        {
            // Eylem
            graph.AddNode(NodeA);

            // Doğrulama
            Assert.AreEqual(1, graph.Nodes.Count, "Nodes Listesine Eklenemedi");
        }

        [TestMethod]
        public void AddEdgeTest()
        {
            // Eylem
            graph.AddNode(NodeA);
            graph.AddNode(NodeB);
            graph.AddEdge(NodeA, NodeB);

            // Eklenen kenarı al
            var createdEdge = graph.Edges[0];

            // Doğrulama
            Assert.AreEqual(1, graph.Edges.Count, "Edge listeye eklenmedi");

            // Kenar yönü kontrolü
            Assert.AreEqual(NodeA, createdEdge.Source, "Source hatalı");
            Assert.AreEqual(NodeB, createdEdge.Target, "Target hatalı");

            // Graph sınıfı Neighbors listesini dolduruyor, bu yüzden Neighbors.Count kontrol edilmeli
            Assert.AreEqual(1, NodeA.Neighbors.Count, "NodeA'nın komşu sayısı artmadı");
            Assert.AreEqual(1, NodeB.Neighbors.Count, "NodeB'nin komşu sayısı artmadı");

            // Karşılıklı komşuluk kontrolü
            Assert.IsTrue(NodeA.Neighbors.Contains(NodeB), "NodeA'nın komşularında NodeB yok");
            Assert.IsTrue(NodeB.Neighbors.Contains(NodeA), "NodeB'nin komşularında NodeA yok");
        }

        [TestMethod]
        public void RemoveNodeTest()
        {
            // Eylem
            graph.AddNode(NodeA);
            graph.AddNode(NodeB);
            graph.AddEdge(NodeA, NodeB);

            // Silme işlemi
            graph.RemoveNode(NodeA);

            // Doğrulama
            Assert.AreEqual(1, graph.Nodes.Count, "Yanlış Sayıda Node Silindi"); // Sadece NodeB kalmalı
            Assert.IsFalse(graph.Nodes.Contains(NodeA), "NodeA Hala listede"); // NodeA silinmiş olmalı

            // NodeA silinince ona bağlı Edge de silinmeli
            Assert.AreEqual(0, graph.Edges.Count, "NodeA Silindi Ama Edge Hala duruyor");

            // NodeB'nin komşularından da NodeA silinmeli
            Assert.IsFalse(NodeB.Neighbors.Contains(NodeA), "NodeB hala NodeA'yı komşu sanıyor");
        }

        [TestMethod]
        public void RemoveEdgeTest()
        {
            // Eylem
            graph.AddNode(NodeA);
            graph.AddNode(NodeB);
            graph.AddEdge(NodeA, NodeB);

            var CreatedEdge = graph.Edges[0];
            graph.RemoveEdge(CreatedEdge);

            // Doğrulama
            Assert.AreEqual(2, graph.Nodes.Count, "Nodelar Silinmemeliydi");
            Assert.AreEqual(0, graph.Edges.Count, "Edge Silinmedi");

            // Bağlantı sayıları düşmeli
            Assert.AreEqual(0, NodeA.Neighbors.Count, "NodeA'nın bağlantı sayısı düşmedi");
            Assert.AreEqual(0, NodeB.Neighbors.Count, "NodeB'nin bağlantı sayısı düşmedi");

            // Komşuluk bitmeli
            Assert.IsFalse(NodeA.Neighbors.Contains(NodeB), "NodeA ve NodeB hala komşu");
        }
    }
}