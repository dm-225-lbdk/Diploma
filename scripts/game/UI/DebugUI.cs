using Godot;
using System.Collections.Generic;

public partial class DebugUI : VBoxContainer
{
	private List<Label> debugLabelList = new();

	public override void _Ready()
	{
		var cameraController = GetTree().Root.GetNode<Node>("game").GetNode<CameraController>("Camera2D");
		cameraController.ZoomDebug += ZoomDebugGetted;

		for (int i = 0; i < 4; i++)
		{
			Label label = new();
			AddChild(label);
			debugLabelList.Add(label);
		}
    }
	
	private void ZoomDebugGetted(string[] keys, string[] values)
	{
		for (int i = 0; i < debugLabelList.Count; i++) 
			debugLabelList[i].Text = $"{keys[i]}: {values[i]}";
    }
}
