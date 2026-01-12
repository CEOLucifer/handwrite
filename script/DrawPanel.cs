using Godot;
using System;
using System.Collections.Generic;

public struct Pixel
{
	public Color color = new(0, 0, 0);

	public Pixel()
	{
	}

}

/// <summary>
/// 画板
/// </summary>
[GlobalClass]
public partial class DrawPanel : Node2D
{
	public List<List<Pixel>> pixels = new();

	public float size = 1000;

	private int _unit = 28;

	[Export]
	public Vector2 orig;

	public Action onDraw;

	[Export(PropertyHint.Range, "1,100")]
	public int unit
	{
		get => _unit;
		set
		{
			_unit = Math.Clamp(value, 1, int.MaxValue);
			// 预分配好空间
			pixels.EnsureCapacity(unit);
			// 补充不够的行
			var oldCnt = pixels.Count;
			for (var i = 0; i < unit - oldCnt; ++i)
			{
				pixels.Add(new());
			}
			for (var row = 0; row < unit; ++row)
			{
				// 每行预留空间
				pixels[row].EnsureCapacity(unit);
				// 补充像素
				var oldRowCnt = pixels[row].Count;
				for (var i = 0; i < unit - oldRowCnt; ++i)
				{
					pixels[row].Add(new());
				}
			}

		}
	}

	public float pixelSize => size / unit;

	public override void _Ready()
	{
		base._Ready();
		unit = _unit;
		GD.Print($"{pixels.Count}, {pixels[0].Count}");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		QueueRedraw();
	}


	public override void _Draw()
	{
		var rectSize = new Vector2(pixelSize, pixelSize);
		for (int row = 0; row < unit; ++row)
		{
			for (int col = 0; col < unit; ++col)
			{
				DrawRect(
					new(orig + new Vector2(col, row) * pixelSize,
						rectSize),
					pixels[row][col].color
				);
			}
		}

	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse evMouse)
		{
			Vector2 mousePos = evMouse.Position;
			// GD.Print("Mouse position", screenPosition);
			if (Input.IsMouseButtonPressed(MouseButton.Left))
			{
				var row = (int)(mousePos.Y / pixelSize);
				var col = (int)(mousePos.X / pixelSize);

				// 增色
				void Tint(int row, int col, int type)
				{
					if (0 <= col && col < unit && 0 <= row && row < unit)
					{
						var color = pixels[row][col].color;
						if (type == 0)
						{
							color.V += 1.0f;
						}
						else
						{
							color.V += 0.2f;
						}
						color.V = Math.Clamp(color.V, 0, 1);
						var newPixel = new Pixel();
						newPixel.color = color;
						pixels[row][col] = newPixel;
					}
				}

				Tint(row, col, 0);
				Tint(row - 1, col, 1);
				Tint(row + 1, col, 1);
				Tint(row, col - 1, 1);
				Tint(row, col + 1, 1);

				onDraw?.Invoke();

			}
		}
	}

	public void ResetPixels()
	{
		for (int row = 0; row < unit; ++row)
		{
			for (int col = 0; col < unit; ++col)
			{
				pixels[row][col] = new Pixel();
			}
		}

	}
}
