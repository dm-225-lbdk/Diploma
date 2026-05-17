using System.Collections.Generic;
using Godot;

public class TerrainsData
{
    // Лист позицій тайлів місцевості
    public List<Vector2I> TerrainTilesData { get; } = new();

    // Додати позицію
    public void AddTerrainTile(Vector2I position)
    {
        TerrainTilesData.Add(position);
    }

    // Серіалізація в словник Godot
    public Variant ToGodotDictionary()
    {
        var positionsArray = new Godot.Collections.Array();

        foreach (var pos in TerrainTilesData)
        {
            var posArr = new Godot.Collections.Array { pos.X, pos.Y };
            positionsArray.Add(posArr);
        }

        var dict = new Godot.Collections.Dictionary
        {
            { "Positions", positionsArray }
        };

        return dict;
    }

    // Десеріалізація зі словника Godot
    public static TerrainsData FromGodotDictionary(Godot.Collections.Dictionary dict)
    {
        var data = new TerrainsData();

        var positions = dict["Positions"].AsGodotArray();

        foreach (var item in positions)
        {
            var posArr = item.AsGodotArray();
            int x = posArr[0].AsInt32();
            int y = posArr[1].AsInt32();
            data.AddTerrainTile(new Vector2I(x, y));
        }

        return data;
    }

}