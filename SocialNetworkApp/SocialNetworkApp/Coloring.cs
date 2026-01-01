using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SocialNetworkApp
{
    public class Coloring // Komşu olan node lar farklı renk ile boyanıyor
    {
        // 2. WELSH-POWELL İLE RENKLENDİRME
        public static void WelshPowellColor(Graph graph)
        {
            var sortedNodes = graph.Nodes.OrderByDescending(n => n.Neighbors.Count).ToList();
            var colors = new List<System.Drawing.Color>
            {
                System.Drawing.Color.Red, System.Drawing.Color.Blue, System.Drawing.Color.Green,
                System.Drawing.Color.Yellow, System.Drawing.Color.Orange, System.Drawing.Color.Purple,
                System.Drawing.Color.Cyan, System.Drawing.Color.Magenta, System.Drawing.Color.Brown,
                System.Drawing.Color.Pink, System.Drawing.Color.Lime, System.Drawing.Color.Teal
            }; 

            int colorIndex = 0;
            while (sortedNodes.Count > 0)
            {
                var currentColor = colors[colorIndex % colors.Count];
                var coloredInThisRound = new List<Node>();

                var firstNode = sortedNodes[0];
                firstNode.NodeColor = currentColor;
                coloredInThisRound.Add(firstNode);
                sortedNodes.RemoveAt(0);

                for (int i = 0; i < sortedNodes.Count; i++)
                {
                    var node = sortedNodes[i];
                    bool isNeighbor = false;
                    foreach (var coloredNode in coloredInThisRound)
                    {
                        if (node.Neighbors.Contains(coloredNode))
                        {
                            isNeighbor = true;
                            break;
                        }
                    }

                    if (!isNeighbor)
                    {
                        node.NodeColor = currentColor;
                        coloredInThisRound.Add(node);
                        sortedNodes.RemoveAt(i);
                        i--;
                    }
                }
                colorIndex++;
            }
        }
    }
}
