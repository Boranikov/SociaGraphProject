using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetworkApp;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace SocaliGrahps_TESTS
{
    [TestClass]
    public class GraphAlgorithmsTests
    {
        private Graph graph;
        private Node NodeA, NodeB, NodeC, NodeD, NodeE;

        [TestInitialize]

        public void Setuo()
        {
            graph = new Graph();
            NodeA = new Node(1, "A", new Point(0, 0));
            NodeB = new Node(2, "B", new Point(1, 1));
            NodeC = new Node(3, "C", new Point(2, 2));
            NodeD = new Node(4, "D", new Point(3, 3));
            NodeE = new Node(5, "E", new Point(4, 4));

            graph.AddNode(NodeA);
            graph.AddNode(NodeB);
            graph.AddNode(NodeC);
            graph.AddNode(NodeD);
            graph.AddNode(NodeE);
        }

        [TestMethod]
        public void BFS_DirectNeighbors_FindPath()
        {
            // Eylem
            graph.AddEdge(NodeA, NodeB);
            var path = GraphAlgorithms.BFS_ShortestPath(graph,NodeA,NodeB);

            // Doğrulama
            Assert.IsNotNull(path,"Yol Bulunmadı Null Döndü");
            Assert.AreEqual(2, path.Count, "Yol uzunluğu yanlış (A ve B olmalı).");
            Assert.AreEqual(NodeA, path[0], "Başlangıç düğümü yanlış.");
            Assert.AreEqual(NodeB, path[1], "Bitiş düğümü yanlış.");
        }
        [TestMethod]
        public void BFS_LongOrShortPathTest()
        {
            // Eylem

            // Uzun yol yapımı A - B - C - E
            graph.AddEdge(NodeA, NodeB);
            graph.AddEdge(NodeB, NodeC);
            graph.AddEdge(NodeC, NodeE);

            // Kısa yol yapımı A - B - E
            graph.AddEdge(NodeB,NodeE);

            // Algoritma Çağırma
            var path = GraphAlgorithms.BFS_ShortestPath(graph, NodeA, NodeE);

            // Doğrulama
            Assert.IsNotNull(path, "Yol Bulunmadı Null Döndü");
            Assert.AreEqual(3, path.Count, "Doğru yolu seçmedi kısa yolu seçmeliydi");
            Assert.AreEqual(NodeA, path[0]); // İlk düğüm A
            Assert.AreEqual(NodeB, path[1]); // İkinci düğüm B
            Assert.AreEqual(NodeE, path[2]); // Üçüncü düğüm E
        }
    }
}
