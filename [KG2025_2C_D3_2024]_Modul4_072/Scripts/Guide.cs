using Godot;
using System;

public partial class Guide : Control
{
	public override void _Ready()
	{
		// Initialization code here
	}

	private void _on_BtnBack_pressed()
	{
		var error = GetTree().ChangeSceneToFile("res://Scenes/Welcome.tscn");
		if (error != Error.Ok)
		{
			GD.Print("Scene Tidak Ada");
		}
	}
}
