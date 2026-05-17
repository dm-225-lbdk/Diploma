using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Godot;

namespace CityGame.scripts.Models.Resource;

public class ResourceGraph {
    private ResourсesExtensions Properties = new();
    public Dictionary<Vector2I, HashSet<Vector2I>> Graph { get; set; }

    public ResourceGraph() 
    {
        Graph = new();
    }

    // Можливість розміщення станції
    public bool CanPlaceStation(Vector2I node, Dictionary<ResourceType, ResourceTileInfo> resourceTilesData)
    {
        // Якщо на графі із станцій на цій поизції можна знайти сусідів, або це перша станція - це означає, що станція може бути розміщена
        return resourceTilesData.Values.Any(info => info.Stantions.Any(s => Graph[node].Contains(s.Position))) || resourceTilesData.All(info => info.Value.Stantions.Count == 0);
    }

    // Можливість видалення станції
    public bool CanDeleteStation(Vector2I node, Dictionary<ResourceType, ResourceTileInfo> resourceTilesData)
    {
        // Беремо списки наявних сусідів по графу і станцій-сусідів станції за позицією і якщо друге не дорівнює сусідам по графу, то видалення не можливе
        HashSet<Vector2I> neighborsStations = resourceTilesData.Values
            .SelectMany(info => info.Stantions)
            .Select(s => s.Position)
            .Where(pos => Graph[node].Contains(pos))
            .ToHashSet();

        HashSet<Vector2I> neighborsGraph = Graph[node];
        return neighborsStations.Count != neighborsGraph.Count;
    }

    // Пошук станції, яка може забезпечити новий сток-ресурс
    public Vector2I FoundNearestStockConsume(ResourсeStation station, ResourceType type, Dictionary<ResourceType, ResourceTileInfo> resourceTilesData) 
    {

        List<Vector2I> StockConsumeStations = resourceTilesData[Properties.GetProperties(type).StockConsumeType].Stantions.Select(s => s.Position).ToList();

        // Якщо немає сток-станцій, то даємо нульову позицію, якщо є одна - повертаємо її, якщо більше - шукаємо найближчу
        if (StockConsumeStations.Count == 0) return Vector2I.Zero;
        if (StockConsumeStations.Count == 1) return StockConsumeStations[0];
        
        // Знаходимо найближчу сток-станцію до нової станції
        Vector2I nearestStation = StockConsumeStations[0];
        int prevDistance = (station.Position - StockConsumeStations[0]).LengthSquared();

        // Проходимо по всіх сток-станціях і шукаємо найближчу
        foreach (Vector2I StockStation in StockConsumeStations) 
        {
            int distance = (station.Position - StockStation).LengthSquared();
            if (distance < prevDistance)
            {
                prevDistance = distance;
                nearestStation = StockStation;
            }
        }
        
        return nearestStation;
    }

    // Створення вузла у графі
    public void CreateNode(Vector2I node) 
    {
        Graph[node] = new HashSet<Vector2I>();
    }

    // Видалення вузла з графа
    public void RemoveNode(Vector2I node) 
    {
        // Видалити всі ребра, пов'язані з вузлом
        if (!Graph.ContainsKey(node)) return;
        List<Vector2I> neighbors = Graph[node].ToList();
        
        foreach (var neighbor in neighbors)
        {
            if (Graph.ContainsKey(neighbor)) Graph[neighbor].Remove(node);
        }
        Graph.Remove(node);
    }

    // Створення ребра у графі
    public void CreateEdge(Vector2I from, Vector2I to) 
    {
        // Додати ребро в обох напрямках
        Graph[to].Add(from);
        Graph[from].Add(to);
    }

    // Будування графа
    public void BuildGraph()
    {
        List<Vector2I> toRemove = new();
        foreach (var start in Graph.Keys)
        {
            foreach (var end in Graph.Keys)
            {
                if (start != end)
                {
                    int lenght = (start - end).LengthSquared();
                    
                    if (lenght <= 2) 
                    {
                        toRemove.Add(start);
                    }
                    if (lenght > 4 && lenght <= 36) 
                    {
                        CreateEdge(start, end);
                    }
                }
            }
        }

        foreach (var node in toRemove) RemoveNode(node);

        ConnectNodes();

        ClearSmallClusters();
    }

    // Рекурсивна функція для з'єднання розрізнених вузлів у графі
    private void ConnectNodes() 
    {
        List<Vector2I> lonelyNodes = Graph.Keys.Where(node => Graph[node].Count <= 1).ToList();

        Vector2I StartVector = lonelyNodes.FirstOrDefault();
        
        // Отримуємо всі вузли, які вже пов'язані з стартовою позицією
        HashSet<Vector2I> visited = (Graph.Count > 0 && Graph.ContainsKey(StartVector)) 
            ? GetAllConnectedNodes(StartVector) 
            : new HashSet<Vector2I>();

        // Для кожного вузла, який має менше 3 сусідів, шукаємо найближчий вузол, який не належить тому ж кластеру, і створюємо ребро між ними
        if (lonelyNodes.Count <= 2) return;

        foreach (var start in lonelyNodes) 
        {
            HashSet<Vector2I> claster = GetAllConnectedNodes(start);
            int minDistance = int.MaxValue;
            Vector2I nearestNode = start;
            foreach (var end in Graph.Keys)
            {
                if (start == end) continue;
                int distance = (end - start).LengthSquared();
                if (distance < minDistance && (claster.Contains(end) == false))
                {
                    minDistance = distance;
                    nearestNode = end;
                }
            }
            // Якщо найближчий вузол знаходиться на відстані більше 500 одиниць, то шукаємо найближчий вузол в тому ж кластері і з'єднуємо їх
            if (minDistance > 500) nearestNode = ConnectWithNodeInClaster(claster, start);
            if (nearestNode != Vector2I.Zero) CreateEdge(start, nearestNode);
        }
        ConnectNodes();
    }

    // Функція для видалення кластерів, які містять менше 3 вузлів
    private void ClearSmallClusters() 
    {
        HashSet<Vector2I> visited = new();
        List<HashSet<Vector2I>> clusters = new();

        foreach (var node in Graph.Keys)
        {
            if (!visited.Contains(node))
            {
                HashSet<Vector2I> cluster = GetAllConnectedNodes(node, visited);
                clusters.Add(cluster);
            }
        }
        if (clusters.Count == 1) return;
        clusters.Remove(clusters.OrderByDescending(c => c.Count).ToList()[0]);
        List<Vector2I> toRemove = new();
        foreach (var cluster in clusters)
        {
            foreach (var node in cluster) toRemove.Add(node);
        }
        foreach (var node in toRemove) RemoveNode(node);
    }

    // Рекурсивна функція для отримання всіх вузлів, пов'язаних з даним вузлом (кластера)
    private HashSet<Vector2I> GetAllConnectedNodes(Vector2I start, HashSet<Vector2I> visited = null)
    {
        visited ??= new HashSet<Vector2I>();
        visited.Add(start);
        HashSet<Vector2I> claster = new() { start };

        foreach (var neighbor in Graph[start])
        {
            if (!visited.Contains(neighbor))
            {
                claster.UnionWith(GetAllConnectedNodes(neighbor, visited));
            }
        }
        return claster;
    }

    // Функція для з'єднання вузла з найближчим вузлом в тому ж кластері
    private Vector2I ConnectWithNodeInClaster(HashSet<Vector2I> claster, Vector2I node)
    {
        int minDistance = int.MaxValue;
        Vector2I nearestNode = node;
        foreach (var end in claster)
        {
            int distance = (end - node).LengthSquared();
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestNode = end;
            }
        }
        return nearestNode;
    }

    // Серіалізація в словник Godot
    public Godot.Collections.Dictionary ToGodotDictionary() 
    {
        Godot.Collections.Dictionary dict = new();

        foreach (var kvp in Graph)
        {
            Godot.Collections.Array neighborsArray = new();
            foreach (var neighbor in kvp.Value)
            {
                neighborsArray.Add(new Godot.Collections.Array { neighbor.X, neighbor.Y });
            }
            dict[new Godot.Collections.Array { kvp.Key.X, kvp.Key.Y }] = neighborsArray;
        }
        return dict;
    }

    // Десеріалізація зі словника Godot
    public static ResourceGraph FromGodotDictionary(Godot.Collections.Dictionary dict) 
    {
        ResourceGraph resourceGraph = new();
        foreach (var kvp in dict)
        {
            // Ключ може бути рядком типу "[-50, -29]" через JSON
            Vector2I key;
            if (kvp.Key.VariantType == Variant.Type.String)
            {
                string keyStr = kvp.Key.AsString();
                keyStr = keyStr.Trim('[', ']', ' ');
                var parts = keyStr.Split(',');
                key = new Vector2I(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()));
            }
            else
            {
                Godot.Collections.Array keyArr = kvp.Key.AsGodotArray();
                key = new(keyArr[0].AsInt32(), keyArr[1].AsInt32());
            }

            HashSet<Vector2I> neighbors = new();
            Godot.Collections.Array neighborsArray = kvp.Value.AsGodotArray();
            foreach (var neighbor in neighborsArray)
            {
                Godot.Collections.Array neighborArr = neighbor.AsGodotArray();
                neighbors.Add(new Vector2I(neighborArr[0].AsInt32(), neighborArr[1].AsInt32()));
            }
            resourceGraph.Graph[key] = neighbors;
        }
        return resourceGraph;
    }
}