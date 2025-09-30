namespace Godot;

using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class Karya1 : Node2D
{
	private BentukDasar _bentukDasar = new BentukDasar();
	private TransformasiFast _transformasi = new TransformasiFast();
	private Vector2 _initialPosition = new Vector2(100, 100); // Posisi awal persegi
	private float _ukuran = 50; // Ukuran persegi
	private int _counter = 0; // Counter untuk melacak transformasi

	// Dictionary untuk teks berdasarkan counter
	private Dictionary<int, string> counterText = new Dictionary<int, string>
	{
		{ 0, "Translasi" },
		{ 1, "Scaling" },
		{ 2, "Rotasi" },
		{ 3, "Translasi + Scaling" },
		{ 4, "Scaling + Translasi" }
	};

	public override void _Ready()
	{
		ScreenUtils.Initialize(GetViewport()); // Inisialisasi ScreenUtils
		// Hubungkan sinyal tombol (pastikan BtnNext dan BtnPrev ada di scene)
		var btnNext = GetNode<Button>("BtnNext");
		var btnPrev = GetNode<Button>("BtnPrev");
		btnNext.Pressed += ButtonNextPressed;
		btnPrev.Pressed += ButtonPrevPressed;
		QueueRedraw();
	}

	public override void _Draw()
	{
		MarginPixel(); // Gambar margin
		DrawTransformasi(); // Gambar transformasi berdasarkan counter
		// Tampilkan teks transformasi saat ini
		Font font = ThemeDB.FallbackFont;
		DrawString(font, new Vector2(50, 30), counterText[_counter], HorizontalAlignment.Left, 200, 20, Colors.White);
	}

	private void DrawTransformasi()
	{
		// Definisikan titik-titik awal persegi
		List<Vector2> persegiPoints = new List<Vector2>()
		{
			_initialPosition,
			new Vector2(_initialPosition.X + _ukuran, _initialPosition.Y),
			new Vector2(_initialPosition.X + _ukuran, _initialPosition.Y + _ukuran),
			new Vector2(_initialPosition.X, _initialPosition.Y + _ukuran)
		};

		List<Vector2> transformedPoints = null;
		Matrix4x4 transformMatrix = TransformasiFast.Identity();
		Godot.Color color = ColorUtils.ColorStorage(1); // Default merah

		// Pilih transformasi berdasarkan counter
		switch (_counter)
		{
			case 0: // Translasi (Geser ke X=300, Y=100)
				_transformasi.Translation(ref transformMatrix, 200, 0);
				transformedPoints = _transformasi.GetTransformPoint(transformMatrix, persegiPoints);
				color = ColorUtils.ColorStorage(1); // Merah
				break;
			case 1: // Scaling (Skala 2x di X, 1.5x di Y)
				_transformasi.Scaling(ref transformMatrix, 2f, 1.5f, _initialPosition);
				transformedPoints = _transformasi.GetTransformPoint(transformMatrix, persegiPoints);
				color = ColorUtils.ColorStorage(2); // Hijau
				break;
			case 2: // Rotasi (Putar 45 derajat searah jarum jam)
				_transformasi.RotationClockwise(ref transformMatrix, Mathf.DegToRad(45), _initialPosition);
				transformedPoints = _transformasi.GetTransformPoint(transformMatrix, persegiPoints);
				color = ColorUtils.ColorStorage(3); // Biru
				break;
			case 3: // Translasi + Scaling (Geser ke X=300, Y=300, lalu skala)
				_transformasi.Translation(ref transformMatrix, 200, 200);
				_transformasi.Scaling(ref transformMatrix, 1.5f, 1.5f, new Vector2(_initialPosition.X + 200, _initialPosition.Y + 200));
				transformedPoints = _transformasi.GetTransformPoint(transformMatrix, persegiPoints);
				color = ColorUtils.ColorStorage(4); // Kuning
				break;
			case 4: // Scaling + Translasi (Skala dulu, lalu geser ke X=500, Y=100)
				_transformasi.Scaling(ref transformMatrix, 1.5f, 1.5f, _initialPosition);
				_transformasi.Translation(ref transformMatrix, 400, 0);
				transformedPoints = _transformasi.GetTransformPoint(transformMatrix, persegiPoints);
				color = ColorUtils.ColorStorage(5); // Magenta
				break;
		}

		// Gambar poligon transformasi
		var polygon = _bentukDasar.Polygon(transformedPoints);
		GraphicsUtils.PutPixelAll(this, polygon, GraphicsUtils.DrawStyle.DotDot, color, 3, 2);
		PrintUtils.PrintVector2List(transformedPoints, counterText[_counter]);
	}

	private void MarginPixel()
	{
		var margin = _bentukDasar.Margin();
		GraphicsUtils.PutPixelAll(this, margin, GraphicsUtils.DrawStyle.DotDot, ColorUtils.ColorStorage(6)); // Cyan
	}

	private void ButtonNextPressed()
	{
		_counter = (_counter + 1) % 5; // Siklus 0-4
		QueueRedraw();
	}

	private void ButtonPrevPressed()
	{
		_counter = (_counter - 1 + 5) % 5; // Siklus 0-4, hindari negatif
		QueueRedraw();
	}

	public override void _ExitTree()
	{
		NodeUtils.DisposeAndNull(_bentukDasar, "_bentukDasar");
		NodeUtils.DisposeAndNull(_transformasi, "_transformasi");
		base._ExitTree();
	}
}
