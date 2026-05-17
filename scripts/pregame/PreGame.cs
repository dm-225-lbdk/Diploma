using Godot;
using System;
using System.Collections.Generic;

public partial class PreGame : Control
{
	private Button backButton;
	private Button createGameButton;
	private Button confirmButton;
	private LineEdit gameNameInput;
	private List<string> games = new();
	private DirAccess dir;
    public override void _Ready()
    {

		dir = DirAccess.Open("user://saves");

		dir.ListDirBegin();

		string fileName = dir.GetNext();
		while (fileName != "")
		{
			if (!fileName.StartsWith(".") && dir.CurrentIsDir())
			{
				games.Add(fileName);
			}
			fileName = dir.GetNext();
		}

		foreach (string game in games)
		{
            Button button = new()
            {
                Text = game
            };

            button.Pressed += () =>
			{
				LoadGame(game);
				GetTree().ChangeSceneToFile("res://scenes/game.tscn");
			};
            GetNode<HBoxContainer>("UI/VBoxContainer/scrollChoose/HBoxContainer").AddChild(button);
		}

    	backButton = GetNode<Button>("UI/VBoxContainer/backButton");
		backButton.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://scenes/menu.tscn");
		};

		PanelContainer panel = GetNode<PanelContainer>("UI/CrateGameMenu");
		
		createGameButton = GetNode<Button>("UI/VBoxContainer/scrollChoose/HBoxContainer/CreateGameButton");

		confirmButton = panel.GetNode<Button>("VBoxContainer/ConfirmButton");

		gameNameInput = panel.GetNode<LineEdit>("VBoxContainer/InputEdit");

		confirmButton.Pressed += () =>
		{
			string gameName = gameNameInput.Text.Trim();

			if (string.IsNullOrEmpty(gameName))
			{
				return;
			}	

			LoadGame(gameName);

			GetTree().ChangeSceneToFile("res://scenes/game.tscn");
		};

    	createGameButton.Pressed += () =>
		{
			panel.Visible = !panel.Visible;
		};

    }

	private void LoadGame(string savePath)
	{
		// Завантажуємо дані з JSON файлу
		string jsonPath = "user://saves/" + savePath + "/save.json";

		var tileData = new TileData();

		if (FileAccess.FileExists(jsonPath))
		{
			// Достаємо вміст файлу
			using var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
			string content = file.GetAsText();

			// Парсимо JSON в словник
			Json json = new();
			json.Parse(content);
			var dict = json.Data.AsGodotDictionary();
			tileData = TileData.FromGodotDictionary(dict);

			// Зберігаємо дані у GlobalManager
			GlobalManager.Instance.TileData = tileData;
		}

		GlobalManager.Instance.TileData = tileData;
		GlobalManager.Instance.SaveFilePath = savePath; 
	}
}
