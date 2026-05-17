
using System.Collections.Generic;
using CityGame.scripts.Models.Resource;

public class TileData
{
    // Словник "номер шару - дані тайлів місцевості/ресурсів"
    public Dictionary<int, TerrainsData> TerrainsLayers { get; set;}
    public Dictionary<int, ResourсesData> ResorsesLayers { get; set; }

    // Конструктор шарів відносно редактору
    public TileData()
    {
        TerrainsLayers = new()
        {
            { 0, new TerrainsData() },
            { 1, new TerrainsData() },
            { 2, new TerrainsData() },
            { 3, new TerrainsData() },
            { 5, new TerrainsData() },
        };
        ResorsesLayers = new()
        {
            { 4, new ResourсesData() },
        };
    }

    public ResourсesData GetResourceTilesData(int layer) { return ResorsesLayers[layer]; }
    public TerrainsData GetTerrainTilesData(int layer) { return TerrainsLayers[layer]; }

    // Серіалізація в словник Godot
    public Godot.Collections.Dictionary ToGodotDictionary()
    {
        var terrainDict = new Godot.Collections.Dictionary();
        foreach (var kvp in TerrainsLayers)
        {
            terrainDict[kvp.Key] = kvp.Value.ToGodotDictionary();
        }

        var resourceDict = new Godot.Collections.Dictionary();
        foreach (var kvp in ResorsesLayers)
        {
            resourceDict[kvp.Key] = kvp.Value.ToGodotDictionary();
        }

        var dict = new Godot.Collections.Dictionary
        {
            { "TerrainsLayers", terrainDict },
            { "ResorsesLayers", resourceDict }
        };
        return dict;
    }

    // Десеріалізація зі словника Godot
    public static TileData FromGodotDictionary(Godot.Collections.Dictionary dict)
    {
        var data = new TileData
        {
            TerrainsLayers = new(),
            ResorsesLayers = new()
        };

        var terrainsDict = dict["TerrainsLayers"].AsGodotDictionary();
        foreach (var key in terrainsDict.Keys)
        {
            int layer = int.Parse(key.ToString());
            var terrainDict = terrainsDict[key].AsGodotDictionary();
            data.TerrainsLayers[layer] = TerrainsData.FromGodotDictionary(terrainDict);
        }

        var resLayersDict = dict["ResorsesLayers"].AsGodotDictionary();
        foreach (var key in resLayersDict.Keys)
        {
            int layer = int.Parse(key.ToString());
            var resDict = resLayersDict[key].AsGodotDictionary();

            data.ResorsesLayers[layer] = ResourсesData.FromGodotDictionary(resDict);
        }

        return data;
    }

}