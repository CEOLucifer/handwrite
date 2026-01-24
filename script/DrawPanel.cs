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

	private Vector2I _unit = new(28, 28);

	private float _pixelSize = 20;

	[Export(PropertyHint.Range, "0,1")]
	public float tintSpeed = 1.0f;

	[Export]
	public Vector2 orig;

	public Action onDraw;

	[Export(PropertyHint.Range, "1,100")]
	public Vector2I unit
	{
		get => _unit;
		set
		{
			var rowNew = Math.Clamp(value.X, 1, int.MaxValue);
			var colNew = Math.Clamp(value.Y, 1, int.MaxValue);
			_unit = new(rowNew, colNew);
			// 预分配好空间
			pixels.EnsureCapacity(unit.X);
			// 补充不够的行
			var oldCnt = pixels.Count;
			for (var i = 0; i < unit.X - oldCnt; ++i)
			{
				pixels.Add(new());
			}
			for (var row = 0; row < unit.X; ++row)
			{
				// 每行预留空间
				pixels[row].EnsureCapacity(unit.Y);
				// 补充像素
				var oldRowCnt = pixels[row].Count;
				for (var i = 0; i < unit.Y - oldRowCnt; ++i)
				{
					pixels[row].Add(new());
				}
			}

		}
	}

	[Export(PropertyHint.Range, "1,50")]
	public float pixelSize
	{
		get => _pixelSize;
		set => _pixelSize = value;
	}

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
		for (int row = 0; row < unit.X; ++row)
		{
			for (int col = 0; col < unit.Y; ++col)
			{
				DrawRect(
					new(orig + new Vector2(col, row) * pixelSize,
						rectSize),
					pixels[row][col].color
				);
			}
		}

	}

	public override void _UnhandledInput(InputEvent @event)
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
					if (0 <= col && col < unit.Y && 0 <= row && row < unit.X)
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
		for (int row = 0; row < pixels.Count; ++row)
		{
			for (int col = 0; col < pixels[0].Count; ++col)
			{
				pixels[row][col] = new Pixel();
			}
		}

	}
}
