using Godot;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class Sys : Node2D
{
	[Export]
	public DrawPanel drawPanel;

	[Export]
	public Button btnReset;

	[Export]
	public Button btnRecognize;

	[Export]
	public HBoxContainer digitsContainer;

	[Export]
	public CheckButton cbRealTime;

	[Export]
	public HSlider hSlider_unit_row;

	[Export]
	public HSlider hSlider_unit_col;

	[Export]
	public HSlider hSlider_pixel_size;

	[Export]
	public Label label_unit;

	[Export]
	public Label lab_pixel_size;

	public List<Digit> digits;

	private MyPy myPy;

	public dynamic res;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		drawPanel.onDraw += () =>
		{
			// 实时模式
			if (cbRealTime.ButtonPressed)
			{
				Recognize();
			}
		};

		btnReset.Pressed += () =>
		{
			ResetPixels();
			res = null;
		};
		btnRecognize.Pressed += Recognize;
		cbRealTime.Toggled += (state) =>
		{
			if (state)
			{
				Recognize();
			}
		};


		hSlider_unit_row.ValueChanged += (value) =>
		{
			var unit = (int)value;
			drawPanel.unit = new(unit, drawPanel.unit.Y);
			label_unit.Text = $"{drawPanel.unit.X} X {drawPanel.unit.Y}";
		};
		hSlider_unit_row.SetValueNoSignal(drawPanel.unit.X);

		hSlider_unit_col.ValueChanged += (value) =>
		{
			var unit = (int)value;
			drawPanel.unit = new(drawPanel.unit.X, unit);
			label_unit.Text = $"{drawPanel.unit.X} X {drawPanel.unit.Y}";
		};
		hSlider_unit_col.SetValueNoSignal(drawPanel.unit.X);

		label_unit.Text = $"{drawPanel.unit.X} X {drawPanel.unit.Y}";

		hSlider_pixel_size.ValueChanged += (value) =>
		{
			drawPanel.pixelSize = (int)value;
			lab_pixel_size.Text = $"像素大小：{drawPanel.pixelSize}";
		};
		hSlider_pixel_size.SetValueNoSignal(drawPanel.pixelSize);


		digits = digitsContainer
			.GetChildren()
			.OfType<Digit>()
			.ToList();

		myPy = new();
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}


	public override void _Draw()
	{
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
				DrawRect(
					new(new Vector2((float)res[i].x, (float)res[i].y) * drawPanel.pixelSize,
						 new Vector2((float)res[i].w * drawPanel.pixelSize,
									  (float)res[i].w * drawPanel.pixelSize)),
					Colors.Red,
					filled: false,
					width: 10
				);
			}
		}
	}



	public void ResetPixels()
	{
		drawPanel.ResetPixels();

		// 更新UI
		for (int i = 0; i <= 9; ++i)
		{
			digits[i].SetValue(0);
		}
	}


	private float[] res_prob = new float[10];
	public void Recognize()
	{
		// 整理数据
		var unit = drawPanel.unit;
		var data = new float[unit.X][];
		for (int row = 0; row < unit.X; ++row)
		{
			data[row] = new float[unit.Y];
		}
		for (int row = 0; row < unit.X; ++row)
		{
			for (int col = 0; col < unit.Y; ++col)
			{
				data[row][col] = drawPanel.pixels[row][col].color.MyLuminance();
			}
		}

		// 调用python api
		using (Py.GIL())
		{
			// 保存于res变量
			res = myPy.api.recognize(data);
		}
		GD.Print(res.__len__());
		// for (int i = 0; i <= 9; ++i)
		// {
		// 	this.res_prob[i] = res[i];
		// }

		// 更新UI
		// for (int i = 0; i <= 9; ++i)
		// {
		// 	digits[i].SetValue(res_prob[i]);
		// }
	}

}
