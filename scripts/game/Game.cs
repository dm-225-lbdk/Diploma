using Godot;
using System;

public partial class Game : Control
{
	private Button optionsButton;
	private PanelContainer optionsMenu;

    public override void _Ready()
	{
		optionsMenu = GetNode<PanelContainer>("UI/MargUI/OptionMenu");
		optionsButton = GetNode<Button>("UI/MargUI/HUD/TopWindow/OptionButton");
		optionsButton.Pressed += OptionsButtonPressed;
		
		GlobalManager.Instance.InitializeGameReferences(this);
    }

	public override void _Process(double delta)
	{
		if (!optionsMenu.Visible)
		{
			optionsButton.Disabled = false;
		}
	}

	public void OptionsButtonPressed()
	{
		GlobalManager.Instance.GameTicking = !GlobalManager.Instance.GameTicking;
		optionsMenu.Visible = !optionsMenu.Visible;
		optionsButton.Disabled = true;
	}
}
