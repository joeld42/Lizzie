using Godot;
using System;
using System.Collections.Generic;

public partial class VcBoard : VisualComponentBase
{
	private MeshInstance3D _boardMesh;
	private float _boardThickness = 0.2f; //2mm

	private Dictionary<String, int> _surfaceForMaterial = new();

	public override void _Ready()
	{
		base._Ready();
		Visible = true;
		ComponentType = VisualComponentType.Board;

		_boardMesh = GetNode<MeshInstance3D>("BoardMesh");
		HighlightMesh = GetNode<MeshInstance3D>("HighlightMesh");
	}

	public override void _Process(double delta)
	{
		if (flipInProcess)
		{
			ProcessFlip(delta);
		}
		base._Process(delta);
	}



	private float _flipRate = 720;	//degrees per second
	private bool _showFace = true;
	private int _rotMult = 1;
	private float _targetZ;
	private bool flipInProcess;
	private void StartFlip()
	{
		flipInProcess = true;
		_showFace = !_showFace;
		_rotMult = _showFace ? -1 : 1;
		_targetZ = _showFace ? 0 : 180;
	}

	private void ProcessFlip(double delta)
	{
		var curZ = RotationDegrees.Z;
		float newZ = curZ + (_flipRate * (float)delta * _rotMult);
		if (_showFace)
		{
			if (newZ < _targetZ)
			{
				newZ = _targetZ;
				flipInProcess = false;
			}
		}
		else
		{
			if (newZ > _targetZ)
			{
				newZ = _targetZ;
				flipInProcess = false;
			}
		}

		RotationDegrees = new Vector3(RotationDegrees.X, RotationDegrees.Y, newZ);
	}

	public override bool Build(Dictionary<string, object> parameters, TextureFactory textureFactory)
	{
		_boardMesh = GetNode<MeshInstance3D>("BoardMesh/Cube");
		MainMesh = _boardMesh;

		HighlightMesh = GetNode<MeshInstance3D>("HighlightMesh");

		// Figure out which materials are used for the front and back. We could just
		// hardcoded these but that is more likely to break if someone re-exports the model
		for (int i=0; i < _boardMesh.Mesh.GetSurfaceCount(); i++)
		{
			var mtl = _boardMesh.Mesh.SurfaceGetMaterial( i );
			_surfaceForMaterial[mtl.ResourceName] = i;
		}

		base.Build(parameters, textureFactory);

		if (parameters.ContainsKey(nameof(Height)))
		{
			if (parameters[nameof(Height)] is float h)
			{
				if (h <= 0) return false;
				Height = h / 10f;
			}

			if (parameters[nameof(Width)] is float w)
			{
				Width = w / 10f;
			}
		}

		FrontImage = parameters["FrontImage"].ToString();
		BackImage = parameters["BackImage"].ToString();

		var tf = LoadTexture(FrontImage);

		var mat = new StandardMaterial3D();
		mat.AlbedoTexture = tf;


		_boardMesh.SetSurfaceOverrideMaterial( _surfaceForMaterial["M_Top"], mat );
		_boardMesh.SetSurfaceOverrideMaterial( _surfaceForMaterial["M_Bottom"], mat );

		var tb = LoadTexture(BackImage);
		var mat2 = new StandardMaterial3D();

		if (!string.IsNullOrEmpty(BackImage))
		{
			mat2.AlbedoTexture = tb;
		}
		else
		{
			mat2.AlbedoColor = new Color(0, 0, 0);
		}

		YHeight = _boardThickness;

		Scale = new Vector3(Width, _boardThickness, Height);

		var r = new RectangleShape2D();
		r.Size = new Vector2(Width, Height);

		ShapeProfiles.Add(r);

		return true;
	}

	public override List<string> ValidateParameters(Dictionary<string, object> parameters)
	{
		var ret = new List<string>();

		//must have a name and height. Width/length optional
		if (parameters.ContainsKey(nameof(ComponentName)))
		{
			if (string.IsNullOrEmpty(parameters[nameof(ComponentName)].ToString()))
				ret.Add("Instance Name may not be blank");
		}
		else
		{
			ret.Add("Instance Name not included");
		}

		if (parameters.ContainsKey(nameof(Height)))
		{
			if (parameters[nameof(Height)] is float h)
			{
				if (h <= 0) ret.Add("Height must be > 0");
			}
		}
		else
		{
			ret.Add("Height not included");
		}

		if (parameters.TryGetValue(nameof(Width), out var w))
		{
			if (w is float d)
			{
				if (d <= 0) ret.Add("Diameter must be > 0");
			}
		}
		else
		{
			ret.Add("Diameter not included");
		}

		if (parameters.TryGetValue(nameof(FrontImage), out var parameter))
		{
			if (string.IsNullOrEmpty(parameter.ToString()))
			{
				ret.Add("Front Image must be included");
			}
		}

		return ret;
	}

	public override float MaxAxisSize => Math.Max(Height, Width);
	public override GeometryInstance3D DragMesh => MainMesh;

	private float Height;
	private float Width;
	private string FrontImage;
	private string BackImage;
}
