using System;

namespace SociaGraph.UI  // <-- Buranın MainWindow ile aynı olduğuna emin ol
{
    public class Node
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Activity { get; set; }
        public double Interaction { get; set; }
        public double ConnectionCount { get; set; }
    }
}