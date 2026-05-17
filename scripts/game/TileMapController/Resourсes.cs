using Godot;
using System.Collections.Generic;

namespace CityGame.scripts.Models.Resource;

public partial class Resourсes : Node
{
    [Signal] 
    public delegate void PlacementConfirmedEventHandler(Vector2I cell, ResourceType type);
    [Signal] 
    public delegate void DeletionConfirmedEventHandler(Vector2I cell);
    [Signal]
    public delegate void HasConstructWithTypeEventHandler(Vector2I cell, ResourceType type);
    private GlobalManager globalManager;
    private HBoxContainer statsUI;

    private Dictionary<ResourceType, Label> resourceStats = new()
    {
        { ResourceType.Fuel, null },
        { ResourceType.Ore, null },
        { ResourceType.Ingot, null },
        { ResourceType.Food, null },
        { ResourceType.Money, null },
        { ResourceType.People, null }
    };
    private Dictionary<ResourceType, int> resourceAmounts = new()
    {
        { ResourceType.Fuel, 0 },
        { ResourceType.Ore, 0 },
        { ResourceType.Ingot, 0 },
        { ResourceType.Food, 0 },
        { ResourceType.Money, 0 },
        { ResourceType.People, 0 }
    };

    private TileData tileData;
    private float timeSinceLastUpdate = 0f;
    private const float UPDATE_INTERVAL = 1.0f;

    public override void _Ready()
    {

        globalManager = GlobalManager.Instance;
        globalManager.Connect(GlobalManager.SignalName.TilePlaced, Callable.From<Vector2I, ResourceType>(OnTilePlaced));
        globalManager.Connect(GlobalManager.SignalName.TileDeleted, Callable.From<Vector2I>(OnTileDeleted));
        globalManager.Connect(GlobalManager.SignalName.HasConstruct, Callable.From<Vector2I>(OnHasConstruct));

        statsUI = GetTree()
                .Root
                .GetNode<Node>("game")
                .GetNode<HBoxContainer>("UI/MargUI/HUD/TopWindow/StatsBox");

        foreach (Label label in statsUI.GetChildren())
		{
            foreach (ResourceType type in resourceStats.Keys)
            {
                if (label.GetMeta("Type").ToString() == type.ToString())
                {
                    resourceStats[type] = label;
                    break;
                }
            }
		}

        tileData = GlobalManager.Instance.TileData;
		
        foreach (ResourceType type in resourceAmounts.Keys)
        {
            resourceStats[type].Text = type.ToString() + ": " + tileData.ResorsesLayers[4].ResourceTilesData[type].Amount;
        }
    }

    private void OnTilePlaced(Vector2I cell, ResourceType type)
    {
        bool canPlace = tileData.ResorsesLayers[4].AddPosition(type, cell);
        if (canPlace) EmitSignal(SignalName.PlacementConfirmed, cell, (int)type);
    }

    private void OnTileDeleted(Vector2I cell)
    {
        bool canDelete = tileData.ResorsesLayers[4].ClearPosition(tileData.ResorsesLayers[4].TypeAtPosition(cell), cell);
        if (canDelete) EmitSignal(SignalName.DeletionConfirmed, cell);
    }

    private void OnHasConstruct(Vector2I cell)
    {
        EmitSignal(SignalName.HasConstructWithType, cell, (int)tileData.ResorsesLayers[4].TypeAtPosition(cell));
    }
    public override void _Process(double delta)
    {
        if (!GlobalManager.Instance.GameTicking) return;
        
        timeSinceLastUpdate += (float)delta;
        
        if (timeSinceLastUpdate >= UPDATE_INTERVAL)
		{
            tileData.ResorsesLayers[4].TickAction();
            foreach (ResourceType type in resourceAmounts.Keys)
            {
                resourceStats[type].Text = type.ToString() + ": " + tileData.ResorsesLayers[4].ResourceTilesData[type].Amount;
                resourceAmounts[type] = tileData.ResorsesLayers[4].ResourceTilesData[type].Amount;
            }
            timeSinceLastUpdate = 0f;
        }
    }
}
