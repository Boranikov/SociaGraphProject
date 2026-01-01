# 🕸️ Sosyal Ağ Analizi Uygulaması (Social Network Analysis)

> **Ders:** Yazılım Geliştirme Laboratuvarı-I - Proje 2  
> **Tarih:** Ocak 2026  
> **Durum:** Tamamlandı ✅

## 📖 Proje Hakkında
Bu proje, kullanıcılar arasındaki ilişkileri bir graf yapısı olarak modelleyen, görselleştiren ve üzerinde çeşitli analiz algoritmaları çalıştıran bir masaüstü uygulamasıdır. Kullanıcılar (düğümler) ve etkileşimler (kenarlar) dinamik olarak yönetilebilir, JSON formatında kaydedilip tekrar yüklenebilir.

### 🎯 Amaç
- Sosyal ağ verilerini **Graf Teorisi** kullanarak modellemek.
- **BFS, DFS, Dijkstra, A\*** gibi temel algoritmaları gerçek hayat senaryosunda uygulamak.
- **Welsh-Powell** algoritması ile ağ renklendirmesi yapmak.
- Karmaşık verileri **WPF** ile görselleştirmek.

---

## 🏗️ Mimari Tasarım (Class Diagram)
Proje, Nesne Yönelimli Programlama (OOP) prensiplerine uygun olarak tasarlanmıştır. Aşağıdaki diyagramda sınıflar arası ilişkiler gösterilmektedir.

```mermaid
classDiagram
    class Node {
        +int Id
        +string Name
        +Point Location
        +Color NodeColor
        +List~Node~ Neighbors
        +double ActivityScore
        +int InteractionCount
    }

    class Edge {
        +Node Source
        +Node Target
        +double Weight
        +Color EdgeColor
        +int Thickness
        +CalculateWeight()
    }

    class Graph {
        +List~Node~ Nodes
        +List~Edge~ Edges
        +AddNode(Node)
        +AddEdge(Node, Node)
        +RemoveNode(Node)
        +RemoveEdge(Edge)
    }

    class GraphAlgorithms {
        +GetTopInfluencers(Graph, int)
        +WelshPowellColor(Graph)
        +Dijkstra_ShortestPath(Graph, Node, Node)
        +BFS_ShortestPath(Graph, Node, Node)
        +DFS_FindPath(Node, Node)
        +AStar_Path(Graph, Node, Node)
    }

    class FileManager {
        +SaveGraph(Graph, string)
        +LoadGraph(string)
    }

    Graph "1" *-- "*" Node : Contains
    Graph "1" *-- "*" Edge : Contains
    Edge "1" --> "2" Node : Connects
    GraphAlgorithms ..> Graph : Analyzes
    FileManager ..> Graph : Persists