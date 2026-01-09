using Godot;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

public partial class Sys : Node
{
	[Export]
	public PackedScene pixelSce;

	[Export]
	public Node root;

	[Export]
	public Button btnReset;

	[Export]
	public Button btnRecognize;

	[Export]
	public HBoxContainer digitsContainer;

	public List<Digit> digits;

	private List<List<TextureRect>> pixels;

	private int size = 28;

	private Vector2 pixelSize;

	private string url = "http://localhost:8000";

	private MyPy myPy;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pixels = [];
		for (int row = 0; row < size; ++row)
		{
			pixels.Add([]);
			for (int col = 0; col < size; ++col)
			{
				pixels[row].Add(null);
			}
		}
		CallDeferred("createPixels");

		btnReset.Pressed += ResetPixels;
		btnRecognize.Pressed += Recognize;

		digits = digitsContainer
			.GetChildren()
			.OfType<Digit>()
			.ToList();

		myPy = new();
	}

	private void createPixels()
	{
		// 生成绘制网格
		for (int row = 0; row < size; ++row)
		{
			for (int col = 0; col < size; ++col)
			{
				var pixel = (TextureRect)pixelSce.Instantiate();
				pixel.Position = new Vector2(pixel.Size.X * col, pixel.Size.Y * row);
				root.AddChild(pixel);

				pixel.SelfModulate = Color.Color8(0, 0, 0);
				pixels[row][col] = pixel;
				pixelSize = pixel.Size;
			}
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse evMouse)
		{
			Vector2 screenPosition = evMouse.Position;
			// GD.Print("Mouse position", screenPosition);
			if (Input.IsMouseButtonPressed(MouseButton.Left))
			{
				var row = (int)(screenPosition.Y / pixelSize.Y);
				var col = (int)(screenPosition.X / pixelSize.X);

				// 增色
				void Tint(int row, int col, int type)
				{
					if (0 <= col && col < size && 0 <= row && row < size)
					{
						var color = pixels[row][col].SelfModulate;
						if (type == 0)
						{
							color.V = color.V + 1;
						}
						else
						{
							color.V = color.V + 0.2f;
						}
						pixels[row][col].SelfModulate = color;
					}
				}

				Tint(row, col, 0);
				Tint(row - 1, col, 1);
				Tint(row + 1, col, 1);
				Tint(row, col - 1, 1);
				Tint(row, col + 1, 1);
			}
		}
	}

	public void ResetPixels()
	{
		for (int row = 0; row < size; ++row)
		{
			for (int col = 0; col < size; ++col)
			{
				pixels[row][col].SelfModulate = new Color(0, 0, 0);
			}
		}
	}

	public void Recognize()
	{
		// 整理数据
		var data = new float[size][];
		for (int row = 0; row < size; ++row)
		{
			data[row] = new float[size];
		}
		for (int row = 0; row < size; ++row)
		{
			for (int col = 0; col < size; ++col)
			{
				data[row][col] = pixels[row][col].SelfModulate.V;
			}
		}

		var res_prob = new List<float>(9);

		// 调用python api
		dynamic res;
		using (Py.GIL())
		{
			res = myPy.api.recognize(data);
		}
		foreach (PyObject each in res)
		{
			res_prob.Add(each.As<float>());
		}

		// 更新UI
		for (int i = 0; i <= 9; ++i)
		{
			digits[i].SetValue(res_prob[i]);
		}
	}

}
