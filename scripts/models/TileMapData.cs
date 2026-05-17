using System.Collections.Generic;
using Godot;
using Vector2 = System.Numerics.Vector2;

public class TileMapData
{
    // Розміри тайлмапи
    public int Width { get; } = 100;
    public int Height { get; } = 100;

    // Дані для генерації шуму
    public float NoiseScale { get; } = 0.1f;

    // Ідентифікатор джерела тайлів місцевості та ресурс-станцій
    public int TerrainAtlasId { get; } = 0;
    public int ResourceAtlasId { get; } = 1;

    public List<int> TerrainLayers { get; } = new() { 0, 1, 2, 3, 5 };
    public int ResourceLayer { get; } = 4;

    // Позиції тайлів на атласі для рендерингу трави
    public List<Vector2I> RenderTilesGrass { get; } = new() {
        new (0, 0),
        new (1, 0),
        new (2, 0),
        new (3, 0),
        new (0, 1),
        new (1, 1),
        new (2, 1),
        new (3, 1),
        new (0, 2),
        new (1, 2),
        new (2, 2),
        new (3, 2),
        new (0, 3),
        new (1, 3),
        new (2, 3),
        new (3, 3)
    };

    public Vector2I TreeTile { get; } = new(4, 0);
    public Vector2I SelectTile { get; } = new(2, 4);

    // Позиції тайлів на атласі для рендерингу станцій
    public Dictionary<ResourceType, Vector2I> ResourceTiles { get; } = new()
    {
        { ResourceType.Ore, new(2, 0) },
        { ResourceType.Fuel, new(3, 0)},
        { ResourceType.Ingot, new(5, 0) },
        { ResourceType.People, new(1, 0) },
        { ResourceType.Money, new(6, 0) },
        { ResourceType.Food, new(4, 0) }
    };

    public Vector2I FoundationTile { get; } = new(0, 0);
    public Vector2I StartTile { get; } = new(7, 0);

    // Відповідність "тип - вектор зміщення до сусіднього тайлу"
    public Dictionary<TerrainMatch, Vector2I> TerrainVariants { get; } = new()
    {
        { TerrainMatch.left, new(-1, 0) },
        { TerrainMatch.right, new(1, 0) },
        { TerrainMatch.up, new(0, -1) },
        { TerrainMatch.down, new(0, 1) },
        { TerrainMatch.up_left, new(-1, -1) },
        { TerrainMatch.up_right, new(1, -1) },
        { TerrainMatch.down_left, new(-1, 1) },
        { TerrainMatch.down_right, new(1, 1) }
    };

    // Відповідність "тип - маска тайлу"
    public Dictionary<TerrainMatch, List<Vector2>> TerrainCorners { get; } = new()
    {
        { TerrainMatch.left, new List<Vector2> { new(0,0), new(1, 0)}},
        { TerrainMatch.right, new List<Vector2> { new(0,1), new(1, 1)}},
        { TerrainMatch.up, new List<Vector2> { new(0,0), new(0, 1)}},
        { TerrainMatch.down, new List<Vector2> { new(1,0), new(1, 1)}},
        { TerrainMatch.up_left, new List<Vector2> { new(0,0)}},
        { TerrainMatch.up_right, new List<Vector2> { new(0,1)}},
        { TerrainMatch.down_left, new List<Vector2> { new(1,0)}},
        { TerrainMatch.down_right, new List<Vector2> { new(1,1)}}
    };
    public TileMapData() { }
}

// Типи відповідності типу тайлу
public enum TerrainMatch
{
    left,
    right,
    up,
    down,
    up_left,
    up_right,
    down_left,
    down_right
}