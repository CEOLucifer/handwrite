using Godot;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Sys : Node
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

	public List<Digit> digits;

	private string url = "http://localhost:8000";

	private MyPy myPy;

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

		btnReset.Pressed += ResetPixels;
		btnRecognize.Pressed += Recognize;
		cbRealTime.Toggled += (state) =>
		{
			if (state)
			{
				Recognize();
			}
		};

		digits = digitsContainer
			.GetChildren()
			.OfType<Digit>()
			.ToList();

		myPy = new();
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
		int unit = drawPanel.unit;
		var data = new float[unit][];
		for (int row = 0; row < unit; ++row)
		{
			data[row] = new float[unit];
		}
		for (int row = 0; row < unit; ++row)
		{
			for (int col = 0; col < unit; ++col)
			{
				data[row][col] = drawPanel.pixels[row][col].color.MyLuminance();
			}
		}

		// 调用python api
		dynamic res;
		using (Py.GIL())
		{
			res = myPy.api.recognize(data);
		}
		for (int i = 0; i <= 9; ++i)
		{
			this.res_prob[i] = res[i];
		}

		// 更新UI
		for (int i = 0; i <= 9; ++i)
		{
			digits[i].SetValue(res_prob[i]);
		}
	}

}
