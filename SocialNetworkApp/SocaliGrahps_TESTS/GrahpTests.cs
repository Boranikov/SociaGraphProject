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
            var createdEdge = graph.Edges[0];

            // Doğrulama
            
            Assert.AreEqual(1,graph.Edges.Count); //Edge listeye eklendi mi
            Assert.AreEqual(NodeB, createdEdge.Target,"A'nın komşularında B yok"); // Komşulama doğru mu
            Assert.AreEqual(NodeA, createdEdge.Source,"B'nin komşularında A yok"); // Komşulama doğru mu
            Assert.AreEqual(1, NodeA.ConnectionCount); // Node bağlantı sayısı arttı mı
            Assert.AreEqual(1, NodeB.ConnectionCount); // Node bağlantı sayısı arttı mı

        }
        [TestMethod]
        public void RemoveNodeTest()
        {
            // Eylem
            graph.AddNode(NodeA);
            graph.AddNode(NodeB);
            graph.AddEdge(NodeA, NodeB);
            graph.RemoveNode(NodeA);
            // Doğrulama
            Assert.AreEqual(1, graph.Nodes.Count, "Yanlış Sayıda Node Silindi"); // NodeB hala listede mi
            Assert.IsFalse(graph.Nodes.Contains(NodeA), "NodeA Hala listede"); // NodeA silindi mi
            Assert.AreEqual(0, graph.Edges.Count, "NodeA Silindi Ama Edge Hala duruyor");
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
            Assert.AreEqual(2, graph.Nodes.Count, "Nodelar Silindi"); // Nodelar hala duruyor mu
            Assert.AreEqual(0, graph.Edges.Count, "Edge Silinmedi");  // Edge silindi mi
            Assert.AreEqual(0, NodeA.ConnectionCount, "NodeA'nın bağlantı sayısı düşmedi"); // NodeA'da bağlantı var mı
            Assert.AreEqual(0, NodeB.ConnectionCount, "NodeB'nin bağlantı sayısı düşmedi"); // NodeB'de bağlantı var mı
            Assert.IsFalse(NodeA.Neighbors.Contains(NodeB), "NodeA ve NodeB hala komşu"); //NodeA ve NodeB hala komşu mu
        }
    }
}
