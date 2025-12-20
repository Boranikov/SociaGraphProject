# 🕸️ SociaGraph - Social Network Visualization & Analysis Tool

SociaGraph is a WPF-based desktop application developed to visualize, edit, and analyze social network graphs. It allows users to create nodes and edges interactively, perform centrality analysis, and apply graph coloring algorithms.

![Project Screenshot]()
## 🚀 Features

* **Interactive Graph Editor:**
    * **Add Nodes:** Create user nodes with random interaction scores.
    * **Connect Users:** Link nodes dynamically to define relationships.
    * **Drag & Drop:** Move nodes around the canvas with real-time edge updating.
    * **Delete:** Right-click to remove nodes and their connections.
* **Data Persistence:** Save and Load graph structures using JSON serialization.
* **Algorithmic Analysis:**
    * **Top 5 Influencers:** Calculates the most influential users based on Degree Centrality (Connection Count) and Interaction Scores.
    * **Graph Coloring:** Implements the **Welsh-Powell Algorithm** to assign colors to nodes, ensuring no two connected nodes share the same color.
* **User Interface:** Clean WPF Canvas implementation with mode switching (Editing vs. Moving).

## 🛠️ Technologies & Tools

* **Language:** C# (.NET Framework)
* **UI Framework:** WPF (Windows Presentation Foundation)
* **Data Format:** JSON (Newtonsoft.Json)
* **Concepts:** Graph Theory, OOP, File I/O, Event-Driven Programming.

## 🧮 Algorithms Used

### 1. Top 5 Influencers Analysis
This feature identifies the most popular users in the network. The ranking logic is:
1.  **Primary Sort:** Number of Connections (Neighbors).
2.  **Secondary Sort:** Interaction Score (Activity points generated upon creation).
3.  Returns the top 5 nodes.

### 2. Welsh-Powell Graph Coloring
To visualize distinct groups or separate connected users visually:
1.  Nodes are sorted by their degree (number of connections) in descending order.
2.  The algorithm iterates through the list, assigning the first available color that is not used by any neighbor.
3.  This ensures high visual clarity for complex networks.

## 🎮 How to Use

1.  **Create:** Click on empty space to add a "User".
2.  **Connect:** Click a node (turns Red), then click another node to connect them.
3.  **Move:** Check the **"Taşıma Modu"** box to drag nodes.
4.  **Analyze:** Click **"Analiz (Top 5)"** to see the leaderboard.
5.  **Color:** Click **"Renklendir"** to run the Welsh-Powell algorithm.
6.  **Save/Load:** Use the JSON buttons to backup your graph.

---

### 👤 Author
**[Göksel Bekdemir]** *Student / Developer*