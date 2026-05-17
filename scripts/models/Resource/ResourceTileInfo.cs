using System.Collections.Generic;
using Godot;
namespace CityGame.scripts.Models.Resource;
public class ResourceTileInfo
{
    public int Amount { get; set; }
    public List<ResourсeStation> Stantions { get; set; }

    // Конструктор
    public ResourceTileInfo()
    {
        Amount = 0;
        Stantions = new();
    }

    // Наявність позиції у станцій
    public bool ContainsPosition(Vector2I position)
    {
        foreach (var station in Stantions) if (station.Position == position) return true;
        return false;
    }

    // Видалити позицію сток-ресурсу
    public void ClearTargetPosition(Vector2I position)
    {
        Stantions.Find(s => s.TargetPosition == position).TargetPosition = Vector2I.Zero;
    }

    // Додати позицію
    public void AddPosition(Vector2I position, Vector2I targetPosition)
    {
        Stantions.Add(new ResourсeStation(position, targetPosition));
    }

    // Видалити позицію
    public void ClearPosition(Vector2I position)
    {
        Stantions.Remove(Stantions.Find(s => s.Position == position));
    }

    // Серіалізація в словник Godot
    public Godot.Collections.Dictionary ToGodotDictionary()
    {
        Godot.Collections.Array stationsArray = new();
        foreach (var station in Stantions) stationsArray.Add(station.ToGodotDictionary());
        
        return new Godot.Collections.Dictionary
        {
            { "Amount", Amount },
            { "Stantions", stationsArray }
        };
    }

    // Десеріалізація зі словника Godot
    public static ResourceTileInfo FromGodotDictionary(Godot.Collections.Dictionary dict)
    {
        ResourceTileInfo info = new()
        {
            Amount = dict["Amount"].AsInt32()
        };

        Godot.Collections.Array arr = dict["Stantions"].AsGodotArray();

        foreach (var item in arr)
        {
            Godot.Collections.Dictionary stationDict = item.AsGodotDictionary();
            info.Stantions.Add(ResourсeStation.FromDictionary(stationDict));
        }
        return info;
    }
}