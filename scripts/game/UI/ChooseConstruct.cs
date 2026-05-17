using Godot;
using System;

public partial class ChooseConstruct : VBoxContainer
{
	[Signal]
	public delegate void PlacementPressedEventHandler(ResourceType type);

	Button confirmButton;
	HBoxContainer gridContainer;

	ResourceType type;
	GlobalManager globalManager;
	
	public override void _Ready()
    {
		confirmButton = GetNode<Button>("Confirm");
		gridContainer = GetNode<HBoxContainer>("VariantsBox");

		foreach (Button button in gridContainer.GetChildren())
		{
			button.Pressed += () =>
			{
				if (Enum.TryParse<ResourceType>((string)button.GetMeta("Type"), out var parsedType)) type = parsedType;
			};
		}
		
		globalManager = GlobalManager.Instance;
		globalManager.Connect(GlobalManager.SignalName.ConstructNotPlacable, Callable.From<Vector2I>(OnConstructNotPlacable));
		globalManager.Connect(GlobalManager.SignalName.ConstructPlacable, Callable.From<Vector2I>(OnConstructPlacable));
		globalManager.Connect(GlobalManager.SignalName.HasConstruct, Callable.From<Vector2I>(OnHasConstruct));
		globalManager.Connect(GlobalManager.SignalName.PlacementConfirmed, Callable.From<Vector2I, ResourceType>(OnPlacementConfirmed));
		globalManager.Connect(GlobalManager.SignalName.DeletionConfirmed, Callable.From<Vector2I>(OnDeletionConfirmed));

		confirmButton.Pressed += () => 
		{
			EmitSignal(SignalName.PlacementPressed, (int)type);
		};
    }

	private void OnConstructNotPlacable(Vector2I cell) => Visible = false;
	private void OnConstructPlacable(Vector2I cell) => Visible = true;
	private void OnHasConstruct(Vector2I cell) => Visible = false;
	private void OnPlacementConfirmed(Vector2I cell, ResourceType type) => Visible = false;
	private void OnDeletionConfirmed(Vector2I cell) => Visible = true;

}
