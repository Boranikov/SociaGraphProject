using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetworkApp;
using System.Drawing;
using System.Collections.Generic;

namespace SocialNetworkApp.Tests
{
    [TestClass]
    public class NodeTests
    {
        // 1. Test: Constructor değerleri doğru atıyor mu?
        [TestMethod]
        public void Constructor_ShouldSetPropertiesCorrectly()
        {
            // Hazırlık
            int expectedId = 1;
            string expectedName = "Boran";
            Point expectedLocation = new Point(10, 20);

            // Eylem
            Node node = new Node(expectedId, expectedName, expectedLocation);

            // Doğrulama
            Assert.AreEqual(expectedId, node.Id);
            Assert.AreEqual(expectedName, node.Name);
            Assert.AreEqual(expectedLocation, node.Location);
        }

        // 2. Test: Neighbors listesi null olmamalı
        [TestMethod]
        public void Constructor_ShouldInitializeNeighborsList()
        {
            Node node = new Node(1, "TestUser", new Point(0, 0));

            Assert.IsNotNull(node.Neighbors);
            Assert.AreEqual(0, node.Neighbors.Count);
        }

        // 3. Test: Komşu eklenebiliyor mu?
        [TestMethod]
        public void CanAddNeighborToNode()
        {
            Node node1 = new Node(1, "User1", new Point(0, 0));
            Node node2 = new Node(2, "User2", new Point(5, 5));

            node1.Neighbors.Add(node2);

            Assert.AreEqual(1, node1.Neighbors.Count);
            // MSTest'te liste kontrolü şöyledir:
            CollectionAssert.Contains(node1.Neighbors, node2);
        }
    }
}