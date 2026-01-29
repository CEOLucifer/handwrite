using Godot;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MultiPanel : Control
{
	[Export]
	public DrawPanel drawPanel;

	[Export]
	public Button btnReset;

	[Export]
	public Button btnRecognize;

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

	[Export]
	public BorderDrawer borderDrawer;

	public Sys sys;

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

		borderDrawer.multiPanel = this;
	}




	public void ResetPixels()
	{
		drawPanel.ResetPixels();
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
			res = sys.myPy.api.recognize(data);
		}
		GD.Print(res.__len__());
	}
}
