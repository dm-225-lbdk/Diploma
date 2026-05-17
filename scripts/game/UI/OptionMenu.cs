using Godot;
using System;

public partial class OptionMenu : PanelContainer
{
	private Button backButton;
	private Button saveButton;
	private Button quitButton;
	private TileData tileData;

	public override void _Ready()
	{
		tileData = GlobalManager.Instance.TileData;

		backButton = GetNode<Button>("VBoxContainer/BackButton");
		backButton.Pressed += () =>
		{
			Visible = !Visible;
			GlobalManager.Instance.GameTicking = !GlobalManager.Instance.GameTicking;
		};

		quitButton = GetNode<Button>("VBoxContainer/QuitButton");
		quitButton.Pressed += () =>
		{
			SaveGame();
			GlobalManager.Instance.GameTicking = true;
			tileData = null;
			GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
		};

		saveButton = GetNode<Button>("VBoxContainer/SaveButton");
		saveButton.Pressed += () =>
		{
			SaveGame();
		};
	}
	
	private void SaveGame()
	{
		// Створення JSON
		string jsonString = Json.Stringify(Variant.From(tileData.ToGodotDictionary()), "\t");

		string savePath = GlobalManager.Instance.SaveFilePath;

		string saveDir = "user://saves/" + savePath;
DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.LocalizePath(saveDir));


		string filePath = saveDir + "/save.json";

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);

		file.StoreString(jsonString);
	}
}
