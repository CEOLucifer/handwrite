using Godot;
using System;

public partial class Digit : Control
{
	[Export]
	public Label label;

	[Export]
	public TextureProgressBar tpb;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetValue(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetValue(float v)
	{
		label.Text = $"{(v * 100).ToString("F1")} %";
		tpb.Value = v * 100;
	}
}
