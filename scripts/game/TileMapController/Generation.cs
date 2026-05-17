using Godot;
using System;
using System.Collections.Generic;
using CityGame.scripts.Models.Resource;
public partial class Generation : Node
{
    TileMapGame tileMapGame;
    TileMapData tileMapData;
    TileData tileData;
    private float noiseScale = 0.1f;
    private float threshold = 0.5f;
    private Random random = new();
    public override void _Ready()
    {

        // Отримуємо посилання ноду TileMap на TileMapGame та TileMapData
        tileMapGame = GetTree()
                .Root
                .GetNode<Node>("game")
                .GetNode<TileMapGame>("TileMapGame");
        tileMapData = tileMapGame.Data;
        tileData = GlobalManager.Instance.TileData;

        // Очищуємо TileMap перед рендерингом
        tileMapGame.Clear();

        // Перевіряємо чи є дані у tileData (з файлу)
        if (tileData.TerrainsLayers[1].TerrainTilesData.Count == 0) GenerateTerrain();
        RenderLayers();

    }

    // Метод генерації місцевості та даних
    private void GenerateTerrain()
    {
        // Створення шуму
        FastNoiseLite noise = new()
        {
            NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
            Frequency = tileMapData.NoiseScale
        };

        // Генерація тайлів місцевості на основі шуму
        for (int y = -tileMapGame.Data.Height/2; y < tileMapGame.Data.Height/2; y++)
        {
            for (int x = -tileMapGame.Data.Width/2; x < tileMapGame.Data.Width/2; x++)
            {
                bool isTreeZone = false;
                // Значення шуму від 0 до 1
                float value = (noise.GetNoise2D(x, y) + 1f) / 2f;

                // Генерація тайлів місцневсоті

                // Генерація дерев
                if (value > 0.55f && random.NextDouble() < 0.2)
                {
                    tileData.GetTerrainTilesData(3).AddTerrainTile(new Vector2I(x, y));
                    isTreeZone = true;
                }

                // Генерація ресурсів (50% шанс, якщо це не зона дерев)
                if (value > 0.5f && random.NextDouble() < 0.4 && !isTreeZone)
                {
                    tileData.GetResourceTilesData(4).ResourceGraph.CreateNode(new Vector2I(x, y));
                }

                // Генерація трави
                if (value > 0.5f)
                {
                    tileData.GetTerrainTilesData(1).AddTerrainTile(new Vector2I(x, y));
                }

                // Генерація землі
                else
                {
                    tileData.GetTerrainTilesData(0).AddTerrainTile(new Vector2I(x, y));
                }
            }
        }

        tileData.GetResourceTilesData(4).BuildGraph();
        tileData.GetResourceTilesData(4).GiveFirstResource();
    }

    
    // Рендер усіх шарів
    private void RenderLayers()
    {
        // Рендер шарів місцевості
        foreach (int terrainLayer in tileData.TerrainsLayers.Keys)
        {
            foreach (Vector2I tile in tileData.TerrainsLayers[terrainLayer].TerrainTilesData)
            {
                Vector2I atlasTileCoords = terrainLayer switch
                {
                    // Земля
                    0 => tileMapData.RenderTilesGrass[0],
                    // Трава
                    1 => tileMapData.RenderTilesGrass[15],
                    // Дерева
                    3 => tileMapData.TreeTile,
                    // За замовчуванням (порожній тайл)
                    _ => new Vector2I(-1, -1)
                };

                tileMapGame.SetCell(terrainLayer, tile, tileMapData.TerrainAtlasId, atlasTileCoords);
            }
        }

        // Рендер шарів ресурс-станцій
        int resourceLayer = tileMapData.ResourceLayer;
        
        // Спочатку рендер графу ресурсів
        foreach (Vector2I node in tileData.ResorsesLayers[resourceLayer].ResourceGraph.Graph.Keys)
        {
            tileMapGame.SetCell(resourceLayer, node, tileMapData.ResourceAtlasId, tileMapData.FoundationTile);
            
            foreach (Vector2I neighbor in tileData.ResorsesLayers[resourceLayer].ResourceGraph.Graph[node])
            {                    
                Line2D line = new()
                {
                    Width = 4,
                    DefaultColor = Color.Color8(53, 38, 10),
                    ZIndex = 1
                };
                line.AddPoint(tileMapGame.MapToLocal(node));
                line.AddPoint(tileMapGame.MapToLocal(neighbor));
                tileMapGame.CallDeferred(Node.MethodName.AddChild, line);
            }
        }

        // Потім рендер станцій (вони перезапишуть фундамент)
        foreach (ResourceType type in tileData.ResorsesLayers[resourceLayer].ResourceTilesData.Keys)
        {
            foreach (ResourсeStation station in tileData.ResorsesLayers[resourceLayer].ResourceTilesData[type].Stantions)
            {
                tileMapGame.SetCell(resourceLayer, station.Position, tileMapData.ResourceAtlasId, tileMapData.ResourceTiles[type]);
            }
        }

        // Оновлення тайлів трави на основі сусідніх тайлів (перехід)
        foreach (var terrainTile in tileData.GetTerrainTilesData(0).TerrainTilesData)
        {
            // Знаходимо всі відповідні варіанти сусідніх тайлів трави
            List<TerrainMatch> matches = new();

            foreach (var variant in tileMapData.TerrainVariants)
            {
                Vector2I terrainMatch = terrainTile + variant.Value;
                if (tileData.GetTerrainTilesData(1).TerrainTilesData.Contains(terrainMatch)) matches.Add(variant.Key);
            }

            // Немає відповідностей - продовжуємо
            if (matches.Count == 0) continue;

            // Фундамент маски
            List<List<int>> tileCorners = new(){
                new() {0, 0},
                new() {0, 0}
            };

            // Заповнення маски за відповідностями
            foreach (TerrainMatch match in matches)
            {
                foreach (var pos in tileMapData.TerrainCorners[match])
                {
                    int i = (int)pos.X;
                    int j = (int)pos.Y;
                    tileCorners[i][j] |= 1;
                }
            }

            // Визначення типу тайлу трави на основі маски
            int numer = tileCorners[0][0] 
                + tileCorners[0][1] * 2 
                + tileCorners[1][0] * 4 
                + tileCorners[1][1] * 8;
            if (numer == 15) tileData.GetTerrainTilesData(1).TerrainTilesData.Add(terrainTile);

            tileMapGame.SetCell(1, terrainTile, tileMapData.TerrainAtlasId, tileMapData.RenderTilesGrass[numer]);
        }
    }
}


