using Godot;
using System;

/// <summary>
/// 控件：绘制数字识别边框
/// </summary>
public partial class BorderDrawer : Node2D
{
	public MultiPanel multiPanel;

	[Export]
	public DrawPanel drawPanel;

	[Export]
	public Font font;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
	{
		var res = multiPanel.res;

		// 绘制数字识别边框
		if (res != null)
		{
			int count = 0;
			try
			{
				count = res.__len__();
			}
			catch
			{
				// fallback: try as array
				if (res is Array arr)
					count = arr.Length;
			}
			for (int i = 0; i < count; ++i)
			{
				var x = (float)res[i].x;
				var y = (float)res[i].y;
				var w = (float)res[i].w;

				DrawRect(
					new(new Vector2(x, y) * drawPanel.pixelSize,
						 new Vector2(w * drawPanel.pixelSize,
									  w * drawPanel.pixelSize)),
					Colors.Red,
					filled: false,
					width: 10
				);

				// 显示识别出来的是什么数字
				var res_prob = res[i].probs;
				double max = res_prob[0];
				int i_max = 0;
				for (var ii = 0; ii <= 9; ++ii)
				{
					if (res_prob[ii] > max)
					{
						max = res_prob[ii];
						i_max = ii;
					}
				}
				if (font != null)
				{
					DrawString(font, new Vector2(x, y - 2) * drawPanel.pixelSize, $"{i_max}", modulate: Colors.Red, fontSize: 40);
				}
			}
		}
	}
}
