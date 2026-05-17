using Godot;
using System;

public partial class PointInfo : Label
{
	private GlobalManager globalManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		globalManager = GlobalManager.Instance;
		globalManager.Connect(GlobalManager.SignalName.HasConstructWithType, Callable.From<Vector2I, ResourceType>(OnHasConstructWithType));
		globalManager.Connect(GlobalManager.SignalName.ConstructNotPlacable, Callable.From<Vector2I>(OnNoConstruct));
		globalManager.Connect(GlobalManager.SignalName.ConstructPlacable, Callable.From<Vector2I>(OnNoConstruct));
		globalManager.Connect(GlobalManager.SignalName.PlacementConfirmed, Callable.From<Vector2I, ResourceType>(OnHasConstructWithType));

		globalManager.Connect(GlobalManager.SignalName.DeletionConfirmed, Callable.From<Vector2I>(OnNoConstruct));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void OnHasConstructWithType(Vector2I cell, ResourceType type)
	{
		GD.Print("Has construct: " + type + "\n" + cell);
		Visible = true;
		Text = type.ToString() + "-Станція" + "\n" + cell.ToString();
	} 
	private void OnNoConstruct(Vector2I cell)
	{
		Text = cell.ToString();
	}
}
