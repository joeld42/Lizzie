using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public partial class VcToken : VisualComponentBase
{
	private Sprite3D _frontSprite;
	private Sprite3D _backSprite;


	private TokenTextureSubViewport _frontView;
	private TokenTextureSubViewport _backView;
	
	public override void _Ready()
	{
		base._Ready();
		Visible = true;
		ComponentType = VisualComponentType.Token;
		StackingCollider = GetNode<Area3D>("Area3D");

		_frontSprite = GetNode<Sprite3D>("FrontSprite");
		_backSprite = GetNode<Sprite3D>("BackSprite");
	}

	public override void _Process(double delta)
	{
		if (flipInProcess)
		{
			ProcessFlip(delta);
		}
		base._Process(delta);
	}

	public override GeometryInstance3D DragMesh => _frontSprite;

	public override CommandResponse ProcessCommand(SceneController.VisualCommand command)
	{
		if (command == SceneController.VisualCommand.Flip)
		{
			return StartFlip();
		}
		
		return base.ProcessCommand(command);
	}
	

	private float _flipRate = 720;	//degrees per second
	private bool _showFace = true;
	private int _rotMult = 1;
	private float _targetZ;
	private bool flipInProcess;
	private CommandResponse StartFlip()
	{
		flipInProcess = true;
		_showFace = !_showFace;
		_rotMult = _showFace ? -1 : 1;
		_targetZ = _showFace ? 0 : 180;

		var c = new Change
		{
			Action = Change.ChangeType.Transform,
			Begin = Transform,
			Component = this
		};

		float rot = (float)Math.PI;

		if (_targetZ == 0) rot *= -1;
		
		c.End = Transform.RotatedLocal(new Vector3(0, 0, 1), rot);

		return new CommandResponse(true, c);
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
	
	public override bool Build(Dictionary<string, object> parameters)
	{
		_frontSprite = GetNode<Sprite3D>("FrontSprite");
		_backSprite = GetNode<Sprite3D>("BackSprite");

		if (!InitializeParameters(parameters)) return false;

		switch (Mode)
		{
			case 0:
				BuildQuick();
				break;
			
			case 1:
				BuildCustom();
				break;
			
			case 2:
				BuildImport();
				break;
		}
		

		YHeight = Thickness;
		
		Scale = new Vector3(Width, Thickness, Height);
		
		return true;
	}

	private void BuildQuick()
	{
		_frontView = GetNode<TokenTextureSubViewport>("FrontViewport");
		_frontView.Ready += CreateQuickFrontTexture;

		if (DifferentBack)
		{
			GD.Print("Build Back");
			_backView = GetNode<TokenTextureSubViewport>("BackViewport");
			_backView.Ready += CreateQuickBackTexture;
		}
	}

	private void BuildCustom()
	{
		_frontView = GetNode<TokenTextureSubViewport>("FrontViewport");
		_frontView.Ready += CreateCustomFrontTexture;
		
		if (DifferentBack)
		{
			_backView = GetNode<TokenTextureSubViewport>("BackViewport");
			_backView.Ready += CreateCustomBackTexture;
		}
		
	}
	
	private void BuildImport(){}

	private void CreateCustomFrontTexture()
	{
		if (!File.Exists(FrontImage)) return;
		
		_frontView.SetViewPortMode(TokenTextureSubViewport.ShapeViewportMode.Texture);
		_frontView.SetShape((TokenTextureSubViewport.TokenShape) Shape);
		_frontView.SetTexture(LoadTexture(FrontImage));

		var t = _frontView.GetTexture();

		float pixelSize = 0.95f / t.GetWidth();
		_frontSprite.PixelSize = pixelSize;
		_frontSprite.Texture = t;
		
		
		if (!DifferentBack)
		{
			_backSprite.PixelSize = pixelSize;
			_backSprite.Texture = t;
		}
		
	}
	
	//In all the texture creation routines, we scale the pixel size to 0.95.
	//This is the base size of the front and bottom sprites in the token,
	//and matches the side mesh (the gray punchboard texture
	//The width is 0.95 so the highlight mesh, which is size 1.0, so it still shows.
	
	private void CreateCustomBackTexture()
	{
		if (!File.Exists(BackImage)) return;
		
		_backView.SetViewPortMode(TokenTextureSubViewport.ShapeViewportMode.Texture);
		_backView.SetShape((TokenTextureSubViewport.TokenShape) Shape);
		var t = _backView.GetTexture();

		float pixelSize = 0.95f / t.GetWidth();
		_backSprite.PixelSize = pixelSize;
		_backView.SetTexture(LoadTexture(BackImage));

		_backSprite.Texture = _backView.GetTexture();
		
	}
	
	
	private void CreateQuickFrontTexture()
	{
		_frontView.SetBackgroundColor(FrontBgColor);
		_frontView.SetText(FrontCaption);
		_frontView.SetTextColor(FrontCaptionColor);
		_frontView.SetShape((TokenTextureSubViewport.TokenShape) Shape);
		
		var t = _frontView.GetTexture();

		float pixelSize = 0.95f / t.GetWidth();
		_frontSprite.PixelSize = pixelSize;
		_frontSprite.Texture = t;

		if (!DifferentBack)
		{
			_backSprite.PixelSize = pixelSize;
			_backSprite.Texture = t;
		}
	}
	
	private void CreateQuickBackTexture()
	{
		_backView.SetBackgroundColor(BackBgColor);
		_backView.SetText(BackCaption);
		_backView.SetTextColor(BackCaptionColor);
		_backView.SetShape((TokenTextureSubViewport.TokenShape)Shape);
		
		var t = _backView.GetTexture();

		float pixelSize = 0.95f / t.GetWidth();
		_backSprite.PixelSize = pixelSize;
		_backSprite.Texture = t;
	}

	private bool InitializeParameters(Dictionary<string, object> parameters)
	{
		base.Build(parameters);

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
			
			if (parameters[nameof(Thickness)] is float t)
			{
				Thickness = t / 10f;
			}
		}

		FrontImage = parameters["FrontImage"].ToString();
		BackImage = parameters["BackImage"].ToString();

		Shape = (int)parameters["Shape"];
		Mode = (int)parameters["Mode"];
		FrontBgColor = (Color)parameters["FrontBgColor"];
		FrontCaption = parameters["FrontCaption"].ToString();
		FrontCaptionColor = (Color)parameters["FrontCaptionColor"];

		DifferentBack = (bool)parameters["DifferentBack"];
		
		BackBgColor = (Color)parameters["BackBgColor"];
		BackCaption = parameters["BackCaption"].ToString();
		BackCaptionColor = (Color)parameters["BackCaptionColor"];

		return true;
	}

	public override List<string> ValidateParameters(Dictionary<string, object> parameters)
	{
		var ret = new List<string>();

		//must have a name and height. Width/length optional
		if (parameters.ContainsKey(nameof(InstanceName)))
		{
			if (string.IsNullOrEmpty(parameters[nameof(InstanceName)].ToString()))
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

	private float Height;
	private float Width;
	private float Thickness;
	private string FrontImage;
	private string BackImage;
	private int Shape;
	private int Mode;
	private Color FrontBgColor;
	private string FrontCaption;
	private Color FrontCaptionColor;
	private bool DifferentBack;
	private Color BackBgColor;
	private string BackCaption;
	private Color BackCaptionColor;
	
}
