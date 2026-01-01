using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json; // Kayıt için

namespace SocialNetworkApp
{
    // Kaydetmek için ara sınıflar
    public class GraphData
    {
        public List<NodeData> Nodes { get; set; } = new List<NodeData>();
        public List<EdgeData> Edges { get; set; } = new List<EdgeData>();
    }

    public class NodeData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double ActivityScore { get; set; }
        public int InteractionCount { get; set; }
    }

    public class EdgeData
    {
        public int SourceId { get; set; }
        public int TargetId { get; set; }
    }

    public static class FileManager
    {
        // KAYDETME FONKSİYONU
        public static void SaveGraph(Graph graph, string filePath)
        {
            var data = new GraphData();

            foreach (var node in graph.Nodes)
            {
                data.Nodes.Add(new NodeData
                {
                    Id = node.Id,
                    Name = node.Name,
                    X = node.Location.X,
                    Y = node.Location.Y,
                    ActivityScore = node.ActivityScore,
                    InteractionCount = node.InteractionCount
                });
            }

            foreach (var edge in graph.Edges)
            {
                data.Edges.Add(new EdgeData
                {
                    SourceId = edge.Source.Id,
                    TargetId = edge.Target.Id
                });
            }

            // --- DEĞİŞEN KISIM (Newtonsoft) ---
            // Formatting.Indented sayesinde JSON dosyası okunaklı kaydedilir.
            string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, jsonString);
        }

        // YÜKLEME FONKSİYONU
        public static Graph LoadGraph(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            string jsonString = File.ReadAllText(filePath);

            var data = JsonConvert.DeserializeObject<GraphData>(jsonString);

            if (data == null) return null;

            Graph newGraph = new Graph();
            var nodeDict = new Dictionary<int, Node>();

            foreach (var nData in data.Nodes)
            {
                var location = new System.Drawing.Point((int)nData.X, (int)nData.Y);
                Node newNode = new Node(nData.Id, nData.Name, location);

                newNode.ActivityScore = nData.ActivityScore;
                newNode.InteractionCount = nData.InteractionCount;
                newNode.NodeColor = System.Drawing.Color.Blue;

                newGraph.Nodes.Add(newNode);
                nodeDict[nData.Id] = newNode;
            }

            foreach (var eData in data.Edges)
            {
                if (nodeDict.ContainsKey(eData.SourceId) && nodeDict.ContainsKey(eData.TargetId))
                {
                    Node source = nodeDict[eData.SourceId];
                    Node target = nodeDict[eData.TargetId];

                    Edge newEdge = new Edge(source, target);
                    newGraph.Edges.Add(newEdge);

                    source.Neighbors.Add(target);
                    target.Neighbors.Add(source);
                }
            }

            return newGraph;
        }
    }
}