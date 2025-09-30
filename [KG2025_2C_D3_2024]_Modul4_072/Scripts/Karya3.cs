namespace Godot;

using Godot;
using System.Collections.Generic;

public partial class Karya3 : Node2D
{
	// Kelas dan variabel member
	public class ShapeData
	{
		public List<Vector2> Vertices { get; set; }
		public Color ShapeColor { get; set; }
		public int ColorIndex { get; set; }
	}

	private BentukDasar _bentukDasar;
	private Transformasi _transformasi;
	private List<ShapeData> _allShapes = new List<ShapeData>();
	private List<Vector2> _baseHexagonVertices;
	private List<Vector2> _baseTriangleVertices;
	private List<Vector2> _baseDiamondVertices;
	private List<Vector2> _baseTrapesiumVertices; // <-- BARU

	public override void _Ready()
	{
		_bentukDasar = new BentukDasar();
		_transformasi = new Transformasi();

		// Inisialisasi "Template" Bentuk Dasar
		_baseHexagonVertices = new List<Vector2>();
		for (int i = 0; i < 6; i++) {
			float angle = Mathf.DegToRad(60 * i);
			_baseHexagonVertices.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
		}
		_baseTriangleVertices = new List<Vector2>();
		for (int i = 0; i < 3; i++) {
			float angle = Mathf.DegToRad(120 * i - 90);
			_baseTriangleVertices.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
		}
		_baseDiamondVertices = new List<Vector2> {
			new Vector2(0.5f, 0), new Vector2(0, 0.5f),
			new Vector2(-0.5f, 0), new Vector2(0, -0.5f)
		};
		_baseTrapesiumVertices = new List<Vector2> { // <-- BARU
			new Vector2(-0.5f, 0.5f),
			new Vector2(0.5f, 0.5f),
			new Vector2(0.25f, -0.25f),
			new Vector2(-0.25f, -0.25f)
		};

		GambarCandy();
		GambarPenguin();
	}
	// KUMPULAN FUNGSI GAMBAR

	private void GambarCandy()
	{
		AddHexagon(new Vector2(255, 150), 60, 180, 1);//(x, y,(scale, roration, colorUtils))
		AddTriangle(new Vector2(195, 115), 35, 180, 7);
		AddTriangle(new Vector2(195, 185), 35, 0, 7);
		AddTriangle(new Vector2(350, 150), 35, 90, 7);
		AddTriangle(new Vector2(100, 150), 35, 150, 7);
		AddDiamond(new Vector2(165, 150), new Vector2(100, 60), 90, 2);
	}

	private void GambarPenguin()
	{
		AddHexagon(new Vector2(580, 205), 60, 180, 1);
		AddHexagon(new Vector2(580, 310), 60, 180, 1);
		AddDiamond(new Vector2(565, 125), new Vector2(100, 60), 150, 2);
		AddDiamond(new Vector2(518, 257), new Vector2(100, 60), 90, 2);
		AddDiamond(new Vector2(641, 257), new Vector2(100, 60), 90, 2);
		AddDiamond(new Vector2(518, 362), new Vector2(100, 60), 90, 2);
		AddDiamond(new Vector2(641, 362), new Vector2(100, 60), 90, 2);
		AddDiamond(new Vector2(683, 245), new Vector2(110, 30), 136, 10);
		AddDiamond(new Vector2(476, 245), new Vector2(110, 30), 223, 10);
	}
	// FUNGSI _Draw()
	public override void _Draw()
	{
		if (_allShapes == null || _allShapes.Count == 0) return;

		foreach (var shape in _allShapes)
		{
			Color fillColor = (shape.ColorIndex == 0) ? Colors.Black : shape.ShapeColor;
			DrawPolygon(shape.Vertices.ToArray(), new Color[] { fillColor });
			List<Vector2> polygonPixels = _bentukDasar.Polygon(shape.Vertices);
			GraphicsUtils.PutPixelAll(this, polygonPixels, GraphicsUtils.DrawStyle.DotDot, fillColor);
		}
	}

	// FUNGSI PEMBANTU
	private List<Vector2> TransformVertices(List<Vector2> baseVertices, Vector2 scale, float rotationDeg, Vector2 position)
	{
		float[,] matrix = new float[3, 3];
		Transformasi.Matrix3x3Identity(matrix);
		Vector2 dummyCoord = Vector2.Zero;
		_transformasi.Scaling(matrix, scale.X, scale.Y, Vector2.Zero);
		_transformasi.RotationClockwise(matrix, rotationDeg, Vector2.Zero);
		_transformasi.Translation(matrix, position.X, position.Y, ref dummyCoord);
		return _transformasi.GetTransformPoint(matrix, baseVertices);
	}

	private void AddHexagon(Vector2 position, float scale, float rotation, int colorIndex)
	{
		var transformedVertices = TransformVertices(_baseHexagonVertices, new Vector2(scale, scale), rotation, position);
		_allShapes.Add(new ShapeData { Vertices = transformedVertices, ShapeColor = ColorUtils.ColorStorage(colorIndex), ColorIndex = colorIndex });
	}

	private void AddTriangle(Vector2 position, float scale, float rotation, int colorIndex)
	{
		var transformedVertices = TransformVertices(_baseTriangleVertices, new Vector2(scale, scale), rotation, position);
		_allShapes.Add(new ShapeData { Vertices = transformedVertices, ShapeColor = ColorUtils.ColorStorage(colorIndex), ColorIndex = colorIndex });
	}

	private void AddDiamond(Vector2 position, Vector2 size, float rotation, int colorIndex)
	{
		var transformedVertices = TransformVertices(_baseDiamondVertices, size, rotation, position);
		_allShapes.Add(new ShapeData { Vertices = transformedVertices, ShapeColor = ColorUtils.ColorStorage(colorIndex), ColorIndex = colorIndex });
	}

	private void AddTrapesium(Vector2 position, Vector2 size, float rotation, int colorIndex) // <-- BARU
	{
		var transformedVertices = TransformVertices(_baseTrapesiumVertices, size, rotation, position);
		_allShapes.Add(new ShapeData { Vertices = transformedVertices, ShapeColor = ColorUtils.ColorStorage(colorIndex), ColorIndex = colorIndex });
	}

	public override void _ExitTree()
	{
		_bentukDasar?.Dispose();
	}
}
