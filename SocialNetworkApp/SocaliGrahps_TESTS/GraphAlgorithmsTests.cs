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

        public void Setup()
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
        // ----------------------------------- BFS TEST -----------------------------------//
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
        // ----------------------------------- BFS TEST -----------------------------------//
        // ----------------------------------- DFS TEST -----------------------------------//
        [TestMethod]
        public void DFS_LinearPath()
        {
            // A -> B -> C yol
            graph.AddEdge(NodeA, NodeB);
            graph.AddEdge(NodeB, NodeC);

            // DFS algoritmasını çağır 
            var path = GraphAlgorithms.DFS_FindPath(NodeA, NodeC);

            // Doğrulama
            Assert.IsNotNull(path, "Yol bulunamadı.");
            Assert.AreEqual(3, path.Count, "Yol eksik veya yanlış.");
            Assert.AreEqual(NodeA, path[0]); // Başlangıç
            Assert.AreEqual(NodeC, path[2]); // Bitiş
        }
        [TestMethod]
        public void DFS_ShouldFindPath_EvenIfItIsNotShortest()
        {
            // Eylem
            graph.AddEdge(NodeA, NodeE);
            graph.AddEdge(NodeE, NodeD);

            graph.AddEdge(NodeA, NodeB);
            graph.AddEdge(NodeB, NodeC);
            graph.AddEdge(NodeC, NodeD);
            var path = GraphAlgorithms.DFS_FindPath(NodeA, NodeD);

            // Doğrulama

            Assert.IsNotNull(path);
            Assert.AreEqual(NodeA, path.First());
            Assert.AreEqual(NodeD, path.Last());
            //  path[0] ile path[1] komşu mu? path[1] ile path[2] komşu mu?
            for (int i = 0; i < path.Count - 1; i++)
            {
                Node current = path[i];
                Node next = path[i + 1];
                Assert.IsTrue(current.Neighbors.Contains(next), $"{current.Name} ve {next.Name} arasında bağlantı yok, DFS kopuk yol buldu!");
            }
        }
        // ----------------------------------- DFS TEST -----------------------------------//
        // ----------------------------------- Dijkstra TEST -----------------------------------//
        [TestMethod]
        public void Dijkstra_ShouldChoose_CheaperPath_Over_ShorterPath()
        {
            // Senaryo Kurulumu:

            // Yol 1: A -> C (Direkt ama çok PAHALI)
            graph.AddEdge(NodeA, NodeC);

            // Kenarı bulup ağırlığını elle 50 yapıyoruz
            var edgeAC = graph.Edges.First(e =>
                (e.Source == NodeA && e.Target == NodeC) ||
                (e.Source == NodeC && e.Target == NodeA));
            edgeAC.Weight = 50;

            // Yol 2: A -> B -> C (Uzun ama çok UCUZ)
            graph.AddEdge(NodeA, NodeB);
            graph.AddEdge(NodeB, NodeC);

            var edgeAB = graph.Edges.First(e => (e.Source == NodeA && e.Target == NodeB) || (e.Source == NodeB && e.Target == NodeA));
            var edgeBC = graph.Edges.First(e => (e.Source == NodeB && e.Target == NodeC) || (e.Source == NodeC && e.Target == NodeB));

            edgeAB.Weight = 5;
            edgeBC.Weight = 5;

            // Eylem: Algoritmayı çalıştır
            var path = GraphAlgorithms.Dijkstra_ShortestPath(graph, NodeA, NodeC);

            // Doğrulama
            Assert.IsNotNull(path, "Yol bulunamadı.");

            // Dijkstra çalıştığı için Count 3 olmalı (A, B, C).
            Assert.AreEqual(3, path.Count, "Dijkstra en ucuz yolu seçmedi! Pahalı olan kısa yoldan gitti.");

            // Ara durak B olmalı
            Assert.AreEqual(NodeB, path[1]);
        }

        // 2. TEST: Yol Yoksa
        [TestMethod]
        public void Dijkstra_ShouldReturnNull_WhenNoPathExists()
        {
            // A-B bağlı, C-D bağlı. Ama gruplar kopuk.
            graph.AddEdge(NodeA, NodeB);
            graph.AddEdge(NodeC, NodeD);

            var path = GraphAlgorithms.Dijkstra_ShortestPath(graph, NodeA, NodeD);

            Assert.IsNull(path, "Olmayan bir yol buldu.");
        }

        // 3. TEST: Daha Karmaşık Bir Ağda Doğru Maliyet Hesabı
        [TestMethod]
        public void Dijkstra_ShouldCalculate_ComplexPath_Correctly()
        {

            graph.AddEdge(NodeA, NodeB); // A-B
            graph.AddEdge(NodeB, NodeD); // B-D
            graph.AddEdge(NodeA, NodeC); // A-C
            graph.AddEdge(NodeB, NodeC); // B-C

            // Ağırlıkları atayalım
            SetEdgeWeight(graph, NodeA, NodeB, 1);
            SetEdgeWeight(graph, NodeB, NodeD, 1);
            SetEdgeWeight(graph, NodeA, NodeC, 10);
            SetEdgeWeight(graph, NodeB, NodeC, 5);

            var path = GraphAlgorithms.Dijkstra_ShortestPath(graph, NodeA, NodeD);

            Assert.IsNotNull(path);
            Assert.AreEqual(3, path.Count); // A -> B -> D
            Assert.AreEqual(NodeB, path[1]); // Ara durak B olmalı
        }

        // Yardımcı Metot: Ağırlık değiştirmeyi kolaylaştırmak için
        private void SetEdgeWeight(Graph g, Node n1, Node n2, double weight)
        {
            var edge = g.Edges.FirstOrDefault(e =>
                (e.Source == n1 && e.Target == n2) ||
                (e.Source == n2 && e.Target == n1));

            if (edge != null)
            {
                edge.Weight = weight;
            }
        }
        // ----------------------------------- Dijkstra TEST -----------------------------------//
        // ----------------------------------- A* TEST -----------------------------------//
        [TestMethod]
        public void AStar_ShouldFind_ShortestPath_BasedOnWeights()
        {
            // Senaryo: Geometrik olarak iki yol var ama biri ağırlık olarak daha ucuz.
            // Yol 1: A -> B -> End (Koordinat olarak dümdüz çizgi)
            graph.AddEdge(NodeA, NodeB); // Ağırlık varsayılan 1 olsun
            graph.AddEdge(NodeB, NodeE);

            // Yol 2: A -> C -> End (Yolu uzatıyor ama ağırlıkları manipüle edeceğiz)
            graph.AddEdge(NodeA, NodeC);
            graph.AddEdge(NodeC, NodeE);

            // Ağırlıkları ayarlayalım
            // Yol 1 (A-B-End) Toplam Ağırlık: 100 (Pahalı yol)
            SetEdgeWeight(NodeA, NodeB, 50);
            SetEdgeWeight(NodeB, NodeE, 50);

            // Yol 2 (A-C-End) Toplam Ağırlık: 10 (Ucuz yol)
            SetEdgeWeight(NodeA, NodeC, 5);
            SetEdgeWeight(NodeC, NodeE, 5);

            // Eylem
            var path = GraphAlgorithms.AStar_Path(graph, NodeA, NodeE);

            // Doğrulama
            Assert.IsNotNull(path);
            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(NodeC, path[1], "A* daha düşük maliyetli (C üzerinden) yolu seçmeliydi.");
        }

        [TestMethod]
        public void AStar_ShouldReturnNull_WhenNoPathExists()
        {
            // A-B bağlı, C-End bağlı ama iki grup kopuk
            graph.AddEdge(NodeA, NodeB);
            graph.AddEdge(NodeC, NodeE);

            var path = GraphAlgorithms.AStar_Path(graph, NodeA, NodeE);

            Assert.IsNull(path, "Yol olmadığı halde bir sonuç döndü.");
        }

        [TestMethod]
        public void AStar_DirectPath_Test()
        {
            // Direkt bağlantı testi
            graph.AddEdge(NodeA, NodeE);
            SetEdgeWeight(NodeA, NodeE, 1);

            var path = GraphAlgorithms.AStar_Path(graph, NodeA, NodeE);

            Assert.AreEqual(2, path.Count);
            Assert.AreEqual(NodeA, path[0]);
            Assert.AreEqual(NodeE, path[1]);
        }

        // Yardımcı metot
        private void SetEdgeWeight(Node n1, Node n2, double weight)
        {
            var edge = graph.Edges.FirstOrDefault(e =>
                (e.Source == n1 && e.Target == n2) ||
                (e.Source == n2 && e.Target == n1));

            if (edge != null) edge.Weight = weight;
        }
    }
}


