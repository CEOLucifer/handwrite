using Godot;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SinglePanel : Control
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
	public HSlider hSlider_pixel_size;

	[Export]
	public Label lab_pixel_size;



	public List<Digit> digits;

	public Sys sys;

	public dynamic res;

	public double[] res_prob = new double[10];

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
			res = sys.myPy.api.recognize_28x28(data);
		}
		for (int i = 0; i <= 9; ++i)
		{
			this.res_prob[i] = res[i];
		}

		// 更新UI
		for (int i = 0; i <= 9; ++i)
		{
			digits[i].SetValue((float)res_prob[i]);
		}
	}
}
