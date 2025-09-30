namespace Godot;

using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class Karya2 : Node2D
{
	private BentukDasar _bentukDasar = new BentukDasar();
	private TransformasiFast _transformasi = new TransformasiFast();

	public override void _Ready()
	{
		ScreenUtils.Initialize(GetViewport()); // Inisialisasi ScreenUtils
		QueueRedraw();
	}

	public override void _Draw()
	{
		MarginPixel(); // Gambar margin
		
		// Duplikasikan bunga tipe 1 (4 kelopak) sebanyak 4 kali
		DrawBunga(4, new Vector2(150, 150), ColorUtils.ColorStorage(1)); // Merah
		DrawBunga(4, new Vector2(350, 150), ColorUtils.ColorStorage(2)); // Hijau
		DrawBunga(4, new Vector2(150, 350), ColorUtils.ColorStorage(3)); // Biru
		DrawBunga(4, new Vector2(350, 350), ColorUtils.ColorStorage(4)); // Kuning

		// Duplikasikan bunga tipe 2 (8 kelopak) sebanyak 6 kali
		DrawBunga(8, new Vector2(550, 150), ColorUtils.ColorStorage(5)); // Magenta
		DrawBunga(8, new Vector2(750, 150), ColorUtils.ColorStorage(6)); // Cyan
		DrawBunga(8, new Vector2(550, 350), ColorUtils.ColorStorage(7)); // Orange
		DrawBunga(8, new Vector2(750, 350), ColorUtils.ColorStorage(8)); // Rose
		DrawBunga(8, new Vector2(550, 550), ColorUtils.ColorStorage(9)); // Violet
		DrawBunga(8, new Vector2(750, 550), ColorUtils.ColorStorage(10)); // Azure
	}

	// Fungsi parametrik untuk menggambar bunga dengan jumlah kelopak tertentu di pusat tertentu
	private void DrawBunga(int jumlahKelopak, Vector2 pusat, Godot.Color warnaKelopak)
	{
		// Gambar lingkaran pusat
		var lingkaranPoin = _bentukDasar.Lingkaran(pusat, 20); // Radius 20 untuk pusat
		GraphicsUtils.PutPixelAll(this, lingkaranPoin, GraphicsUtils.DrawStyle.CircleDot, ColorUtils.ColorStorage(4), gap: 1); // Kuning untuk pusat

		// Generate satu kelopak ellips (horizontal)
		var kelopakAwal = _bentukDasar.Elips(new Vector2(pusat.X + 40, pusat.Y), 40, 20); // Ellips dengan radiusX=40, radiusY=20, offset ke kanan

		// Hitung sudut rotasi per kelopak
		float sudutPerKelopak = Mathf.DegToRad(360f / jumlahKelopak);

		// Buat kelopak lainnya dengan rotasi
		for (int i = 0; i < jumlahKelopak; i++)
		{
			// Buat matriks rotasi
			Matrix4x4 rotationMatrix = TransformasiFast.Identity();
			_transformasi.RotationClockwise(ref rotationMatrix, sudutPerKelopak * i, pusat); // Rotasi terhadap pusat bunga

			// Translasi kelopak ke posisi relatif dari pusat sebelum rotasi
			List<Vector2> kelopakTranslated = new List<Vector2>();
			foreach (var poin in kelopakAwal)
			{
				kelopakTranslated.Add(poin - new Vector2(pusat.X + 40, pusat.Y) + pusat); // Adjust ke pusat
			}

			// Apply rotasi
			List<Vector2> kelopakRotated = _transformasi.GetTransformPoint(rotationMatrix, kelopakTranslated);

			// Gambar kelopak
			GraphicsUtils.PutPixelAll(this, kelopakRotated, GraphicsUtils.DrawStyle.EllipseDot, warnaKelopak, gap: 1);
		}
	}

	private void MarginPixel()
	{
		var margin = _bentukDasar.Margin();
		GraphicsUtils.PutPixelAll(this, margin, GraphicsUtils.DrawStyle.DotDot, ColorUtils.ColorStorage(0)); // Putih
	}

	public override void _ExitTree()
	{
		NodeUtils.DisposeAndNull(_bentukDasar, "_bentukDasar");
		NodeUtils.DisposeAndNull(_transformasi, "_transformasi");
		base._ExitTree();
	}
}
