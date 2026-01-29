using Godot;

[GlobalClass]
public partial class Sys : Node
{
	/// <summary>
	/// 切换模式按钮
	/// </summary>
	[Export]
	public OptionButton ob;

	public MyPy myPy;

	/// <summary>
	/// 当前的页面
	/// </summary>
	[Export]
	public Control panel;

	[Export]
	public CanvasLayer canvasLayer;

	[Export]
	public PackedScene singlePanel;
	[Export]
	public PackedScene multiPanel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		myPy = new();
		(panel as SinglePanel).sys = this;

		ob.ItemSelected += (value) =>
		{
			panel.QueueFree();

			switch (value)
			{
				case 0:
					{
						var newSinglePanel = (SinglePanel)singlePanel.Instantiate();
						canvasLayer.AddChild(newSinglePanel);
						canvasLayer.MoveChild(newSinglePanel, 0);
						panel = newSinglePanel;

						newSinglePanel.sys = this;

					}

					break;
				case 1:
					{
						var newMultiPanel = (MultiPanel)multiPanel.Instantiate();
						canvasLayer.AddChild(newMultiPanel);
						canvasLayer.MoveChild(newMultiPanel, 0);
						panel = newMultiPanel;

						newMultiPanel.sys = this;
					}
					break;

			}
		};
	}
}
