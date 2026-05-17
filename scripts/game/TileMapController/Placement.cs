using Godot;

public partial class Placement : Node
{
    [Signal]
    public delegate void TilePlacedEventHandler(Vector2I cell, ResourceType type);
    [Signal]
    public delegate void TileDeletedEventHandler(Vector2I cell);
    [Signal]
    public delegate void ConstructNotPlacableEventHandler(Vector2I cell);
    [Signal]
    public delegate void HasConstructEventHandler(Vector2I cell);
    [Signal]
    public delegate void ConstructPlacableEventHandler(Vector2I cell);

    private GlobalManager globalManager;
    private TileMapGame tileMapGame;
    private Vector2I previousCellPos = new(-1, -1);

    public override void _Ready()
    {
        tileMapGame = GetTree()
                .Root
                .GetNode<Node>("game")
                .GetNode<TileMapGame>("TileMapGame");

        globalManager = GlobalManager.Instance;
        globalManager.Connect(GlobalManager.SignalName.PlacementPressed, Callable.From<ResourceType>(OnPlacementPressed));
        globalManager.Connect(GlobalManager.SignalName.PlacementConfirmed, Callable.From<Vector2I, ResourceType>(OnPlacementConfirmation));
        globalManager.Connect(GlobalManager.SignalName.DeletionPressed, Callable.From(OnDeletionPressed));
        globalManager.Connect(GlobalManager.SignalName.DeletionConfirmed, Callable.From<Vector2I>(OnDeletionConfirmation));
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.DoubleClick && mouseEvent.ButtonIndex == MouseButton.Left && GlobalManager.Instance.GameTicking)
            {
                Vector2I cell = tileMapGame.LocalToMap(tileMapGame.ToLocal(tileMapGame.GetGlobalMousePosition()));

                if (cell == previousCellPos) return;

                if (tileMapGame.GetCellSourceId(4, cell) != -1 && tileMapGame.GetCellAtlasCoords(4, cell) != tileMapGame.Data.FoundationTile)
                {
                    EmitSignal(SignalName.HasConstruct, cell);
                }
                else if (tileMapGame.GetCellAtlasCoords(4, cell) == tileMapGame.Data.FoundationTile)
                {
                    EmitSignal(SignalName.ConstructPlacable, cell);
                }
                else
                {
                    EmitSignal(SignalName.ConstructNotPlacable, cell);
                }

                tileMapGame.EraseCell(5, previousCellPos);

                tileMapGame.SetCell(5, cell, tileMapGame.Data.TerrainAtlasId, tileMapGame.Data.SelectTile);
                previousCellPos = cell;
            }
        }
    }

    private void OnPlacementPressed(ResourceType type)
    {
        EmitSignal(SignalName.TilePlaced, previousCellPos, (int)type);
    }

    private void OnPlacementConfirmation(Vector2I cell, ResourceType type)
    {
        tileMapGame.SetCell(4, previousCellPos, tileMapGame.Data.ResourceAtlasId, tileMapGame.Data.ResourceTiles[type]);
    }

    private void OnDeletionPressed()
    {
        EmitSignal(SignalName.TileDeleted, previousCellPos);
    }

    private void OnDeletionConfirmation(Vector2I cell)
    {
        tileMapGame.SetCell(4, previousCellPos, tileMapGame.Data.ResourceAtlasId, tileMapGame.Data.FoundationTile);
    }
}
