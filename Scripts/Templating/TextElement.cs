using System.Collections.Generic;
using Godot;

namespace TTSS.Scripts.Templating;





public partial class TextElement : TemplateElement
{
	[Export] private LineEdit _caption;
	[Export] private ColorPickerButton _foregroundColor;

	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		
		ElementType = ITemplateElement.TemplateElementType.Text;

		Parameters.Add(new TemplateParameter{ Name = "Text" });
		Parameters.Add(new TemplateParameter{ Name = "ForegroundColor", Value = (Colors.Black).ToString()});
		
	}

	public override List<TextureFactory.TextureObject> ElementData
	{
		get
		{
			var l = new List<TextureFactory.TextureObject>();

			var t = new TextureFactory.TextureObject
			{
				CenterX = Parameters.EvaluateNumberParameter("X"),
				CenterY = Parameters.EvaluateNumberParameter("Y"),
				Height = Parameters.EvaluateNumberParameter("Height"),
				Width = Parameters.EvaluateNumberParameter("Width"),
				RotationDegrees = Parameters.EvaluateNumberParameter("Rotation"),
				Text = Parameters.EvaluateTextParameter( "Text"),
				ForegroundColor = Parameters.EvaluateColorParameter("ForegroundColor"),
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