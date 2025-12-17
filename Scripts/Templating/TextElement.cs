using System.Collections.Generic;
using Godot;

namespace TTSS.Scripts.Templating;





public partial class TextElement : TemplateElement
{
	private LineEdit _caption;
	private ColorPickerButton _foregroundColor;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		
		ElementType = TemplateElementType.Text;

		_caption = GetNode<LineEdit>("%Caption");
		_foregroundColor = GetNode<ColorPickerButton>("%ForegroundColor");
		
		
		_caption.TextChanged += _  => OnElementUpdated();
		_foregroundColor.ColorChanged += _  => OnElementUpdated();
		

	}

	public override List<TextureFactory.TextureObject> ElementData
	{
		get
		{
			var l = new List<TextureFactory.TextureObject>();

			var t = new TextureFactory.TextureObject
			{
				CenterX = Position.X,
				CenterY = Position.Y,
				Height = Position.Height,
				Width = Position.Width,
				RotationDegrees = Position.Rotation,
				Text = _caption.Text,
				ForegroundColor = _foregroundColor.Color,
			};
			
			l.Add(t);
			return l;
		}
		
	}

	private static int ForceParse(string s)
	{
		if (int.TryParse(s, out var i)) return i;
		return 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

public class TextElementData
{
	public string Text { get; set; }
	public Color ForegroundColor { get; set; }
}