using Godot;
using System;

public partial class Menu : Control
{
	private Button startButton;
	private Button settingsButton;
	private Button quitButton;
    public override void _Ready()
    {

    	startButton = GetNode<Button>("UI/VBoxContainer/StartButton");
    	startButton.Pressed += () => {
			GetTree().ChangeSceneToFile("res://scenes/preGame.tscn");
		};

		settingsButton = GetNode<Button>("UI/VBoxContainer/VBoxContainer/SettingsButton");
		settingsButton.Pressed += () => {
			GetTree().ChangeSceneToFile("res://scenes/settings.tscn");
		};

		quitButton = GetNode<Button>("UI/VBoxContainer/VBoxContainer/QuitButton");
		quitButton.Pressed += () => {
			GetTree().Quit();
		};

    }
}
