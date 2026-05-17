using Godot;
using System;

public partial class Settings : Control
{
	private Button backButton;
    public override void _Ready()
    {

    	backButton = GetNode<Button>("UI/backButton");
    	backButton.Pressed += () => {
			GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
		};

    }
}
