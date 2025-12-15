using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetworkApp;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace SocaliGrahps_TESTS
{
    [TestClass]
    public class GrahpTests
    {
        private Graph graph;
        private Node NodeA;
        private Node NodeB;
        private Node NodeC;

        // Her Testen Önce Her şeyi sıfırlar
        [TestInitialize]
        public void Setup()
        {
            graph = new Graph();
            NodeA = new Node(1,"A",new Point(0,0));
            NodeB = new Node(2,"B",new Point(1,1));
            NodeC = new Node(3,"C",new Point(1,1));
        }
        [TestMethod]
        public void AddNodeTest()
        {
            // Eylem
            graph.AddNode(NodeA);

            // Doğrulama
            Assert.AreEqual(1,graph.Nodes.Count,"Edges Listesine Eklenemedi");

        }
        [TestMethod]
        public void AddEdgeTest()
        {
            // Eylem
            graph.AddNode(NodeA);
            graph.AddNode(NodeB);
            graph.AddEdge(NodeA, NodeB);

            // Doğrulama
            var createdEdge = graph.Edges[0];
            Assert.AreEqual(1,graph.Edges.Count); //Edge listeye eklendi mi
            Assert.AreEqual(NodeB, createdEdge.Target,"A'nın komşularında B yok"); // Komşulama doğru mu
            Assert.AreEqual(NodeA, createdEdge.Source,"B'nin komşularında A yok"); // Komşulama doğru mu
            Assert.AreEqual(1, NodeA.ConnectionCount); // Node bağlantı sayısı arttı mı
            Assert.AreEqual(1, NodeB.ConnectionCount); // Node bağlantı sayısı arttı mı

        }
    }
}
