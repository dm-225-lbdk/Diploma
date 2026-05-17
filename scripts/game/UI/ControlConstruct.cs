using Godot;

public partial class ControlConstruct : VBoxContainer
{
    [Signal]
    public delegate void DeletionPressedEventHandler();

    Button deleteButton;
    ResourceType type;
    GlobalManager globalManager;

    public override void _Ready()
    {
        globalManager = GlobalManager.Instance;
        globalManager.Connect(GlobalManager.SignalName.ConstructNotPlacable, Callable.From<Vector2I>(OnConstructNotPlacable));
        globalManager.Connect(GlobalManager.SignalName.ConstructPlacable, Callable.From<Vector2I>(OnConstructPlacable));
        globalManager.Connect(GlobalManager.SignalName.HasConstruct, Callable.From<Vector2I>(OnHasConstruct));
        globalManager.Connect(GlobalManager.SignalName.PlacementConfirmed, Callable.From<Vector2I, ResourceType>(OnPlacementConfirmed));
        globalManager.Connect(GlobalManager.SignalName.DeletionConfirmed, Callable.From<Vector2I>(OnDeletionConfirmed));

        deleteButton = GetNode<Button>("Delete");
        deleteButton.Pressed += () => 
        {
            EmitSignal(SignalName.DeletionPressed);
        };
    }

    private void OnConstructNotPlacable(Vector2I cell) => Visible = false;
    private void OnConstructPlacable(Vector2I cell) => Visible = false;
    private void OnHasConstruct(Vector2I cell) => Visible = true;
    private void OnDeletionConfirmed(Vector2I cell) => Visible = false;
    private void OnPlacementConfirmed(Vector2I cell, ResourceType type) => Visible = true;
}