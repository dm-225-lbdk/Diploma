using Godot;

namespace CityGame.scripts.Models.Resource;

public class ResourсeStation
{
    // Позиція станції, її ID
    public Vector2I Position { get; set; }
    // Активність станції
    public bool IsActive { get; set; }
    // Позиція, де зберізається займаний сток-ресурс
    public Vector2I TargetPosition { get; set; }

    // Конструктор під час першого створення станції
    public ResourсeStation(Vector2I position, Vector2I targetPosition)
    {
        Position = position;
        IsActive = false;
        TargetPosition = targetPosition;
    }

    // Конструктор під час десеріалізації
    public ResourсeStation(Vector2I position, Vector2I targetPosition, bool isActive)
    {
        Position = position;
        IsActive = isActive;
        TargetPosition = targetPosition;
    }

    // Серіалізація в словник Godot
    public Godot.Collections.Dictionary ToGodotDictionary()
    {
        return new Godot.Collections.Dictionary
        {
            { "Position", new Godot.Collections.Array { Position.X, Position.Y } },
            { "IsActive", IsActive },
            { "TargetPosition", new Godot.Collections.Array { TargetPosition.X, TargetPosition.Y } }
        };
    }

    // Десеріалізація зі словника Godot
    public static ResourсeStation FromDictionary(Godot.Collections.Dictionary dict)
    {
        Godot.Collections.Array posArr = dict["Position"].AsGodotArray();
        Godot.Collections.Array targetPosArr = dict["TargetPosition"].AsGodotArray();

        Vector2I position = new(posArr[0].AsInt32(), posArr[1].AsInt32());
        Vector2I targetPosition = new(targetPosArr[0].AsInt32(), targetPosArr[1].AsInt32());
        bool isActive = dict["IsActive"].AsBool();

        return new ResourсeStation(position, targetPosition, isActive);
    }
}