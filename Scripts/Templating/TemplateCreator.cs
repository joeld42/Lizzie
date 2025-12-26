using Godot;
using System;
using System.Net.Mime;
using TTSS.Scripts.Templating;

public partial class TemplateCreator : MarginContainer
{
	[Export] private TextureRect _preview;

	private VBoxContainer _elementContainer;
	private VBoxContainer _paramContainer;
	
	private Button _testButton;

	private PackedScene _stringParam;
	private PackedScene _numberParam;
	private PackedScene _colorParam;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_elementContainer = GetNode<VBoxContainer>("%TemplateElements");
		_paramContainer = GetNode<VBoxContainer>("%TemplateParams");
		_testButton = GetNode<Button>("%TestButton");
		_testButton.Pressed += TestFunction;

		_stringParam = GD.Load<PackedScene>("res://Scenes/Templating/StringParam.tscn");
		_numberParam = GD.Load<PackedScene>("res://Scenes/Templating/NumericParam.tscn");
		_colorParam = GD.Load<PackedScene>("res://Scenes/Templating/ColorParam.tscn");
		/*
		foreach (var c in _groupContainer.GetChildren())
		{
			if (c is GroupElement ge)
			{
				ge.ElementUpdated += UpdateTexture;
			}
		}
		*/
	}

	private void UpdateTexture(object sender, TemplateElementUpdateEventArgs e)
	{
		var td = new TextureFactory.TextureDefinition
		{
			BackgroundColor = Colors.White,
			Height = 256,
			Width = 256 * 250 / 350,
			Shape = TextureFactory.TextureShape.Square
		};

		foreach (var l in e.ElementData)
		{
			td.Objects.Add(new TextureFactory.TextureObject
			{
				Width = td.Width,
				Height = td.Height,
				CenterX = l.Width,
				CenterY = l.Height,
				Multiline = true,
				Text= l.Text,
				ForegroundColor = l.ForegroundColor,
				Font= new SystemFont(),
				Type = TextureFactory.TextureObjectType.Text
			});
		}
		
		TextureFactory.GenerateTexture(td, UpdatePreview);	
	}


	private void TestFunction()
	{
		var td = new TextureFactory.TextureDefinition
		{
			BackgroundColor = Colors.White,
			Height = 256,
			Width = 256 * 250 / 350,
			Shape = TextureFactory.TextureShape.Square
		};

	
		
		td.Objects.Add(new TextureFactory.TextureObject
		{
			Width = td.Width,
			Height = td.Height,
			CenterX = td.Width / 2,
			CenterY = td.Height / 2,
			Multiline = true,
			Text= "HELLO",
			ForegroundColor = Colors.Black,
			Font= new SystemFont(),
			Type = TextureFactory.TextureObjectType.Text
		});

		TextureFactory.GenerateTexture(td, UpdatePreview);
		
	}

	private void UpdatePreview(ImageTexture texture)
	{
		_preview.Texture = texture;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public TextureFactory TextureFactory { get; set; }
}
