using Godot;

[GlobalClass]
public partial class Test : Node
{
	[Export]
	public TextureRect tr;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(tr.SelfModulate.V);
		Color c = new Color();
		c.V = 999;
		GD.Print(c.V);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
    }
}
