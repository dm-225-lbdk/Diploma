using Godot;
using CityGame.scripts.Models.Resource;

public partial class GlobalManager : Node
{
    [Signal]
    public delegate void PlacementPressedEventHandler(ResourceType type);
    [Signal]
    public delegate void DeletionPressedEventHandler();
    [Signal]
    public delegate void ConstructNotPlacableEventHandler(Vector2I cell);
    [Signal]
    public delegate void ConstructPlacableEventHandler(Vector2I cell);
    [Signal]
    public delegate void TilePlacedEventHandler(Vector2I cell, ResourceType type);
    [Signal]
    public delegate void TileDeletedEventHandler(Vector2I cell);
    [Signal]
    public delegate void PlacementConfirmedEventHandler(Vector2I cell, ResourceType type);
    [Signal]
    public delegate void DeletionConfirmedEventHandler(Vector2I cell);
    [Signal]
    public delegate void HasConstructEventHandler(Vector2I cell);
    [Signal]
    public delegate void HasConstructWithTypeEventHandler(Vector2I cell, ResourceType type);

    public static GlobalManager Instance { get; private set; }
    public TileData TileData { get; set; }
    public bool GameTicking {get; set; } = true;
    public string SaveFilePath {get; set; }

    private Placement tileMapPlacement;
    private Resourсes tileMapResourсes;
    private ChooseConstruct chooseConstruct;
    private ControlConstruct controlConstruct;
    
    public override void _Ready()
    {

        if (Instance == null)
        {
            Instance = this;
            TileData ??= new TileData();
        }
        else
        {
            QueueFree();
        }
    }

    public void InitializeGameReferences(Node gameNode)
    {
        tileMapPlacement = gameNode.GetNode<Placement>("TileMapGame/Placement");
        tileMapPlacement.Connect(Placement.SignalName.TilePlaced, Callable.From<Vector2I, ResourceType>(OnTilePlaced));
        tileMapPlacement.Connect(Placement.SignalName.TileDeleted, Callable.From<Vector2I>(OnTileDeleted));

        tileMapPlacement.Connect(Placement.SignalName.ConstructNotPlacable, Callable.From<Vector2I>(OnConstructNotPlacable));
        tileMapPlacement.Connect(Placement.SignalName.ConstructPlacable, Callable.From<Vector2I>(OnConstructPlacable));
        tileMapPlacement.Connect(Placement.SignalName.HasConstruct, Callable.From<Vector2I>(OnHasConstruct));

        tileMapResourсes = gameNode.GetNode<Resourсes>("TileMapGame/Resourсes");
        tileMapResourсes.Connect(Resourсes.SignalName.PlacementConfirmed, Callable.From<Vector2I, ResourceType>(OnPlacementConfirmation));
        tileMapResourсes.Connect(Resourсes.SignalName.DeletionConfirmed, Callable.From<Vector2I>(OnDeletionConfirmation));
        tileMapResourсes.Connect(Resourсes.SignalName.HasConstructWithType, Callable.From<Vector2I, ResourceType>(OnHasConstructWithType));

        chooseConstruct = gameNode.GetNode<ChooseConstruct>("UI/MargUI/HUD/MenusUI/MenuBox/ChooseConstruct");
        chooseConstruct.Connect(ChooseConstruct.SignalName.PlacementPressed, Callable.From<ResourceType>(OnPlacementPressed));

        controlConstruct = gameNode.GetNode<ControlConstruct>("UI/MargUI/HUD/MenusUI/MenuBox/ControlConstruct");
        controlConstruct.Connect(ControlConstruct.SignalName.DeletionPressed, Callable.From(OnDeletionPressed));
    }

    private void OnTilePlaced(Vector2I cell, ResourceType type) => EmitSignal(SignalName.TilePlaced, cell, (int)type);

    private void OnTileDeleted(Vector2I cell) => EmitSignal(SignalName.TileDeleted, cell);

    private void OnConstructNotPlacable(Vector2I cell) => EmitSignal(SignalName.ConstructNotPlacable, cell);

    private void OnConstructPlacable(Vector2I cell) => EmitSignal(SignalName.ConstructPlacable, cell);

    private void OnPlacementPressed(ResourceType type) => EmitSignal(SignalName.PlacementPressed, (int)type);

    private void OnDeletionPressed() => EmitSignal(SignalName.DeletionPressed);

    private void OnHasConstruct(Vector2I cell) => EmitSignal(SignalName.HasConstruct, cell);

    private void OnHasConstructWithType(Vector2I cell, ResourceType type) => EmitSignal(SignalName.HasConstructWithType, cell, (int)type);

    private void OnPlacementConfirmation(Vector2I cell, ResourceType type) => EmitSignal(SignalName.PlacementConfirmed, cell, (int)type);

    private void OnDeletionConfirmation(Vector2I cell) => EmitSignal(SignalName.DeletionConfirmed, cell);
}
